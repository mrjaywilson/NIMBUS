using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    AudioSource _backgroundMusic;
    [SerializeField]
    List<AudioClip> _musics;
    [SerializeField]
    AudioClip _creditsClip;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu" || SceneManager.GetActiveScene().name == "Arcade" || SceneManager.GetActiveScene().name == "Standard")
        {
            Init();
            StartNewSong();
        }
    }

    private void Init()
    {
        _backgroundMusic = GetComponent<AudioSource>();

        if (_backgroundMusic == null)
        {
            Debug.LogError("BackgroundMusic::AudioSource is null.");
        }
    }

    // Background Music

    // Update is called once per frame
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

    private AudioClip GetRandomSong()
    {
        return (_musics[Random.Range(0, _musics.Count)]);
    }
}
