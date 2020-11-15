using UnityEngine;
using TMPro;

public class Gem : MonoBehaviour
{
    [SerializeField]
    // private int _scoreValue;
    private Collider2D _collider2D;
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private ParticleSystem _pickupEffect;

    [SerializeField]
    private TextMeshProUGUI _score;
    [SerializeField]
    private TextMeshProUGUI _time;


    // public int ScoreValue { get { return _scoreValue; } set { _scoreValue = value; } }

    // Start is called before the first frame update
    void Start()
    {
        _collider2D = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (_collider2D == null)
        {
            Debug.LogError("Diamond::Collider2D is null!");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Diamond::AudioSource is null!");
        }

        if (_spriteRenderer == null)
        {
            Debug.LogError("Diamond::SpriteRenderer is null!");
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Collider2D overlapObjectCollider = Physics2D.OverlapCircle(transform.position, 0.5f);

        //if (overlapObjectCollider != null)
        //{
        //    var mine = overlapObjectCollider.GetComponentInParent<Mine>();

        //    if (mine != null)
        //    {
        //        mine.Explode();
        //    }
        //}
    }

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

            if (_audioSource.clip != null)
            {
                _audioSource.Play();
            }

            Destroy(gameObject, 1f);
        }

        if (other.tag == "Mine")
        {
            other.GetComponent<Mine>().Explode();
        }
    }
}
