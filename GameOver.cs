using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles the Game Over scene.
/// 
/// </summary>
public class GameOver : MonoBehaviour
{
    [SerializeField]
    private Button _retryButton = null;

    /// <summary>
    /// Method called before the first frame after instantation and before update loops starts
    /// </summary>
    void Start()
    {
        if (_retryButton != null)
        {
            _retryButton.Select();
        }
    }

    /// <summary>
    /// Method called if the player intends to exit the current game rather than retry.
    /// </summary>
    public void EndGame()
    {
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Method called if the player decides to retry the game.
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
