using UnityEngine;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles sound effects for entire game.
/// 
/// </summary>
public class SoundEffects : MonoBehaviour
{

    [SerializeField]
    private AudioClip _collectGem = null;
    [SerializeField]
    private AudioClip _collectPowerUp = null;
    private AudioSource _audioSource = null;

    /// <summary>
    /// Method called after instantiation and before the furst update loop frame
    /// </summary>
    private void Start()
    {

        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource component not available on SoundEffects.");
        }

    }

    /// <summary>
    /// Method to handle the sound for gems.
    /// </summary>
    public void PlayGemSound()
    {
        if (_collectGem != null)
        {
            if (_audioSource.clip != _collectGem)
            {
                _audioSource.clip = _collectGem;
            }

            _audioSource.Play();
        }
    }

    /// <summary>
    /// Method to handle the sound for pick up items.
    /// </summary>
    public void PlayPowerUpSound()
    {
        if (_collectPowerUp != null)
        {
            if (_audioSource.clip != _collectPowerUp)
            {
                _audioSource.clip = _collectPowerUp;
            }

            _audioSource.Play();
        }
    }

}
