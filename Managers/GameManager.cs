using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using TMPro;

/// <summary>
/// Author:         Jay Wilson
/// Description:    Overall game manager.
/// </summary>
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
    private GameObject _minePrefab = null;

    private Player _player;

    [SerializeField]
    private List<GameObject> _diamonds;
    private List<Mine> _mines;

    [SerializeField]
    private TextMeshProUGUI _levelText = null;

    [SerializeField]
    private PostProcessVolume _postProcessVolume = null;
    private ColorGrading _colorGrading;

    [SerializeField]
    private List<Collectible> _gems = null;

    private int _gemIndex;
    private int _levelScore;
    private long _currentScore;
    private long _finalScore;
    private int _gemCount;
    private bool _resetGame = false;
    private GameObject _currentGem;

    private BackgroundMusic _backgroundMusic;
    private AudioSource _soundEffects;
    private AudioSource _mineEffects;

    // Arcade and hardcore specific
    private bool _isArcade;
    private bool _isHardcore;
    private float _timer;
    private float _timerIncrement;
    private int _continueCost = 50;
    private bool _hardcoreTimerLock;

    [SerializeField]
    private TextMeshProUGUI _timerValue = null;

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
    private GameObject _gameOverObject = null;
    [SerializeField]
    private bool _counterComplete;
    [SerializeField]
    private Button _retry;

    // Confirm Quit
    private bool _confirmQuit = false;
    [SerializeField]
    private GameObject ConfirmQuitDialog = null;

    [SerializeField]
    private List<GameObject> _powerUps = null;
    private bool _powerupAvailable;

    [SerializeField]
    private GameObject Joysticks = null;

    [SerializeField]
    private GameObject _continueGamePanel = null;
    private Continue _continueGame;

    public int Level { get { return _level; } set { _level = value; } }
    public int LevelScore { get { return _levelScore; } set { _levelScore = value; } }
    public float TimerIncrement { get { return _timerIncrement; } set { _timerIncrement = value; } }
    public long CurrentGemScore { get { return _currentScore; } set { _currentScore = value; } }
    public long FinalScore { get { return _finalScore; } set { _finalScore = value; } }
    public bool HardCoreTimerLock { get { return _hardcoreTimerLock; } set { _hardcoreTimerLock = value; } }
    public bool ConfirmQuit { get { return _confirmQuit; } set { _confirmQuit = value; } }
    public int ContinueCost { get { return _continueCost; } set { _continueCost = value; } }

    #endregion

    /// <summary>
    /// Initialization of the game.
    /// </summary>
    void Init()
    {
        // Set the application target framerate and
        // remove multitouch capabilities.
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;

        // Check what platform the player is using, and allow the use of
        // Joysticks.
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Joysticks.SetActive(true);
        }

        // General game settings.
        _gemIndex = 0;
        _newGame = true;
        _colorGrading = null;
        _powerupAvailable = true;

        // Get the settings for post processing
        if (_postProcessVolume != null)
        {
            _postProcessVolume.profile.TryGetSettings(out _colorGrading);
        }

        //
        // Game Objects
        //

        // Player
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        // Setup Audio
        _backgroundMusic = GameObject.Find("Background Music").GetComponent<BackgroundMusic>();
        _backgroundMusic.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume", .5f);
        _soundEffects = GameObject.Find("SoundEffects").GetComponent<AudioSource>();
        _mineEffects = GameObject.Find("MineEffects").GetComponent<AudioSource>();
        _soundEffects.volume = PlayerPrefs.GetFloat("SoundVolume", .5f);
        _mineEffects.volume = PlayerPrefs.GetFloat("SoundVolume", .5f);

        // Continue access
        _continueGame = _continueGamePanel.GetComponent<Continue>();

        // Create a new mine list
        if (_mines == null)
        {
            _mines = new List<Mine>();
        }

        // Initial Arcade Game Settings
        if (SceneManager.GetActiveScene().name == "Arcade")
        {
            TimerIncrement = 1f;

            _counterComplete = false;
            _isArcade = true;
            _timer = 48f;
        }
        
        // Initial Hardcore Game Settings
        if (SceneManager.GetActiveScene().name == "Hardcore")
        {
            TimerIncrement = 1f;

            _counterComplete = false;
            _isHardcore = true;
            _timer = 85f;
        }

        // Finalize initialization
        PositionBorders();
        StartCoroutine(CreateGameBoard());
        StartCoroutine(StartBackgroundMusic());
    }

    /// <summary>
    /// Restart the Arcade Mode Timer
    /// </summary>
    public void ArcadeRestart()
    {
        _counterComplete = false;
        _timer = 12f;
    }

    /// <summary>
    /// GameManager Loop
    /// </summary>
    private void Update()
    {
        // Countdown the time if the we are in Arcade or Hardcore mode.
        if ((IsArcade() || IsHardcore()) && !_counterComplete)
        {
            if (!_player.IsBlocked)
            {
                _timer -= Time.deltaTime;

                _timerValue.text = ((int)_timer).ToString();
            }

            // If the timer reduces to zero, game over.
            if (_timer <= 0)
            {
                _counterComplete = true;
                _player.PlayerDead();
            }
        }

    }

    /// <summary>
    /// Check the GemCount to see if the player has collected
    /// all of them.
    /// </summary>
    public IEnumerator CheckGemCount()
    {
        _gemCount--;

        // This keeps time from being added to the
        // hardcore timer on every gem.
        if (IsHardcore())
        {
            if (HardCoreTimerLock == false)
            {
                HardCoreTimerLock = true;
            }
            else if (HardCoreTimerLock == true)
            {
                HardCoreTimerLock = false;
                _timer += TimerIncrement;
            }
        }

        // Increment the timer in arcade mode for each gem pickup
        if (IsArcade())
        {
            _timer += TimerIncrement;
        }

        // If the gem count  is less than zero,
        // add to the timer and reset the game.
        if (_gemCount <= 0)
        {
            _resetGame = true;

            if (IsArcade() || IsHardcore())
            {
                _timer += 5f;
            }

            yield return StartCoroutine(CreateGameBoard());
        }

        yield break;

    }

    /// <summary>
    /// Get a random position.
    /// </summary>
    /// <returns>Returns a random position on the gameboard.</returns>
    private Vector3 GetRandomPosition()
    {
        // Get a random position in the gameboard
        float playerDistance = 0;
        bool locationUnsafe = true;
        Vector3 position = Vector3.zero;

        // Loop until a safe location is found
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

        // return the position
        return position;
    }

    /// <summary>
    /// Create the gameboard.
    /// </summary>
    public IEnumerator CreateGameBoard()
    {

        // Set the change for obtaining a special item
        var powerUpChance = Random.value;
        var powerUpValue = Random.Range(0, 3);

        // if a new game or a game reset...
        if (_resetGame || _newGame)
        {
            _resetGame = false;

            // Set a new game level
            SetCurrentLevel();

            // Update the game board UI
            _levelText.text = _level.ToString();

            // Reset Score Multiplier on new level
            _player.ScoreMultiplier(0);

            // If the level is bad, throw error and kick back to menu
            if (Level == 0)
            {
                Debug.LogError("Level cannot be 0");
                SceneManager.LoadScene("Menu");
                yield break;
            }

            // Set the mines
            _mines = FindObjectsOfType<Mine>().ToList();

            // Load mines into list
            foreach (var mine in _mines)
            {
                mine.Explode(true);
            }

            // Block the player from moving when relocating
            if (!_newGame)
            {
                _player.Block();

                _player.ResetPlayerLocation();
            }

            // Make sure newgame is false
            _newGame = false;


            // Check if gem list is empty
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

            // Update the color
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

            // Get a powerup
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

            // Update the score value for each gem pickup
            LevelScore = _gems[_gemIndex].ScoreValue;

            // Update the gem count
            _gemCount = Level;
        }
    }

    /// <summary>
    /// Reset the ability to spawn a powerup
    /// </summary>
    public void ResetPowerUp()
    {
        _powerupAvailable = true;
    }

    /// <summary>
    /// Start background music
    /// </summary>
    IEnumerator StartBackgroundMusic()
    {
        yield return new WaitForSeconds(1f);

        if (Level % 10 == 0)
        {
            _backgroundMusic.StartNewSong();
        }
    }

    /// <summary>
    /// Update Color Grading for gems.
    /// </summary>
    /// <param name="newTempValue"></param>
    /// <param name="newTintValue"></param>
    private void UpdateColorGrading(float newTempValue, float newTintValue)
    {
        _colorGrading.temperature.value = newTempValue;
        _colorGrading.tint.value = newTintValue;
    }

    /// <summary>
    /// Move the borders of the game board to the edges of the screen
    /// </summary>
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

    /// <summary>
    /// Get the controller type the player is using
    /// </summary>
    /// <returns></returns>
    public PlayerControllerType GetControllerType()
    {
        return PlayerControllerType.STICK; // fix with options menu [CONTROLLER]
    }

    /// <summary>
    /// Set the current level of the game.
    /// </summary>
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

    /// <summary>
    /// Return the highest standard mode level
    /// </summary>
    public int GetCurrentNormalLevel()
    {
        return PlayerPrefs.GetInt("HighestLevel", 1);
    }

    /// <summary>
    /// Return the highest arcade mode level
    /// </summary>
    public int GetCurrentArcadeLevel()
    {
        return PlayerPrefs.GetInt("ArcadeLevel", 1);
    }

    /// <summary>
    /// Return the highest score
    /// </summary>
    public long GetHighScore()
    {
        long highScore = long.Parse(PlayerPrefs.GetString("HighScore", "0"));

        return highScore;
    }

    /// <summary>
    /// Check if arcade mode
    /// </summary>
    public bool IsArcade()
    {
        return _isArcade;
    }

    /// <summary>
    /// Check of hardcore mode.
    /// </summary>
    /// <returns></returns>
    public bool IsHardcore()
    {
        return _isHardcore;
    }

    /// <summary>
    /// Continue the game
    /// </summary>
    public void ContinueGame()
    {
        // Check if hardcore mode because hardcore mode is not allowed
        // to continue. Otherwise, allow continue dialog to show
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
        else
        {
            GameOver();
        }

        //GameFinished = false;
        //// Reload player
        //_player.Init();

        //_player.transform.position = new Vector3(0f, 0f, 0f);
    }

    /// <summary>
    /// End the game
    /// </summary>
    public void GameOver()
    {
        if ((IsArcade() || IsHardcore()) && !_counterComplete)
        {
            _counterComplete = true;
        }

        if (!IsHardcore())
        {
            //Instantiate(_continueGamePanel, Vector3.zero, Quaternion.identity);
            _continueGame.ToggleContinue();
        }
        else
        {
            //Instantiate(_gameOverObject, Vector3.zero, Quaternion.identity);
            _gameOverObject.SetActive(true);
        }
    }

    /// <summary>
    /// Get the cost of a continue
    /// </summary>
    public int GetContinueCost()
    {
        return _continueCost;
    }

    /// <summary>
    /// Show the confirm exit dialog
    /// </summary>
    public void ToggleConfirmQuit()
    {
        GameManager.Instance.ConfirmQuit = !GameManager.Instance.ConfirmQuit;
        ConfirmQuitDialog.SetActive(!ConfirmQuitDialog.activeSelf);

        if (GameManager.Instance.ConfirmQuit)
        {
            ConfirmQuitDialog.GetComponent<ConfirmExit>().NoButton.Select();
        }
    }

    /// <summary>
    /// Return of the coninue panel is active.
    /// </summary>
    public bool IsContinueActive()
    {
        return _continueGamePanel.activeSelf;
    }

    /// <summary>
    /// Reset the player.
    /// </summary>
    public void PlayerReset()
    {
        _player.gameObject.SetActive(true);
        _player.Init();
    }

    /// <summary>
    /// Game over.
    /// </summary>
    public void ToggleGameOver()
    {
        _gameOverObject.SetActive(true);
    }
}
