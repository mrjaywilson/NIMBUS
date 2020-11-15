using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLife : MonoBehaviour
{
    [SerializeField]
    private GameObject _pickupParticle;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Player>().AddLife();

            if (_pickupParticle != null)
            {
                Instantiate(_pickupParticle, transform.position, Quaternion.identity);
            }

            GameManager.Instance.ResetPowerUp();

            Destroy(gameObject);
        }
    }

}
