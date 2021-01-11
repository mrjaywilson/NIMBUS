using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles the exit confirmation panel when the user escapes.
/// 
/// </summary>
public class ConfirmExit : MonoBehaviour
{
    [SerializeField]
    private Button _no = null;

    public Button NoButton { get { return _no; } }

    /// <summary>
    /// Method handles when the player presses the confirmation button.
    /// </summary>
    public void Yes()
    {
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Method to handle when the player declines.
    /// </summary>
    public void No()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().ToggleBlock();
        GameManager.Instance.ToggleConfirmQuit();
    }    
}
