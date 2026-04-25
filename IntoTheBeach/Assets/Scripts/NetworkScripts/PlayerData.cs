using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    public static PlayerData Local { get; private set; }

    public NetworkVariable<FixedString64Bytes> Username = new NetworkVariable<FixedString64Bytes>(
        "Player",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<int> TeamIndex = new NetworkVariable<int>(
        -1,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );


    private readonly List<int> ownedUnits = new();
    public IReadOnlyList<int> OwnedUnits => ownedUnits;

    public static readonly Dictionary<int, PlayerData> unitOwnerMap = new();

    private NetworkVariable<FixedString64Bytes>.OnValueChangedDelegate _onUsernameChanged;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Local = this;
            string saved = PlayerPrefs.GetString("Username", "Player");
            SetUsernameServerRpc(saved);
        }

        _onUsernameChanged = (_, _) => LobbyPlayerListUI.Instance?.Refresh();
        Username.OnValueChanged += _onUsernameChanged;
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner) Local = null;
        Username.OnValueChanged -= _onUsernameChanged; 
        if (IsServer) ClearUnits();
    }

    [ServerRpc]
    public void SetUsernameServerRpc(FixedString64Bytes newUsername)
    {
        Username.Value = newUsername;
    }

    public void SetTeam(int teamIndex)
    {
        if (!IsServer) return;
        TeamIndex.Value = teamIndex;
    }

    public void RegisterUnit(int unitID)
    {
        if (!IsServer) return;
        if (ownedUnits.Contains(unitID)) return;

        ownedUnits.Add(unitID);
        unitOwnerMap[unitID] = this;
    }
    public void ClearUnits()
    {
        if (!IsServer) return;

        foreach (var unit in ownedUnits)
            unitOwnerMap.Remove(unit);

        ownedUnits.Clear();
    }

    public static bool IsOwnedByLocal(int unitID)
    {
        return unitOwnerMap.TryGetValue(unitID, out var owner)
            && owner == Local;
    }

    public static PlayerData GetOwner(int unitID)
    {
        unitOwnerMap.TryGetValue(unitID, out var owner);
        return owner;
    }

    public static List<PlayerData> GetTeam(int teamIndex)
    {
        var result = new List<PlayerData>();
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var pd = client.PlayerObject?.GetComponent<PlayerData>();
            if (pd != null && pd.TeamIndex.Value == teamIndex)
                result.Add(pd);
        }
        return result;
    }
}