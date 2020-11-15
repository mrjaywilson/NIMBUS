using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multiplier : MonoBehaviour
{
    [SerializeField]
    private GameObject _pickupParticle;
    [SerializeField]
    private int _multiplierAmount;

    private bool _isActive;

    public bool IsActive { get { return _isActive; } set { _isActive = value; } }

    private GameObject _powerupBar;

    private void Start()
    {
        IsActive = false;
        _powerupBar = GameObject.FindGameObjectWithTag("PowerUp Bar");

        if (_powerupBar == null)
        {
            Debug.LogError("Multiplier::PowerUpBar is null.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            IsActive = true;
            other.GetComponent<Player>().ScoreMultiplier(_multiplierAmount);

            GameManager.Instance.ResetPowerUp();

            transform.position = _powerupBar.transform.position;
            GameObject.Find("PowerUp Ring 2").transform.position = _powerupBar.transform.position;

            //Destroy(gameObject);
        }
    }
}
