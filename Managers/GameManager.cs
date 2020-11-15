using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Singleton Code
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Init();
            _instance = this;
        }
    }
    #endregion

    #region Properties
    private int _level = 0;
    private bool _newGame;

    private Vector3 _playerPadding;

    [SerializeField]
    private GameObject _minePrefab;

    private Player _player;

    [SerializeField]
    private List<GameObject> _diamonds;
    private List<Mine> _mines;

    [SerializeField]
    private TextMeshProUGUI _levelText;

    [SerializeField]
    private PostProcessVolume _postProcessVolume;
    private ColorGrading _colorGrading;

    [SerializeField]
    private List<Collectible> _gems;

    private int _gemIndex;
    private int _levelScore;
    private long _currentScore;
    private long _finalScore;
    private int _gemCount;
    private bool _resetGame = false;
    private GameObject _currentGem;
    private BackgroundMusic _backgroundMusic;

    // Arcade and hardcore specific
    private bool _isArcade;
    private bool _isHardcore;
    private float _timer;
    private float _timerIncrement;
    private int _continueCost = 50;

    [SerializeField]
    private TextMeshProUGUI _timerValue;

    // Left and Right Borders
    private GameObject _leftBorderCollider;
    private GameObject _rightBorderCollider;
    private GameObject _topBorderCollider;
    private GameObject _bottomBorderCollider;
    private GameObject _leftBorderBar;
    private GameObject _rightBorderBar;
    private GameObject _topBorderBar;
    private GameObject _bottomBorderBar;

    // Gameover Overlay
    [SerializeField]
    private GameObject _gameOverObject;
    [SerializeField]
    private bool _counterComplete;
    [SerializeField]
    private Button _retry;

    [SerializeField]
    private List<GameObject> _powerUps;
    private bool _powerupAvailable;

    [SerializeField]
    private GameObject _continueGamePanel;

    public int Level { get { return _level; } set { _level = value; } }
    public int LevelScore { get { return _levelScore; } set { _levelScore = value; } }
    public float TimerIncrement { get { return _timerIncrement; } set { _timerIncrement = value; } }
    public long CurrentGemScore { get { return _currentScore; } set { _currentScore = value; } }
    public long FinalScore { get { return _finalScore; } set { _finalScore = value; } }

    #endregion

    void Init()
    {
        //Application.targetFrameRate = 60;
        _newGame = true;
        _gemIndex = 0;

        _colorGrading = null;

        _powerupAvailable = true;

        _timerIncrement = 1f;

        if (_postProcessVolume != null)
        {
            _postProcessVolume.profile.TryGetSettings(out _colorGrading);
        }

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _backgroundMusic = GameObject.Find("Background Music").GetComponent<BackgroundMusic>();

        if (_mines == null)
        {
            _mines = new List<Mine>();
        }

        if (SceneManager.GetActiveScene().name == "Arcade")
        {
            _counterComplete = false;
            _isArcade = true;
            _timer = 8f;
        }
        
        if (SceneManager.GetActiveScene().name == "Hardcore")
        {
            _counterComplete = false;
            _isHardcore = true;
            _timer = 5f;
        }

        PositionBorders();
        StartCoroutine(CreateGameBoard());
        StartCoroutine(StartBackgroundMusic());
    }

    private void ArcadeRestart()
    {
        _counterComplete = false;
        _timer = 12f;
    }

    // FPS in Normal Mode, to be removed after optimization.
    float timer = 0f;
    float delay = 1f;
    // -----------------------------------------------------

    private void Update()
    {
        // FPS in Normal Mode, to be removed after optimization.
        timer += Time.deltaTime;
        // -----------------------------------------------------

        if (IsArcade() && !_counterComplete)
        {
            _timer -= Time.deltaTime;

            _timerValue.text = ((int)_timer).ToString();

            if (_timer <= 0)
            {
                _counterComplete = true;
                _player.PlayerDead();
            }
        }
        // FPS in Normal Mode, to be removed after optimization.
        else
        {
            if (timer >= delay)
            {
                timer = 0f;
                _timerValue.text = ((int)(1.0f / Time.deltaTime)).ToString();
            }
        }
        // -----------------------------------------------------
    }

    public IEnumerator CheckGemCount()
    {
        _gemCount--;

        if (IsArcade())
        {
            _timer += _timerIncrement;
        }

        if (_gemCount <= 0)
        {
            _resetGame = true;

            if (IsArcade())
            {
                _timer += 5f;
            }

            yield return StartCoroutine(CreateGameBoard());
        }

        yield break;

    }

    private Vector3 GetRandomPosition()
    {
        float playerDistance = 0;
        Vector3 position = Vector3.zero;
        bool locationUnsafe = true;

        do
        {
            float gap = 100f;

            position = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(gap, Screen.width - gap), Random.Range(gap, Screen.height - gap), 10f));

            playerDistance = Vector3.Distance(position, Vector3.zero);

            if (playerDistance > 2)
            {
                locationUnsafe = false;
            }

        } while (locationUnsafe);

        return position;
    }

    public IEnumerator CreateGameBoard()
    {

        var powerUpChance = Random.value;
        var powerUpValue = Random.Range(0, 3);

        if (_resetGame || _newGame)
        {
            _resetGame = false;

            SetCurrentLevel();
            _levelText.text = _level.ToString();

            // Reset Score Multiplier on new level
            _player.ScoreMultiplier(0);

            if (Level == 0)
            {
                Debug.LogError("Level cannot be 0");
                yield break;
            }

            _mines = FindObjectsOfType<Mine>().ToList();

            foreach (var mine in _mines)
            {
                mine.Explode(true);
            }

            if (!_newGame)
            {
                _player.Block();

                yield return StartCoroutine(_player.RelocatePlayer());

                _player.Block();
            }

            _newGame = false;

            if (_gems == null)
            {
                Debug.LogError("Gem Scriptable Objects is Null.");
            }

            // change the gem and the colors.
            if (Level % 10 == 0)
            {
                ++_gemIndex;

                if (_gemIndex == _gems.Count)
                {
                    _gemIndex = 0;
                }

                _backgroundMusic.StartNewSong();
            }

            UpdateColorGrading(_gems[_gemIndex].Temperature, _gems[_gemIndex].Tint);

            for (int i = 0; i < _level; i++)
            {
                _currentGem = Instantiate(_gems[_gemIndex].Gem, GetRandomPosition(), Quaternion.identity);

                for (int j = 0; j < 3; j++)
                {
                    var newMine = Instantiate(_minePrefab, GetRandomPosition(), Quaternion.identity).GetComponent<Mine>();
                    _mines.Add(newMine);
                }
            }

            if (_powerUps != null)
            {
                if (powerUpValue == 0 && powerUpChance <= .03 && _powerupAvailable) // ScoreMultiplier
                {
                    _powerupAvailable = false;
                    Instantiate(_powerUps[powerUpValue], GetRandomPosition(), Quaternion.identity);
                }
                else if (powerUpValue == 1 && powerUpChance <= .13 && _powerupAvailable) // Invulnerability
                {
                    _powerupAvailable = false;
                    Instantiate(_powerUps[powerUpValue], GetRandomPosition(), Quaternion.identity);
                }
                else if (powerUpValue == 2 && powerUpChance <= .23 && _powerupAvailable) // Extra Life
                {
                    _powerupAvailable = false;
                    Instantiate(_powerUps[powerUpValue], GetRandomPosition(), Quaternion.identity);
                }
            }

            LevelScore = _gems[_gemIndex].ScoreValue;

            _gemCount = Level;
        }
    }

    public void ResetPowerUp()
    {
        _powerupAvailable = true;
    }

    IEnumerator StartBackgroundMusic()
    {
        yield return new WaitForSeconds(1f);

        if (Level % 10 == 0)
        {
            _backgroundMusic.StartNewSong();
        }
    }

    private void UpdateColorGrading(float newTempValue, float newTintValue)
    {
        _colorGrading.temperature.value = newTempValue;
        _colorGrading.tint.value = newTintValue;
    }

    private void PositionBorders()
    {
        _leftBorderCollider = GameObject.Find("Left");
        _rightBorderCollider = GameObject.Find("Right");
        _topBorderCollider = GameObject.Find("Top");
        _bottomBorderCollider = GameObject.Find("Bottom");
        _leftBorderBar = GameObject.Find("LeftBar");
        _rightBorderBar = GameObject.Find("RightBar");
        _topBorderBar = GameObject.Find("TopBar");
        _bottomBorderBar = GameObject.Find("BottomBar");

        // Colliders
        if (_leftBorderCollider == null)
        {
            Debug.LogError("GameManager::LeftBorderCollider object is null.");
        }
        else
        {
            _leftBorderCollider.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 2f, 10f));
        }

        if (_rightBorderCollider == null)
        {
            Debug.LogError("GameManager::LeftBorderCollider object is null.");
        }
        else
        {
            _rightBorderCollider.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2f, 10f));
        }

        if (_topBorderCollider == null)
        {
            Debug.LogError("GameManager::LeftBorderCollider object is null.");
        }
        else
        {
            _topBorderCollider.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, 0f, 10f));
        }

        if (_rightBorderCollider == null)
        {
            Debug.LogError("GameManager::LeftBorderCollider object is null.");
        }
        else
        {
            _bottomBorderCollider.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height, 10f));
        }

        // Borders
        if (_leftBorderBar == null)
        {
            Debug.LogError("GameManager::LeftBorderCollider object is null.");
        }
        else
        {
            _leftBorderBar.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 2f, 10f));
        }

        if (_rightBorderBar == null)
        {
            Debug.LogError("GameManager::LeftBorderCollider object is null.");
        }
        else
        {
            _rightBorderBar.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2f, 10f));
        }

        if (_topBorderBar == null)
        {
            Debug.LogError("GameManager::LeftBorderCollider object is null.");
        }
        else
        {
            _topBorderBar.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, 0f, 10f));
        }

        if (_bottomBorderBar == null)
        {
            Debug.LogError("GameManager::LeftBorderCollider object is null.");
        }
        else
        {
            _bottomBorderBar.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height, 10f));
        }
    }

    public PlayerControllerType GetControllerType()
    {
        return PlayerControllerType.STICK; // fix with options menu [CONTROLLER]
    }

    private void SetCurrentLevel()
    {
        Level += 1;

        int highestLevel = 0;

        if (IsArcade())
        {
            highestLevel = PlayerPrefs.GetInt("ArcadeLevel", 1);

            if (Level > highestLevel)
            {
                PlayerPrefs.SetInt("ArcadeLevel", Level);
            }
        }
        else if (IsHardcore())
        {
            highestLevel = PlayerPrefs.GetInt("HardcoreLevel", 1);

            if (Level > highestLevel)
            {
                PlayerPrefs.SetInt("HardcoreLevel", Level);
            }
        }
        else
        {
            highestLevel = PlayerPrefs.GetInt("HighestLevel", 1);

            if (Level > highestLevel)
            {
                PlayerPrefs.SetInt("HighestLevel", Level);
            }
        }
    }

    public int GetCurrentNormalLevel()
    {
        return PlayerPrefs.GetInt("HighestLevel", 1);
    }
    public int GetCurrentArcadeLevel()
    {
        return PlayerPrefs.GetInt("ArcadeLevel", 1);
    }

    public long GetHighScore()
    {
        long highScore = long.Parse(PlayerPrefs.GetString("HighScore", "0"));

        return highScore;
    }

    public bool IsArcade()
    {
        return _isArcade;
    }

    public bool IsHardcore()
    {
        return _isHardcore;
    }

    public void ContinueGame()
    {
        if (!IsHardcore())
        {
            var gems = PlayerPrefs.GetInt("GemsCollected", 0);

            if (gems > _continueCost)
            {
                gems -= _continueCost;
                PlayerPrefs.SetInt("GemsCollected", gems);
                _continueCost += 50;

                if (IsArcade())
                {
                    ArcadeRestart();
                }

                _player.gameObject.SetActive(true);
                _player.Init();

            }
        }

        //GameFinished = false;
        //// Reload player
        //_player.Init();

        //_player.transform.position = new Vector3(0f, 0f, 0f);
    }

    public void GameOver()
    {
        if ((IsArcade() || IsHardcore()) && !_counterComplete)
        {
            _counterComplete = true;
        }

        if (!IsHardcore())
        {
            Instantiate(_continueGamePanel, Vector3.zero, Quaternion.identity);
        }
        else
        {

        }
    }

    public int GetContinueCost()
    {
        Debug.Log($"Continue Cost: {_continueCost}");
        return _continueCost;
    }
}
