using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField joinCodeInput, usernameInput;
    [SerializeField] TextMeshProUGUI joinCodeDisplay, statusText;
    [SerializeField] UnityTransport transport;
    [SerializeField] NetworkManager networkManager;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public void StartGame()
    {
        networkManager.SceneManager.LoadScene("Level", LoadSceneMode.Single);
    }

    public async void StartHost()
    {
        if (!transport) transport = FindAnyObjectByType<UnityTransport>();
        if (!networkManager) networkManager = FindAnyObjectByType<NetworkManager>();

        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "udp"));

            networkManager.StartHost();
            SetUsername();

            if (joinCodeDisplay) joinCodeDisplay.text = joinCode;
            Debug.Log($"Relay join code: {joinCode}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to start host with relay: {e}");
            if (statusText) statusText.text = "Failed to create session";
        }
    }

    public async void JoinGame()
    {
        if (!transport) transport = FindAnyObjectByType<UnityTransport>();
        if (!networkManager) networkManager = FindAnyObjectByType<NetworkManager>();

        string code = joinCodeInput ? joinCodeInput.text.Trim() : "";
        if (string.IsNullOrEmpty(code))
        {
            if (statusText) statusText.text = "Enter a join code";
            return;
        }

        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(code);
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "udp"));

            networkManager.StartClient();
            LeanTween.delayedCall(0.1f, WaitForPlayerData);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to join relay: {e}");
            if (statusText) statusText.text = "Invalid join code";
        }
    }

    private void WaitForPlayerData()
    {
        if (PlayerData.Local != null)
            SetUsername();
        else
            LeanTween.delayedCall(0.1f, WaitForPlayerData);
    }

    public void SetUsername()
    {
        if (!networkManager.IsClient) return;
        if (PlayerData.Local == null) return;

        FixedString64Bytes name = (!usernameInput || string.IsNullOrWhiteSpace(usernameInput.text))
            ? "Player"
            : (FixedString64Bytes)usernameInput.text;

        PlayerData.Local.SetUsernameServerRpc(name);
    }

    public void HideCanvas()
    {
        transform.parent.gameObject.SetActive(false);
    }
}