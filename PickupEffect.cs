using UnityEngine;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles gems effects.
/// 
/// </summary>
public class PickupEffect : MonoBehaviour
{
    /// <summary>
    /// Method called after instantiation and before the furst update loop frame
    /// </summary>
    void Start()
    {
        Destroy(gameObject, 1f);
    }
}
