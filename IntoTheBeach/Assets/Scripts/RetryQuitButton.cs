using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryQuitButton : NetworkBehaviour
{
    public async Task Retry() 
    {
        //if (NetworkManager.Singleton.IsServer)
        //{
        //    NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        //}
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
            await Task.Yield();
        }
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);

    }
    public void Quit() 
    {
        Application.Quit();
    }
}
