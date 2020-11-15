using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    private enum GameMode
    {
        NORMAL,
        ARCADE,
        HARDCORE,
        NONE
    }

    [SerializeField]
    private Button _startButton;
    [SerializeField]
    private Button _normalButton;
    [SerializeField]
    private Button _playButton;

    [SerializeField]
    private GameObject _startNormal;
    [SerializeField]
    private GameObject _startArcade;
    [SerializeField]
    private GameObject _startHardcore;

    [SerializeField]
    private TextMeshProUGUI _modeDescription;

    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject playMenu;
    [SerializeField]
    private GameObject optionsMenu;

    // PlayMenu
    [SerializeField]
    private TextMeshProUGUI _highScore;
    [SerializeField]
    private TextMeshProUGUI _standardLevel;
    [SerializeField]
    private TextMeshProUGUI _arcadeLevel;
    [SerializeField]
    private TextMeshProUGUI _hardcoreLevel;
    [SerializeField]
    private TextMeshProUGUI _gemsCollected;
    [SerializeField]
    private TextMeshProUGUI _minesHit;

    private GameMode _modeSelected;

    void Start()
    {
        Init();
    }

    void Init()
    {
        if (_startButton != null && mainMenu.activeSelf == true)
        {
            _startButton.Select();
        }

        if (_normalButton != null && playMenu.activeSelf == true)
        {
            NormalSelect();
            _normalButton.Select();
            _modeSelected = GameMode.NORMAL;

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

    public void StartGame()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(true);

        Init();
    }

    public void Options()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);

        Init();
    }

    public void OptionsBack()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void PlayMenuBack()
    {
        playMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

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

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void NormalMode()
    {
        SceneManager.LoadScene("Standard");
    }

    public void ArcadeMode()
    {
        SceneManager.LoadScene("Arcade");
    }

    public void HardcoreMode()
    {
        SceneManager.LoadScene("Hardcore");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void NormalSelect()
    {
        _modeDescription.color = new Color32(103, 197, 110, 255);
        _modeDescription.text = "For the casual player.\n\n3 Lives To Start\n+1 Life Per 50,000 Points";
        _modeSelected = GameMode.NORMAL;
    }
    public void ArcadeSelect()
    {
        _modeDescription.color = new Color32(103, 197, 110, 255);
        _modeDescription.text = "For players that want a challenge.\n\n3 Lives To Start\n+1 Life Per 50,000 Points\n\nCountdown Timer\n+8 Seconds Per Level\n+1 Second Per Crystal";
        _modeSelected = GameMode.ARCADE;
    }
    public void HardcoreSelect()
    {
        _modeDescription.color = new Color32(255, 42, 0, 255);
        _modeDescription.text = "For players that like to bleed tears.\n\n1 Life\nNO Extra Lives\nDeath Means Game Over\nNo Continues\n\nCountdown Timer\n+5 Seconds Per Level\n+1 Second Every Other Crystal\n\nModified Abilities\nBomb destroys 25 % More\nShockwave takes longer to fill";
        _modeSelected = GameMode.HARDCORE;
    }
}
