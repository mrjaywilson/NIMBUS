using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Descrption:     Handles all of the menus outside of the game. This includes the title, options, and play menus.
/// 
/// </summary>
public class MenuManager : MonoBehaviour
{


    // Determines which game mode the player chose
    private enum GameMode
    {
        NORMAL,
        ARCADE,
        HARDCORE,
        NONE
    }


    // Properties and members
    [SerializeField]
    private Button _startButton = null;
    [SerializeField]
    private Button _normalButton = null;
    [SerializeField]
    private Button _playButton;

    [SerializeField]
    private GameObject _startNormal;
    [SerializeField]
    private GameObject _startArcade;
    [SerializeField]
    private GameObject _startHardcore;

    [SerializeField]
    private TextMeshProUGUI _modeDescription = null;

    [SerializeField]
    private GameObject mainMenu = null;
    [SerializeField]
    private GameObject playMenu = null;
    [SerializeField]
    private GameObject optionsMenu = null;

    // Play Menu
    [SerializeField]
    private TextMeshProUGUI _highScore = null;
    [SerializeField]
    private TextMeshProUGUI _standardLevel = null;
    [SerializeField]
    private TextMeshProUGUI _arcadeLevel = null;
    [SerializeField]
    private TextMeshProUGUI _hardcoreLevel = null;
    [SerializeField]
    private TextMeshProUGUI _gemsCollected = null;
    [SerializeField]
    private TextMeshProUGUI _minesHit = null;

    private GameMode _modeSelected;

    // Options Menu
    [SerializeField]
    private Slider _musicVolume = null;
    [SerializeField]
    private Slider _soundVolume = null;
    [SerializeField]
    private AudioSource _musicSource = null;
    [SerializeField]
    private AudioClip _dingle = null;
    [SerializeField]
    private Toggle _handedness = null;

    /// <summary>
    /// Start method called on object creation
    /// </summary>
    void Start()
    {
        Init();
    }

    /// <summary>
    /// Initialization for the class 
    /// </summary>
    void Init()
    {
        // Get music volume, if not set default to 50%
        _musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", .5f);

        // Select button for the user if they happened to be using a controller / gamepad
        if (_startButton != null && mainMenu.activeSelf == true)
        {
            _startButton.Select();
        }

        if (_normalButton != null && playMenu.activeSelf == true)
        {
            // Select button for gamepads
            NormalSelect();
            _normalButton.Select();
            _modeSelected = GameMode.NORMAL;

            //
            // Get game data and fill the play menu UI
            //

            if (_highScore == null)
            {
                Debug.LogError("PlayManager::HighScore UI is null.");
            }
            else
            {
                var score = long.Parse(PlayerPrefs.GetString("HighScore", "0"));

                _highScore.text = score.ToString("N0");
            }

            if (_standardLevel == null)
            {
                Debug.LogError("PlayManager::HighestLevel UI is null.");
            }
            else
            {
                _standardLevel.text = PlayerPrefs.GetInt("HighestLevel", 1).ToString("N0");
            }

            if (_arcadeLevel == null)
            {
                Debug.LogError("PlayManager::ArcadeLevel UI is null.");
            }
            else
            {
                _arcadeLevel.text = PlayerPrefs.GetInt("ArcadeLevel", 1).ToString("N0");
            }

            if (_hardcoreLevel == null)
            {
                Debug.LogError("PlayManager::ArcadeLevel UI is null.");
            }
            else
            {
                _hardcoreLevel.text = PlayerPrefs.GetInt("HardcoreLevel", 1).ToString("N0");
            }

            if (_gemsCollected == null)
            {
                Debug.LogError("PlayManager::GemsCollected UI is null.");
            }
            else
            {
                _gemsCollected.text = PlayerPrefs.GetInt("GemsCollected", 0).ToString("N0");
            }

            if (_minesHit == null)
            {
                Debug.LogError("PlayManager::MinesHit UI is null.");
            }
            else
            {
                _minesHit.text = PlayerPrefs.GetInt("MinesHit", 0).ToString("N0");
            }

        }
    }

    /// <summary>
    /// MenuManager Game Loop
    /// </summary>
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape) || Input.GetButtonDown("Fire2"))
        {
            if (mainMenu.activeSelf == true)
            {
                Quit();
            }

            if (playMenu.activeSelf == true)
            {
                PlayMenuBack();
            }

            if (optionsMenu.activeSelf == true)
            {
                OptionsBack();
            }
        }
    }

    /// <summary>
    /// Method for opening the game menu.
    /// </summary>
    public void StartGame()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(true);

        Init();
    }

    /// <summary>
    /// Opens the Options Menu
    /// </summary>
    public void Options()
    {
        // Get the music and SFX data
        _musicVolume.value = PlayerPrefs.GetFloat("MusicVolume", .5f);
        _soundVolume.value = PlayerPrefs.GetFloat("SoundVolume", .5f);

        if (PlayerPrefs.GetInt("Handedness", 0) == 1)
        {
            _handedness.isOn = true; // is Right Handed
        }
        else
        {
            _handedness.isOn = false; // is Left Handed
        }

        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);

        Init();
    }

    /// <summary>
    /// Method to reuturn to main menu
    /// </summary>
    public void OptionsBack()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    /// <summary>
    /// Method to reuturn to main menu
    /// </summary>
    public void PlayMenuBack()
    {
        playMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    /// <summary>
    /// Method to start a new game.
    /// </summary>
    public void PlayGame()
    {
        switch(_modeSelected)
        {
            case GameMode.NORMAL:
                NormalMode();
                break;
            case GameMode.ARCADE:
                ArcadeMode();
                break;
            case GameMode.HARDCORE:
                HardcoreMode();
                break;
        }
    }

    /// <summary>
    /// Loads credits scenes.
    /// </summary>
    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    /// <summary>
    /// Loads normal game mode
    /// </summary>
    public void NormalMode()
    {
        SceneManager.LoadScene("Standard");
    }

    /// <summary>
    /// Loads arcade game mode
    /// </summary>
    public void ArcadeMode()
    {
        SceneManager.LoadScene("Arcade");
    }

    /// <summary>
    /// Loads hardcore game mode
    /// </summary>
    public void HardcoreMode()
    {
        SceneManager.LoadScene("Hardcore");
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Load Normal Mode info
    /// </summary>
    public void NormalSelect()
    {
        _modeDescription.color = new Color32(103, 197, 110, 255);
        _modeDescription.text = "For the casual player.\n\n3 Lives To Start\n+1 Life Per 50,000 Points";
        _modeSelected = GameMode.NORMAL;
    }

    /// <summary>
    /// Load Arcade Mode info
    /// </summary>
    public void ArcadeSelect()
    {
        _modeDescription.color = new Color32(103, 197, 110, 255);
        _modeDescription.text = "For players that want a challenge.\n\n3 Lives To Start\n+1 Life Per 50,000 Points\n\nCountdown Timer\n+8 Seconds Per Level\n+1 Second Per Crystal";
        _modeSelected = GameMode.ARCADE;
    }

    /// <summary>
    /// Load Hardcore Mode info
    /// </summary>
    public void HardcoreSelect()
    {
        _modeDescription.color = new Color32(255, 42, 0, 255);
        _modeDescription.text = "For players that like to bleed tears.\n\n1 Life\nNO Extra Lives\nDeath Means Game Over\nNo Continues\n\nCountdown Timer\n+5 Seconds Per Level\n+1 Second Every Other Crystal\n\nModified Abilities\nBomb destroys 25 % More\nShockwave takes longer to fill";
        _modeSelected = GameMode.HARDCORE;
    }

    /// <summary>
    /// Update music volume
    /// </summary>
    public void UpdateMusic()
    {
        _musicSource.volume = _musicVolume.value;
        PlayerPrefs.SetFloat("MusicVolume", _musicVolume.value);
    }

    /// <summary>
    /// Update sound effects volume
    /// </summary>
    public void UpdateSound()
    {
        //_soundSource.volume = _soundVolume.value;
        PlayerPrefs.SetFloat("SoundVolume", _soundVolume.value);
    }

    /// <summary>
    /// Plays a 'ding' sound.
    /// </summary>
    public void PlayDing()
    {
        AudioSource.PlayClipAtPoint(_dingle, Camera.main.transform.position, _soundVolume.value);
    }

    /// <summary>
    /// Allows the user to change the handedness of the on-screen controls setting.
    /// 
    /// Currently deprecated.
    /// </summary>
    public void ChangeHandedness()
    {
        int hand = 0;

        if (_handedness.isOn)
        {
            hand = 1; // Right Handed
            
        }
        else
        {
            hand = 0; // Left Handed
        }


        PlayerPrefs.SetInt("Handedness", hand);

        Debug.Log("Handedness changed to: " + PlayerPrefs.GetInt("Handedness"));
    }
}
