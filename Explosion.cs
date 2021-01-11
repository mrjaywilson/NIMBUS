using UnityEngine;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles mine explosions.
/// 
/// </summary>
public class Explosion : MonoBehaviour
{
    /// <summary>
    /// Handles what happens when an explosion is instantiated.
    /// </summary>
    void Start()
    {
        Destroy(gameObject, 1f);
    }
}
