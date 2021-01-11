using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles the ability to continue the game.
/// 
/// </summary>
public class Continue : MonoBehaviour
{

    private float _counter;
    private bool _paused;
    private bool _counterStart = false;

    [SerializeField]
    private TextMeshProUGUI _counterText = null;
    [SerializeField]
    private Button _yesButton = null;
    [SerializeField]
    private TextMeshProUGUI _yesButtonText = null;
    [SerializeField]
    private GameObject _notEnoughGems = null;

    /// <summary>
    /// Initialization method called before the first frame after instantiation
    /// </summary>
    void Start()
    {
        _paused = false;

        if (_yesButtonText != null)
        {
            _yesButtonText.text = $"YES\n({GameManager.Instance.GetContinueCost()})";
        }

        if (_yesButton != null)
        {
            _yesButton.Select();
        }

        _counter = 11;
    }

    /// <summary>
    /// Continue Game Loop
    /// </summary>
    void Update()
    {
        if (_counterStart)
        {
            _counter -= Time.deltaTime;

            _counterText.text = ((int)_counter).ToString();

            if (_counter <= 0 && !_paused)
            {
                _paused = true;
                EndGame();
            }
        }
    }

    /// <summary>
    /// Method for handling continuing the game.
    /// </summary>
    public void ContinueGame()
    {
        if (PlayerPrefs.GetInt("GemsCollected", 0) > GameManager.Instance.GetContinueCost())
        {
            //GameManager.Instance.ContinueGame();

            var gems = PlayerPrefs.GetInt("GemsCollected", 0);

            if (gems > GameManager.Instance.ContinueCost)
            {
                gems -= GameManager.Instance.ContinueCost;
                PlayerPrefs.SetInt("GemsCollected", gems);
                GameManager.Instance.ContinueCost += 75;

                if (GameManager.Instance.IsArcade())
                {
                    GameManager.Instance.ArcadeRestart();
                }

                GameManager.Instance.PlayerReset();
            }

            ToggleContinue();
        }
        else
        {
            _notEnoughGems.SetActive(true);
        }
    }

    /// <summary>
    /// Method to end the game if user chooses to end it.
    /// </summary>
    public void EndGame()
    {
        GameManager.Instance.ToggleGameOver();
        ToggleContinue();
    }

    /// <summary>
    /// Handle toggling the panel on and off.
    /// </summary>
    public void ToggleContinue()
    {
        _counter = 11;
        _counterStart = !_counterStart;

        if (_counterStart)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
