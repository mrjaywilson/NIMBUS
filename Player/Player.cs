using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using MilkShake;

public class Player : MonoBehaviour
{

    // Player Specific
    private float _playerBloat = 0.025f; // Not used, supposed to increase size of player
    private float _speed = 2f;
    private float _modSpeed;
    private bool _playerDead;

    // Player Controls
    [SerializeField]
    private GameObject _mobileStick;
    [SerializeField]
    private GameObject _mobileTilt;

    private PlayerControllerType _controllerType;
    private float _horizontal;
    private float _vertical;

    // Specials
    private int _bonusLifeScoreInterval;
    private bool _invulnerable;
    private bool _bombActive;
    private int _playerAttackCount;

    [SerializeField]
    private ParticleSystem _attackparticles;
    [SerializeField]
    private GameObject _attackObject;

    [SerializeField]
    private Slider _playerAttackSlider;
    [SerializeField]
    private Slider _playerBombSlider;

    [SerializeField]
    private GameObject _attackReady;
    [SerializeField]
    private GameObject _bombReady;

    private ParticleSystem _attackReadyEffect;
    private ParticleSystem _bombReadyEffect;

    // General
    private bool _playerBlocked;
    SpriteRenderer _spriteRenderer;
    private GameObject _particles;

    // Score and Lives
    [SerializeField]
    private long _score;
    [SerializeField]
    private int _lives;
    [SerializeField]
    private TextMeshProUGUI _scoreText;
    [SerializeField]
    private TextMeshProUGUI _livesText;

    private int _scoreModifier;
    private int _currentScoreModifier;

    //[SerializeField]
    //private CameraShake cameraShake;
    [SerializeField]
    private ShakePreset _shakePreset;

    [SerializeField]
    private Shaker _myShaker;

    // private bool _resetPlayer;

    public long Score { get { return _score; } set { _score = value; } }
    public int Lives { get { return _lives; } set { _lives = value; } }


    public void Init()
    {
        _playerAttackSlider.value = 0;
        _playerBombSlider.value = 0;

        if (_attackReadyEffect != null)
        {
            _attackReadyEffect.Play();
        }

        if (_bombReadyEffect != null)
        {
            _bombReadyEffect.Play();
        }

        if (_playerDead)
        {
            // Unlock player controls
            Block();
            _playerDead = false;
        }

        _invulnerable = false;
        _bombActive = true;

        _playerBlocked = false;

        Lives = 3;
        _scoreModifier = 0;
        _currentScoreModifier = 0;

        transform.position = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerDead = false;
        _controllerType = GameManager.Instance.GetControllerType();

        // Attack Setup
        _attackReadyEffect = _attackReady.GetComponent<ParticleSystem>();
        _bombReadyEffect = _bombReady.GetComponent<ParticleSystem>();

        Init();

        Score = 0;

        _particles = null;

        _bonusLifeScoreInterval = 50000;

        if (_controllerType == PlayerControllerType.STICK)
        {
            _mobileStick.SetActive(true);
        }
        else if (_controllerType == PlayerControllerType.TILT)
        {
            _mobileTilt.SetActive(true);
        }

        // _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_spriteRenderer == null)
        {
            Debug.LogError("Player::SpriteRenderer is null!");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!_playerBlocked)
        {
            MovePlayer();

            if (Input.GetButtonDown("Fire2") && _bombActive)
            {
                BombPowerUp();
            }

            if (Input.GetButtonDown("Fire3"))
            {
                PlayerAttack();
            }
        }

        _scoreText.text = Score.ToString("N0");
        _livesText.text = Lives.ToString();

        if (_particles == null && _invulnerable)
        {
            _particles = GameObject.Find("PowerUp Ring 2");

            if (_particles != null)
            {
                _particles.transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            if (_particles != null && !_invulnerable)
            {
                Destroy(_particles);
            }
        }

    }

    private IEnumerator ScoreCount()
    {


        if (_currentScoreModifier != 0)
        {
            _scoreModifier = GameManager.Instance.LevelScore * _currentScoreModifier;
        }
        else
        {
            _scoreModifier = 0;
        }

        // adjust amount
        GameManager.Instance.CurrentGemScore = (GameManager.Instance.LevelScore * GameManager.Instance.Level) + _scoreModifier;
        GameManager.Instance.FinalScore += GameManager.Instance.CurrentGemScore;

        for (int i = 1; i <= GameManager.Instance.CurrentGemScore / 2; i++)
        {

            Score += 2;
            _scoreText.text = Score.ToString("N0");

            if (Score >= _bonusLifeScoreInterval)
            {
                _bonusLifeScoreInterval += 50000;
                Lives++;

                Debug.Log("Bonus life interval: " + _bonusLifeScoreInterval);
            }

            yield return new WaitForSeconds(0f);
        }

        SetGlobalScore();

        yield break;
    }

    public IEnumerator AddScore()
    {
        StartCoroutine(ScoreCount());

        _scoreText.text = Score.ToString("N0");

        StartCoroutine(DiamondCheck());
        yield break;
    }

    IEnumerator DiamondCheck()
    {
        // yield return new WaitForSeconds(.6f);
        yield return StartCoroutine(GameManager.Instance.CheckGemCount());
    }

    public void RemoveLife()
    {
        if (!_invulnerable)
        {
            Lives--;
        }
        else
        {
            if (_particles == null)
            {
                _particles = GameObject.Find("PowerUp Ring 2");

                if (_particles != null)
                {
                    _particles.transform.position = new Vector3(0f, 0f, 0f);
                }
            }
        }

        _livesText.text = Lives.ToString();

        if (Lives <= 0)
        {
            PlayerDead();
        }
    }

    public void IncreaseSize()
    {
        //transform.localScale += new Vector3(_playerBloat, _playerBloat, _playerBloat);
        //_spriteRenderer.transform.localScale += new Vector3(_playerBloat, _playerBloat, _playerBloat);
        // transform.GetChild(0).gameObject.transform.position = Vector3.zero;
    }

    private void MovePlayer()
    {
#if UNITY_ANDROID

        if (_controllerType == PlayerControllerType.STICK || _controllerType == PlayerControllerType.TILT)
        {
            _horizontal = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            _vertical = CrossPlatformInputManager.GetAxisRaw("Vertical");
        }
        else
        {
            _horizontal = Input.GetAxisRaw("Horizontal");
            _vertical = Input.GetAxisRaw("Vertical");
        }

#else
        var _horizontal = Input.GetAxisRaw("Horizontal");
        var _vertical = Input.GetAxisRaw("Vertical");
#endif


        if (Input.GetButton("Fire1"))
        {
#if UNITY_ANDROID
            if (_controllerType == PlayerControllerType.STICK)
            {
                _modSpeed = 3.5f;
            }
            else
            {
                _modSpeed = 0f;
            }
#else
                _modSpeed = 0f;
#endif
        }
        else
        {
#if UNITY_ANDROID
            if (_controllerType == PlayerControllerType.STICK)
            {
                _modSpeed = 0f;
            }
            else
            {
                _modSpeed = 3.5f;
            }
#else
            _modSpeed = 3.5f;
#endif
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }

        Vector3 move = new Vector3(_horizontal, _vertical, 0f) * (_speed + _modSpeed) * Time.deltaTime;

        transform.Translate(move);
    }

    public void SlowPlayer()
    {

    }

    public void PlayerDead()
    {
        _scoreText.text = GameManager.Instance.FinalScore.ToString("N0");

        Block(); // Block player controls
        _playerDead = true;

        GameManager.Instance.GameOver();

        gameObject.SetActive(false);

        //GameManager.Instance.GameFinished = true;
        //// Don't destroy. Need to be able to add lives and continue as needed, but in the mean time, hide and player_block
        //Instantiate(_continueGame, new Vector3(0f, 0f, 0f), Quaternion.identity);

        //gameObject.SetActive(false);
    }

    public void Block()
    {

        GetComponent<Collider2D>().enabled = !GetComponent<Collider2D>().enabled;
        _playerBlocked = !_playerBlocked;
    }

    public void BombPowerUp()
    {
            var currentMines = FindObjectsOfType<Mine>();

            _myShaker.Shake(_shakePreset);
            // StartCoroutine(cameraShake.Shake(.15f, .2f));

            if (currentMines.Length > 1)
            {
                for (int i = 0; i < currentMines.Length - 1; i += 2)
                {
                    currentMines[i].Explode(true);
                }
            }

            _playerBombSlider.value = 1;
            _bombReadyEffect.Stop();
            _bombActive = false;
    }

    public IEnumerator RelocatePlayer()
    {
        while (transform.position != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, 5f * Time.deltaTime);
            yield return new WaitForSeconds(.0001f);
        }
    }

    public void Invulnerable()
    {
        _invulnerable = !_invulnerable;
    }

    public void AddLife()
    {
        ++Lives;
    }

    public void ScoreMultiplier(int amount)
    {
        bool multiplierStatus = false;
        _currentScoreModifier = amount;

        var multiplier = GameObject.FindGameObjectWithTag("Multiplier");

        if (multiplier != null)
        {
            multiplierStatus = multiplier.GetComponent<Multiplier>().IsActive;
        }

        if (multiplier != null && amount == 0 && multiplierStatus)
        {
            Destroy(multiplier.gameObject);
        }
    }

    private void SetGlobalScore()
    {
        //PlayerPrefs.SetString("PlayerScore", _score.ToString());

        long highScore = long.Parse(PlayerPrefs.GetString("HighScore", "0"));

        if (Score > highScore)
        {
            PlayerPrefs.SetString("HighScore", _score.ToString());
        }
    }

    public void RemoveFromAttack()
    {
        if (_playerAttackSlider.value > 0)
        {
            _playerAttackSlider.value -= 1;

            if (_playerAttackSlider.value == 0)
            {
                _attackReadyEffect.Play();
            }
        }
    }

    public void PlayerAttack()
    {
            if (_playerAttackSlider.value == 0)
            {
                _playerAttackSlider.value = 35;
                _attackparticles.Play();

                _attackReadyEffect.Stop();

                StartCoroutine(AttackCollider());
            }
    }

    private IEnumerator AttackCollider()
    {
        _attackObject.GetComponent<Collider2D>().enabled = true;

        while (_attackObject.transform.localScale.x < 1)
        {
            _attackObject.transform.localScale += new Vector3(.1f, .1f, .1f);
            yield return new WaitForSeconds(.1f);
        }

        _attackObject.transform.localScale = new Vector3(.1f, .1f, .1f);

        _attackObject.GetComponent<Collider2D>().enabled = false;
    }
}
