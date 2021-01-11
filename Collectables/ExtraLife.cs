using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author:         Jay Wilson
/// Description:    Handle Extra Life Power Up
/// </summary>
public class ExtraLife : MonoBehaviour
{
    [SerializeField]
    private GameObject _pickupParticle = null;

    private SoundEffects _soundEffect;

    /// <summary>
    /// Initialization Method
    /// </summary>
    private void Start()
    {
        _soundEffect = GameObject.FindGameObjectWithTag("SoundEffects").GetComponent<SoundEffects>();

        if (_soundEffect == null)
        {
            Debug.LogError("Diamond::SoundEffects is null!");
        }
    }

    /// <summary>
    /// Handle trigger event.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Player>().AddLife();

            _soundEffect.PlayPowerUpSound();

            if (_pickupParticle != null)
            {
                Instantiate(_pickupParticle, transform.position, Quaternion.identity);
            }

            GameManager.Instance.ResetPowerUp();

            Destroy(gameObject);
        }
    }

}
