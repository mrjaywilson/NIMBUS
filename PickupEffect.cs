using UnityEngine;

public class PickupEffect : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 1f);
    }
}
