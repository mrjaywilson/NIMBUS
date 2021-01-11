using UnityEngine;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles power ups.
/// 
/// Currently Deprecated.
/// 
/// </summary>
public class PowerUps : MonoBehaviour
{
    [SerializeField]
    private PowerUp? _powerUp = null;
    private Player _player;
    private bool _invulnerable;

    /// <summary>
    /// Handles the collosion of anything with power up items.
    /// </summary>
    /// <param name="other"></param>
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

    /// <summary>
    /// Powerup update loop.
    /// </summary>
    private void Update()
    {
        if (_player != null && _invulnerable)
        {

        }
    }

    /// <summary>
    /// Power up types.
    /// </summary>
    private enum PowerUp
    {
        Invulnerability,
        ExtraLife,
        BonusPoints
    }
}
