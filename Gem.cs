using UnityEngine;
using TMPro;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles gems.
/// 
/// </summary>
public class Gem : MonoBehaviour
{
    [SerializeField]
    private Collider2D _collider2D;
    private SoundEffects _soundEffect;
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private ParticleSystem _pickupEffect = null;

    [SerializeField]
    private TextMeshProUGUI _score = null;
    [SerializeField]
    private TextMeshProUGUI _time = null;

    /// <summary>
    /// Method called after instantiation and before the furst update loop frame
    /// </summary>
    void Start()
    {
        _collider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _soundEffect = GameObject.FindGameObjectWithTag("SoundEffects").GetComponent<SoundEffects>();

        if (_collider2D == null)
        {
            Debug.LogError("Diamond::Collider2D is null!");
        }

        if (_soundEffect == null)
        {
            Debug.LogError("Diamond::SoundEffects is null!");
        }

        if (_spriteRenderer == null)
        {
            Debug.LogError("Diamond::SpriteRenderer is null!");
        }

    }

    /// <summary>
    /// Method determines what happens with this objects collision.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

            if (player != null)
            {
                StartCoroutine(player.AddScore());
                player.RemoveFromAttack();

                // player.IncreaseSize(); // Current disabled
            }
            else
            {
                Debug.Log("Diamond::Player is null!");
            }

            var gemCount = PlayerPrefs.GetInt("GemsCollected");
            PlayerPrefs.SetInt("GemsCollected", gemCount + 1);

            Instantiate(_pickupEffect, transform.position, Quaternion.identity);

            _collider2D.enabled = false;
            _spriteRenderer.enabled = false;

            _score.gameObject.SetActive(true);
            _score.text = GameManager.Instance.CurrentGemScore.ToString("N0");
            _score.GetComponent<Animator>().SetTrigger("Rise");

            if (GameManager.Instance.IsArcade())
            {
                _time.gameObject.SetActive(true);
                _time.text = "+ " + GameManager.Instance.TimerIncrement.ToString();
                _time.GetComponent<Animator>().SetTrigger("Rise");
            }
            else if (GameManager.Instance.IsHardcore() && GameManager.Instance.HardCoreTimerLock)
            {
                _time.gameObject.SetActive(true);
                _time.text = "+ " + GameManager.Instance.TimerIncrement.ToString();
                _time.GetComponent<Animator>().SetTrigger("Rise");
            }

            if (_soundEffect != null)
            {
                _soundEffect.PlayGemSound();
                // _audioSource.Play();
            }

            Destroy(gameObject, 1f);
        }

        if (other.tag == "Mine")
        {
            other.GetComponent<Mine>().Explode();
        }
    }
}
