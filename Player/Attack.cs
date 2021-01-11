using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles what happens when user attack collides with mines.
/// 
/// </summary>
public class Attack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Mine")
        {
            other.GetComponent<Mine>().Explode();
        }
    }

}
