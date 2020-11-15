using UnityEngine;

public class Mine : MonoBehaviour
{

    private float _rotationSpeed;
    private Animator _animator;
    private Collider2D _collider2D;
    private AudioSource _audioSource;

    [SerializeField]
    private GameObject _explosion;


    // Start is called before the first frame update
    void Start()
    {
        _rotationSpeed = Random.Range(10f, 25f);

        _animator = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();

        if (_animator == null)
        {
            Debug.LogError("Mine::Animator is Null!");
        }

        if (_collider2D == null)
        {
            Debug.LogError("Mine::Collider2D is null!");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Mine::AudioSource is null!");
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, _rotationSpeed) * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

            if (player != null)
            {
                player.RemoveLife();
            }
            else
            {
                Debug.Log("Mine::Player is null!");
            }

            var mineCount = PlayerPrefs.GetInt("MinesHit");
            PlayerPrefs.SetInt("MinesHit", mineCount + 1);

            Explode();
        }
    }

    public void Explode(bool noSound = false)
    {
        if (_collider2D != null)
        {
            _collider2D.enabled = false;
        }

        if (_audioSource.clip != null)
        {
            if (!noSound)
            {
                _audioSource.Play();
            }
        }

        _animator.SetTrigger("Explode");
        
        if (_explosion != null)
        {
            Instantiate(_explosion, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 1f);
    }

}
