using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyPlayerListUI : MonoBehaviour
{
    public static LobbyPlayerListUI Instance { get; private set; }

    [SerializeField] TextMeshProUGUI playerEntryPrefab;
    [SerializeField] Transform listContainer;

    readonly List<TextMeshProUGUI> entries = new();

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Refresh()
    {
        foreach (var e in entries) Destroy(e.gameObject);
        entries.Clear();

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerData = client.PlayerObject?.GetComponent<PlayerData>();
            if (playerData == null) continue;

            string name = playerData.Username.Value.ToString();
            bool isLocal = client.ClientId == NetworkManager.Singleton.LocalClientId;

            var entry = Instantiate(playerEntryPrefab, listContainer);
            entry.text = isLocal ? $"{name} (You)" : name;
            entries.Add(entry);
        }
    }
}