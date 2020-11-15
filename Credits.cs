using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1.5f;

    Button _backButton;

    void Start()
    {
        _backButton = GameObject.FindGameObjectWithTag("Button").GetComponent<Button>();

        if (_backButton != null)
        {
            _backButton.Select();
        }
    }

    void Update()
    {
        var moveUp = new Vector3(0f, 1f, 0f) * _speed;

        transform.Translate(moveUp * Time.deltaTime);

        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }
    }

    public void Back()
    {
        SceneManager.LoadScene("Menu");
    }
}
