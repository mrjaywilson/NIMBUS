using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles what background music.
/// 
/// </summary>
public class BackgroundMusic : MonoBehaviour
{
    AudioSource _backgroundMusic;
    [SerializeField]
    List<AudioClip> _musics = null;
    [SerializeField]
    AudioClip _creditsClip = null;

    /// <summary>
    /// Method called just before the first frame that the object is instantiated.
    /// </summary>
    void Start()
    {

        // Ensure that the background music loads only for these scenes, and no others
        if (SceneManager.GetActiveScene().name == "Menu" || 
            SceneManager.GetActiveScene().name == "Arcade" || 
            SceneManager.GetActiveScene().name == "Standard" ||
            SceneManager.GetActiveScene().name == "Hardcore" ||
            SceneManager.GetActiveScene().name == "Credits")
        {
            Init();
            StartNewSong();
        }
    }

    /// <summary>
    /// Object initialization.
    /// </summary>
    private void Init()
    {
        _backgroundMusic = GetComponent<AudioSource>();

        if (_backgroundMusic == null)
        {
            Debug.LogError("BackgroundMusic::AudioSource is null.");
        }
    }

    /// <summary>
    /// Start Background Music. Allows for the background music to be different for each scene
    /// in the future.
    /// </summary>
    public void StartNewSong()
    {
        Init();
        _backgroundMusic.Stop();

        if (_musics != null)
        {

            var _clip = GetRandomSong();

            switch (SceneManager.GetActiveScene().name)
            {
                case "Standard":
                    _backgroundMusic.clip = _clip;
                    break;
                case "Arcade":
                    _backgroundMusic.clip = _clip;
                    break;
                case "Hardcore":
                    _backgroundMusic.clip = _clip;
                    break;
                case "Menu":
                    _backgroundMusic.clip = _clip;
                    break;
                case "Credits":
                    if (_creditsClip != null)
                    {
                        _backgroundMusic.clip = _creditsClip;
                        _backgroundMusic.Play();
                    }
                    break;
            }

            _backgroundMusic.Play();

        }
    }

    /// <summary>
    /// Used to get a random song from the list.
    /// </summary>
    /// <returns></returns>
    private AudioClip GetRandomSong()
    {
        return (_musics[Random.Range(0, _musics.Count)]);
    }
}
