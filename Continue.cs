using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Continue : MonoBehaviour
{

    private float _counter;
    private bool _paused;

    [SerializeField]
    private TextMeshProUGUI _counterText;
    [SerializeField]
    private Button _yesButton;
    [SerializeField]
    private TextMeshProUGUI _yesButtonText;
    [SerializeField]
    private GameObject _notEnoughGems;

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

    // Update is called once per frame
    void Update()
    {
        _counter -= Time.deltaTime;

        _counterText.text = ((int)_counter).ToString();

        if (_counter <= 0 && !_paused)
        {
            _paused = true;
            EndGame();
        }
    }

    public void ContinueGame()
    {
        if (PlayerPrefs.GetInt("GemsCollected", 0) > GameManager.Instance.GetContinueCost())
        {
            Destroy(gameObject, .5f);
            GameManager.Instance.ContinueGame();
        }
        else
        {
            _notEnoughGems.SetActive(true);
        }
    }

    public void EndGame()
    {
        // load game over scene, but for now, load main menu
        SceneManager.LoadScene("Menu");

        //Destroy(gameObject, .5f);
        //GameManager.Instance.GameOver();
    }
}
