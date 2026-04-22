using System.Linq;
using System.Net;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField ipInput, portInput, usernameInput;
    [SerializeField] TextMeshProUGUI yourIP;
    string defaultIP = "127.0.0.1";
    ushort defaultPort = 7777;

    [SerializeField]UnityTransport transport;
    [SerializeField]NetworkManager networkManager;

    private void Awake()
    {
        if(ipInput) ipInput.text = defaultIP;
        if(portInput) portInput.text = defaultPort.ToString();
        if (yourIP) yourIP.text = GetLocalIPv4();
    }

    public void StartGame()
    {
       
        networkManager.SceneManager.LoadScene("Level", LoadSceneMode.Single);
    }
    public void JoinGame()
    {
        if (!transport) transport = FindAnyObjectByType<UnityTransport>();
        if (!networkManager) networkManager = FindAnyObjectByType<NetworkManager>();
        if (networkManager.IsListening)
        {
            networkManager.Shutdown();
        }
        string ip = GetIP();
        ushort port = GetPort();
        transport.SetConnectionData(ip, port);
        networkManager.StartClient();
        SetUsername();
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
    public string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
        .AddressList.First(
        f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        .ToString();
    }

    public void StartHost()
    {
        if (!transport) transport = FindAnyObjectByType<UnityTransport>();
        if (!networkManager) networkManager = FindAnyObjectByType<NetworkManager>();
        if (networkManager.IsListening)
        {
            networkManager.Shutdown();
        }
        ushort port = GetPort();
        transport.SetConnectionData("0.0.0.0", port);
        networkManager.StartHost();
        SetUsername();
    }
    public void SetUsername()
    {
        if (!transport) transport = FindAnyObjectByType<UnityTransport>();
        if (!networkManager) networkManager = FindAnyObjectByType<NetworkManager>();
        if (networkManager.IsClient)
        if(!usernameInput || string.IsNullOrWhiteSpace(usernameInput.text))
        PlayerData.Local.SetUsernameServerRpc((FixedString64Bytes)("Player"));
            else
                PlayerData.Local.SetUsernameServerRpc((FixedString64Bytes)(usernameInput.text));
    }
    public void HideCanvas()
    {
        transform.parent.gameObject.SetActive(false);
    }
 
}
