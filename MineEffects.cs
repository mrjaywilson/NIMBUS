using UnityEngine;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles the mine effects.
/// 
/// </summary>
public class MineEffects : MonoBehaviour
{
    [SerializeField]
    private AudioClip _mineHit = null;

    private AudioSource _audioSource = null;

    /// <summary>
    /// Method called after instantiation and before the furst update loop frame
    /// </summary>
    void Start()
    {

        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource component not available on Mine Effects.");
        }

    }

    /// <summary>
    /// Method to play a sound.
    /// </summary>
    public void PlayMineHitSound()
    {
        if (_mineHit != null)
        {
            _audioSource.PlayOneShot(_mineHit);
        }
    }
}
