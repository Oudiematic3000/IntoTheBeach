using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField ipInput, portInput;
    string defaultIP = "127.0.0.1";
    ushort defaultPort = 7777;

    [SerializeField]UnityTransport transport;
    [SerializeField]NetworkManager networkManager;

    private void Awake()
    {
        if(ipInput) ipInput.text = defaultIP;
        if(portInput) portInput.text = defaultPort.ToString();
    }

    public void StartHost()
    {
        ushort port = GetPort();
        transport.SetConnectionData("0.0.0.0", port);
        networkManager.StartHost();
        SceneManager.LoadScene("Level");
    }
    public void JoinGame()
    {
        string ip = GetIP();
        ushort port = GetPort();
        transport.SetConnectionData(ip, port);
        networkManager.StartClient();
    }
    public void StartServerOnly()
    {
        ushort port=GetPort();
        transport.SetConnectionData("0.0.0.0", port);
        networkManager.StartServer();
    }
    string GetIP()
    {
        if (!ipInput || string.IsNullOrWhiteSpace(ipInput.text)) return defaultIP;

        return ipInput.text.Trim();
    }
    ushort GetPort()
    {
        if(!portInput || !ushort.TryParse(portInput.text, out ushort port))return defaultPort;

        return port;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
