using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public GridState GridState { get; private set; } = new();
    [SerializeField] private Tilemap floorTilemap;
    private Dictionary<int, CharacterVisual> unitVisuals = new();

    private int nextUnitID = 0;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            InitialiseMatch();
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;

        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        }
    }
    private void InitialiseMatch()
    {
        if (!IsServer) return;

        var allVisuals = FindObjectsByType<CharacterVisual>(FindObjectsSortMode.None);
        unitVisuals.Clear();
        nextUnitID = 0;

        var syncDataList = new List<UnitSyncData>();
        foreach (var visual in allVisuals)
        {
            Vector3Int tilePos = visual.GetTilePos(floorTilemap);
            int teamIndex = GetTeamIndexForTile(tilePos);

            visual.unitID = nextUnitID;
            visual.teamIndex = teamIndex;
            unitVisuals[nextUnitID] = visual;
            GridState.RegisterUnit(nextUnitID, tilePos, visual.unitClass.health);  
            syncDataList.Add(new UnitSyncData
            {
                tilePos = tilePos,
                unitID = nextUnitID,
                teamIndex = teamIndex
            });

            nextUnitID++;
        }

        UnitSyncData[] syncDataArray = syncDataList.ToArray();

        var players = NetworkManager.Singleton.ConnectedClientsList
            .Select(c => c.PlayerObject?.GetComponent<PlayerData>())
            .Where(pd => pd != null)
            .ToList();

        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetTeam(i);  

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { players[i].OwnerClientId }
                }
            };

            SyncUnitsClientRpc(syncDataArray, i, clientRpcParams);
        }
    }

    [ClientRpc]
    private void SyncUnitsClientRpc(UnitSyncData[] syncDataArray, int assignedPlayerTeamIndex, ClientRpcParams clientRpcParams = default)
    {
        if (IsServer) return;

        var allVisuals = FindObjectsByType<CharacterVisual>(FindObjectsSortMode.None);

        foreach (var syncData in syncDataArray)
        {
            var visual = allVisuals.FirstOrDefault(v => v.GetTilePos(floorTilemap) == syncData.tilePos);
            if (visual != null)
            {
                visual.unitID = syncData.unitID;
                visual.teamIndex = syncData.teamIndex;  
                unitVisuals[syncData.unitID] = visual;
            }
        }

        var localPlayerData = NetworkManager.Singleton.LocalClient?.PlayerObject?.GetComponent<PlayerData>();
        if (localPlayerData != null)
            localPlayerData.SetTeam(assignedPlayerTeamIndex);
    }
    private void HandleClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId) return;

        var players = NetworkManager.Singleton.ConnectedClientsList
            .Select(c => c.PlayerObject?.GetComponent<PlayerData>())
            .Where(pd => pd != null)
            .ToList();

        int newClientTeamIndex = -1;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetTeam(i);
            if (players[i].OwnerClientId == clientId)
                newClientTeamIndex = i;
        }

        var syncDataList = new List<UnitSyncData>();
        foreach (var kvp in unitVisuals)
        {
            Vector3Int tilePos = kvp.Value.GetTilePos(floorTilemap);
            syncDataList.Add(new UnitSyncData
            {
                tilePos = tilePos,
                unitID = kvp.Key,
                teamIndex = GetTeamIndexForTile(tilePos)
            });
        }

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        SyncUnitsClientRpc(syncDataList.ToArray(), newClientTeamIndex, clientRpcParams);
    }
    public CharacterVisual GetVisual(int unitID)
    {
        return unitVisuals.TryGetValue(unitID, out var visual) ? visual : null;
    }

    public IReadOnlyDictionary<int, CharacterVisual> GetAllVisuals() => unitVisuals;

    private int GetTeamIndexForTile(Vector3Int tilePos)
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        float midY = bounds.yMin + bounds.size.y / 2f;
        return tilePos.y >= midY ? 1 : 0;
    }
}
public struct UnitSyncData : INetworkSerializable
{
    public Vector3Int tilePos;
    public int unitID;
    public int teamIndex;  

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tilePos);
        serializer.SerializeValue(ref unitID);
        serializer.SerializeValue(ref teamIndex);
    }
}
public class GridState
{
    private Dictionary<Vector3Int, EnvironmentalObject> environmentalObjects = new();
    private Dictionary<Vector3Int, int> unitPositions = new();
    private Dictionary<int, int> unitHealth = new();

    public void RegisterEnvironmentalObject(EnvironmentalObject obj)
    {
        foreach (var tile in obj.OccupiedTiles)
            environmentalObjects[tile] = obj;
    }

    public void UnregisterEnvironmentalObject(EnvironmentalObject obj)
    {
        foreach (var tile in obj.OccupiedTiles)
            environmentalObjects.Remove(tile);
    }

    public void RegisterUnit(int unitID, Vector3Int position, int startingHealth)
    {
        unitPositions[position] = unitID;
        unitHealth[unitID] = startingHealth;
    }

    public void UpdateUnitPosition(int unitID, Vector3Int oldPos, Vector3Int newPos)
    {
        unitPositions.Remove(oldPos);
        unitPositions[newPos] = unitID;
    }

    public Vector3Int? GetUnitPosition(int unitID)
    {
        foreach (var kvp in unitPositions)
            if (kvp.Value == unitID) return kvp.Key;
        return null;
    }

    public int? GetUnitAtPosition(Vector3Int position)
    {
        return unitPositions.TryGetValue(position, out int id) ? id : null;
    }

    public Vector3Int WorldToCell(Vector3 position)
    {
        Tilemap tilemap = GameObject.Find("FloorVisual").GetComponent<Tilemap>();
        return tilemap.WorldToCell(position);
    }

    public int GetHealth(int unitID) => unitHealth.TryGetValue(unitID, out int hp) ? hp : 0;

    public void ApplyDamage(int unitID, int damage)
    {
        if (unitHealth.ContainsKey(unitID))
            unitHealth[unitID] = Mathf.Max(0, unitHealth[unitID] - damage);
    }
    public IEnumerable<int> GetAllUnitIDs() => unitHealth.Keys;
    public bool IsDead(int unitID) => GetHealth(unitID) <= 0;

    public bool IsMovementBlocked(Vector3Int position)
    {
        return (environmentalObjects.TryGetValue(position, out var obj) && obj is IMovementBlocker)
            || unitPositions.ContainsKey(position);
    }

    public bool IsAttackBlocked(Vector3Int position, Vector3Int attackDirection)
    {
        if (!environmentalObjects.TryGetValue(position, out var obj)) return false;
        if (obj is IAttackBlocker blocker)
            return blocker.BlocksAttackFromDirection(attackDirection);
        return false;
    }

    public void TriggerAttackReaction(Vector3Int position, int attackerID)
    {
        if (environmentalObjects.TryGetValue(position, out var obj) && obj is IAttackReactor reactor)
            reactor.OnAttacked(this, attackerID);
    }

    public EnvironmentalObject GetEnvironmentalObject(Vector3Int position)
    {
        return environmentalObjects.TryGetValue(position, out var obj) ? obj : null;
    }
}
