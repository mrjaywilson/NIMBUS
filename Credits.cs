using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles the credits scene.
/// 
/// </summary>
public class Credits : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1.5f;

    Button _backButton;

    /// <summary>
    /// Initial method called right before the first frame after instantiation.
    /// </summary>
    void Start()
    {
        _backButton = GameObject.FindGameObjectWithTag("Button").GetComponent<Button>();

        if (_backButton != null)
        {
            _backButton.Select();
        }
    }

    /// <summary>
    /// Credits Game Loop
    /// </summary>
    void Update()
    {
        var moveUp = new Vector3(0f, 1f, 0f) * _speed;

        transform.Translate(moveUp * Time.deltaTime);

        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }
    }

    /// <summary>
    /// Method to return player back to menu.
    /// </summary>
    public void Back()
    {
        SceneManager.LoadScene("Menu");
    }
}
