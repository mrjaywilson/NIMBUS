using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
///
/// Author:         Jay Wilson
/// Description:    Handles the play menu.
/// 
/// Currently deprecated and no longer in use. Code condenced into MenuManager.
/// 
/// </summary>
public class PlayManager : MonoBehaviour
{

    // UI Stuff
    [SerializeField]
    private TextMeshProUGUI _highScore = null;
    [SerializeField]
    private TextMeshProUGUI _standardLevel = null;
    [SerializeField]
    private TextMeshProUGUI _arcadeLevel = null;
    [SerializeField]
    private TextMeshProUGUI _gemsCollected = null;
    [SerializeField]
    private TextMeshProUGUI _minesHit = null;

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

    public void PlayNormalGame()
    {
        SceneManager.LoadScene("Standard");
    }
    public void PlayArcadeGame()
    {
        SceneManager.LoadScene("Arcade");
    }
}
