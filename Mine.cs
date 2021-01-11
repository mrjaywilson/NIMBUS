using System.Collections;
using UnityEngine;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles gems.
/// 
/// </summary>
public class Mine : MonoBehaviour
{

    private float _rotationSpeed;
    private Animator _animator;
    private Collider2D _collider2D;

    [SerializeField]
    private MineEffects _mineEffects = null;

    [SerializeField]
    private GameObject _explosion = null;

    /// <summary>
    /// Method called after instantiation and before the furst update loop frame
    /// </summary>
    void Start()
    {
        _rotationSpeed = Random.Range(10f, 25f);

        _animator = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();
        _mineEffects = GameObject.FindGameObjectWithTag("MineEffects").GetComponent<MineEffects>();

        if (_animator == null)
        {
            Debug.LogError("Mine::Animator is Null!");
        }

        if (_collider2D == null)
        {
            Debug.LogError("Mine::Collider2D is null!");
        }
    }

    /// <summary>
    /// Mine game loop.
    /// </summary>
    void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, _rotationSpeed) * Time.deltaTime);
    }

    /// <summary>
    /// Method for handling collision with Mines.
    /// </summary>
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

    /// <summary>
    /// Method handles what happens when a mine explodes.
    /// </summary>
    /// <param name="noSound">bool: Set to true if no sound should be played during explosion.</param>
    public void Explode(bool noSound = false)
    {
        if (_collider2D != null)
        {
            _collider2D.enabled = false;
        }

        // Removed Audio for Test Build Build2.45.A4.00-04.apk
        // Checking for FPS Drops
        if (!noSound)
        {
            _mineEffects.PlayMineHitSound();
            //_audioSource.Play();
        }

        _animator.SetTrigger("Explode");
        
        if (_explosion != null)
        {
            //Instantiate(_explosion, transform.position, Quaternion.identity);
        }

        StartCoroutine(DeactivateSelf());

        // destroy and instead deactivate self??
        Destroy(gameObject, 1f);

    }

    /// <summary>
    /// Deactivate the game object.
    /// </summary>
    private IEnumerator DeactivateSelf()
    {
        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);

    }
}
