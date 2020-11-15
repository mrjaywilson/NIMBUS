using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    [SerializeField]
    private PowerUp _powerUp;

    private Player _player;

    // Invulnerability
    private bool _invulnerable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _player = other.GetComponent<Player>();


            switch (_powerUp)
            {
                case PowerUp.Invulnerability:
                    _invulnerable = true;
                    break;
                case PowerUp.ExtraLife:
                    Debug.Log(gameObject.name);
                    break;
                case PowerUp.BonusPoints:
                    Debug.Log(gameObject.name);
                    break;
            }
        }
    }

    private void Update()
    {
        if (_player != null && _invulnerable)
        {

        }
    }

    private enum PowerUp
    {
        Invulnerability,
        ExtraLife,
        BonusPoints
    }
}
