using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryQuitButton : MonoBehaviour
{
    public void Retry() 
    {
        SceneManager.LoadScene("Lobby");
    }
    public void Quit() 
    {
        Application.Quit();
    }
}
