using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayManager : MonoBehaviour
{

    // UI Stuff
    [SerializeField]
    private TextMeshProUGUI _highScore;
    [SerializeField]
    private TextMeshProUGUI _standardLevel;
    [SerializeField]
    private TextMeshProUGUI _arcadeLevel;
    [SerializeField]
    private TextMeshProUGUI _gemsCollected;
    [SerializeField]
    private TextMeshProUGUI _minesHit;

    private Button _startButton;

    // Start is called before the first frame update
    void Start()
    {
        _startButton = GameObject.FindGameObjectWithTag("Button").GetComponent<Button>();

        if (_startButton != null)
        {
            _startButton.Select();
        }

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

    private void Update()
    {
        //if (Input.GetKey(KeyCode.Escape) || Input.GetButtonDown("Fire2"))
        //{
        //    PlayMenuBack();
        //}
    }

    public void PlayNormalGame()
    {
        SceneManager.LoadScene("Standard");
    }
    public void PlayArcadeGame()
    {
        SceneManager.LoadScene("Arcade");
    }
}
