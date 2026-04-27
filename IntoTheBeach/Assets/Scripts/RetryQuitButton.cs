using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryQuitButton : NetworkBehaviour
{
    public void Retry() 
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
       
    }
    public void Quit() 
    {
        Application.Quit();
    }
}
