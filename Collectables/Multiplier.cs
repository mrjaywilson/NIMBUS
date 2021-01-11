using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author:         Jay Wilson
/// Description:    Handle Invulnerability Power Up
/// </summary>
public class Multiplier : MonoBehaviour
{
    [SerializeField]
    private GameObject _pickupParticle;
    [SerializeField]
    private int _multiplierAmount = 0;

    private SoundEffects _soundEffect;
    private GameObject _powerupBar;
    private bool _isActive;

    public bool IsActive { get { return _isActive; } set { _isActive = value; } }

    /// <summary>
    /// Initialization Method
    /// </summary>
    private void Start()
    {
        IsActive = false;
        _powerupBar = GameObject.FindGameObjectWithTag("PowerUp Bar");

        _soundEffect = GameObject.FindGameObjectWithTag("SoundEffects").GetComponent<SoundEffects>();

        //
        // Error Handling
        //

        if (_soundEffect == null)
        {
            Debug.LogError("Diamond::SoundEffects is null!");
        }

        if (_powerupBar == null)
        {
            Debug.LogError("Multiplier::PowerUpBar is null.");
        }
    }

    /// <summary>
    /// Trigger Events for collision
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            IsActive = true;
            other.GetComponent<Player>().ScoreMultiplier(_multiplierAmount);

            _soundEffect.PlayPowerUpSound();

            GameManager.Instance.ResetPowerUp();

            transform.position = _powerupBar.transform.position;
            GameObject.Find("PowerUp Ring 2").transform.position = _powerupBar.transform.position;
        }
    }
}
