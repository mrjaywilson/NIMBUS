using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using MilkShake;
using TMPro;


/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles what happens when user attack collides with mines.
/// 
/// </summary>
public class Player : MonoBehaviour
{

    private float _speed = 2f;
    private float _modSpeed;
    private bool _playerDead;
    private Rigidbody2D _rb;

    private PlayerControllerType _controllerType;
    private float _horizontal;
    private float _vertical;

    private int _bonusLifeScoreInterval;
    private bool _invulnerable;
    private bool _bombActive;
    private int _playerAttackCount;

    [SerializeField]
    private ParticleSystem _attackparticles = null;
    [SerializeField]
    private GameObject _attackObject = null;

    [SerializeField]
    private Slider _playerAttackSlider = null;
    [SerializeField]
    private Slider _playerBombSlider = null;

    [SerializeField]
    private GameObject _attackReady = null;
    [SerializeField]
    private GameObject _bombReady = null;

    private ParticleSystem _attackReadyEffect;
    private ParticleSystem _bombReadyEffect;

    // General
    private bool _playerBlocked;
    SpriteRenderer _spriteRenderer;
    private GameObject _particles;

    [SerializeField]
    private GameObject _playerTail = null;

    // Score and Lives
    [SerializeField]
    private long _score;
    [SerializeField]
    private int _lives;
    [SerializeField]
    private TextMeshProUGUI _scoreText = null;
    [SerializeField]
    private TextMeshProUGUI _livesText = null;

    private int _scoreModifier;
    private int _currentScoreModifier;

    [SerializeField]
    private ShakePreset _shakePreset = null;

    [SerializeField]
    private Shaker _myShaker = null;

    [SerializeField]
    private Joystick _leftJoystick = null;
    [SerializeField]
    private Joystick _rightJoystick = null;

    private Vector2 _move;
    private bool _resetPlayerLocation;

    public long Score { get { return _score; } set { _score = value; } }
    public int Lives { get { return _lives; } set { _lives = value; } }

    public bool IsBlocked { get { return _playerBlocked; } }

    /// <summary>
    /// Player initialization.
    /// </summary>
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
            // Unlock player controls if the game is restarting.
            Block();
            _playerDead = false;
        }

        _bombActive = true;
        _invulnerable = false;
        _playerBlocked = false;

        if (GameManager.Instance.IsHardcore())
        {
            Lives = 0;
        }
        else
        {
            Lives = 3;
        }
        _scoreModifier = 0;
        _currentScoreModifier = 0;

        transform.position = Vector3.zero;
    }

    /// <summary>
    /// Called when the gamepobject is created and before the first frame.
    /// </summary>
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

        //if (_controllerType == PlayerControllerType.STICK)
        //{
        //    _mobileStick.SetActive(true);
        //}
        //else if (_controllerType == PlayerControllerType.TILT)
        //{
        //    _mobileTilt.SetActive(true);
        //}

        // _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_spriteRenderer == null)
        {
            Debug.LogError("Player::SpriteRenderer is null!");
        }

        _rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Player Game Loop
    /// </summary>
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

        if (_resetPlayerLocation)
        {
            transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, 5f * Time.deltaTime);

            if (transform.position == Vector3.zero)
            {
                GetComponent<Collider2D>().enabled = true;
                _resetPlayerLocation = false;
                Block();
            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            var cont = GameObject.FindGameObjectWithTag("Continue");

            if (cont != null)
            {
                GameObject.FindGameObjectWithTag("Continue").SetActive(false);
                GameManager.Instance.ToggleGameOver();
                return;
            }
            
            // else if at game over, go back to play menu
            if (!GameManager.Instance.ConfirmQuit)
            {
                Block();
                GameManager.Instance.ToggleConfirmQuit();
            }

            // SceneManager.LoadScene("Menu");
        }
    }

    // Removal of score count setup for performance and bug fix
    // This is also important because this feature doesn't work right anyway
    private IEnumerator ScoreUpdate()
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

        // Update data
        Score += GameManager.Instance.CurrentGemScore;
        GameManager.Instance.FinalScore = Score;

        // Update UI
        _scoreText.text = Score.ToString("N0");

        if (!GameManager.Instance.IsHardcore())
        {
            if (Score >= _bonusLifeScoreInterval)
            {
                _bonusLifeScoreInterval += 50000;
                Lives++;
            }
        }

        SetGlobalScore();

        yield break;
    }

    /// <summary>
    /// Add to the game score.
    /// </summary>
    /// <returns></returns>
    public IEnumerator AddScore()
    {
        // StartCoroutine(ScoreCount());
        StartCoroutine(ScoreUpdate());

        _scoreText.text = Score.ToString("N0");

        StartCoroutine(DiamondCheck());
        yield break;
    }

    /// <summary>
    /// Check the diamonds.
    /// </summary>
    /// <returns></returns>
    IEnumerator DiamondCheck()
    {
        // yield return new WaitForSeconds(.6f);
        yield return StartCoroutine(GameManager.Instance.CheckGemCount());
    }

    /// <summary>
    /// Remove a life from the player.
    /// </summary>
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

        if (Lives < 0)
        {
            _livesText.text = "0";
            PlayerDead();
        }

    }

    /// <summary>
    /// Used to increase the player size.
    /// 
    /// Currently deprecated.
    /// </summary>
    public void IncreaseSize()
    {
        //transform.localScale += new Vector3(_playerBloat, _playerBloat, _playerBloat);
        //_spriteRenderer.transform.localScale += new Vector3(_playerBloat, _playerBloat, _playerBloat);
        // transform.GetChild(0).gameObject.transform.position = Vector3.zero;
    }

    /// <summary>
    /// Method to handle moving the player.
    /// </summary>
    private void MovePlayer()
    {
        #region DEVICE_TYPE
#if UNITY_ANDROID

        if (_controllerType == PlayerControllerType.STICK || _controllerType == PlayerControllerType.TILT)
        {
            if (_leftJoystick.Horizontal != 0 && _rightJoystick.Horizontal == 0)
            {
                _horizontal = _leftJoystick.Horizontal;
            }
            else if (_rightJoystick.Horizontal != 0 && _leftJoystick.Horizontal == 0)
            {
                _horizontal = _rightJoystick.Horizontal;
            }
            else
            {
                _horizontal = 0;
            }

            if (_leftJoystick.Vertical != 0 && _rightJoystick.Horizontal == 0)
            {
                _vertical = _leftJoystick.Vertical;
            }
            else if (_rightJoystick.Vertical != 0 && _leftJoystick.Horizontal == 0)
            {
                _vertical = _rightJoystick.Vertical;
            }
            else
            {
                _vertical = 0;
            }

            //_horizontal = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            //_vertical = CrossPlatformInputManager.GetAxisRaw("Vertical");
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
        #endregion

        _move = new Vector2(_horizontal, _vertical);
        var main = _playerTail.GetComponent<ParticleSystem>().main;

        if (_move.sqrMagnitude != 0)
        {
            transform.up = _move;

            if (_horizontal == 0 && _vertical != 0)
            {
                if (_vertical > 0)
                {
                    main.startRotation = -transform.rotation.eulerAngles.magnitude * Mathf.Deg2Rad;
                }
                else if (_vertical < 0)
                {
                    main.startRotation = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
                }
            }
            else
            {
                main.startRotation = -transform.rotation.eulerAngles.magnitude * Mathf.Deg2Rad;
            }
        }
    }

    /// <summary>
    /// Happens last on the update at the end of every frame.
    /// </summary>
    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + (_move * (_speed + _modSpeed) * Time.deltaTime));
    }

    /// <summary>
    /// Handled player's death.
    /// </summary>
    public void PlayerDead()
    {
        _scoreText.text = GameManager.Instance.FinalScore.ToString("N0");

        Block(); // Block player controls
        _playerDead = true;

        GameManager.Instance.GameOver();

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Method to block/unblock player movement.
    /// </summary>
    public void ToggleBlock()
    {
        _playerBlocked = !_playerBlocked;
    }

    /// <summary>
    /// Handles the bomb powerup.
    /// </summary>
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

    /// <summary>
    /// Reset the player location.
    /// </summary>
    public void ResetPlayerLocation()
    {
        GetComponent<Collider2D>().enabled = false;
        _resetPlayerLocation = true;
    }

    /// <summary>
    /// Sets the player to invulnerable.
    /// </summary>
    public void Invulnerable()
    {
        _invulnerable = !_invulnerable;
    }

    /// <summary>
    /// Add an extra life to the user.
    /// </summary>
    public void AddLife()
    {
        if (GameManager.Instance.IsHardcore())
        {
            return;
        }

        ++Lives;
    }

    /// <summary>
    /// Increase the score by the amount provided.
    /// </summary>
    /// <param name="amount">Amount to increase the score.</param>
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

    /// <summary>
    /// Global score.
    /// </summary>
    private void SetGlobalScore()
    {
        //PlayerPrefs.SetString("PlayerScore", _score.ToString());

        long highScore = long.Parse(PlayerPrefs.GetString("HighScore", "0"));

        if (Score > highScore)
        {
            PlayerPrefs.SetString("HighScore", _score.ToString());
        }
    }

    /// <summary>
    /// Handles the counter for the player's attack.
    /// </summary>
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

    /// <summary>
    /// Handles that player's shockwave attack.
    /// </summary>
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

    /// <summary>
    /// Handles the colloder for the player's attack.
    /// </summary>
    /// <returns></returns>
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