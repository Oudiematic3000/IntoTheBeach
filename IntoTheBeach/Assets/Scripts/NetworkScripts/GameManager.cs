using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public GridState GridState { get; private set; } = new();
    [SerializeField] private Tilemap floorTilemap;
    public Tilemap FloorTilemap => floorTilemap;
    private Dictionary<int, CharacterVisual> unitVisuals = new();
    public static event Action<string, int> winnerBroadcast;
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
        registeredPlayers.Clear();


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
        registeredPlayers.Clear();

        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetTeam(i);
            registeredPlayers.Add(players[i]);
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
            if (!registeredPlayers.Contains(players[i]))
                registeredPlayers.Add(players[i]);
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
    private List<PlayerData> registeredPlayers = new();

    public void RegisterPlayer(PlayerData player)
    {
        if (!registeredPlayers.Contains(player))
            registeredPlayers.Add(player);
    }

    public void CheckWinCondition()
    {
        if (!IsServer) return;

        Debug.Log($"CheckWinCondition — players: {registeredPlayers.Count}, units: {unitVisuals.Count}");

        for (int i = 0; i < registeredPlayers.Count; i++)
        {
            Debug.Log($"Iteration {i}");
            var player = registeredPlayers[i];

            if (player == null)
            {
                Debug.LogError($"Player {i} is null in registeredPlayers!");
                continue;
            }

            Debug.Log($"Player {i} team: {player.TeamIndex.Value}");

            bool hasLivingUnits = unitVisuals.Any(kvp =>
                kvp.Value != null &&
                kvp.Value.teamIndex == player.TeamIndex.Value &&
                !GridState.IsDead(kvp.Key));

            Debug.Log($"Player {i} hasLivingUnits: {hasLivingUnits}");

            if (!hasLivingUnits)
            {
                var winner = registeredPlayers.FirstOrDefault(p => p != null && p != player);
                if (winner != null)
                {
                    Debug.Log($"Winner: {winner.Username.Value}");
                    BroadcastWinnerClientRpc(winner.Username.Value, winner.TeamIndex.Value);
                }
                else
                {
                    Debug.LogError("No winner found — winner is null");
                }
                return;
            }
        }

        Debug.Log("No loser found this turn");
    }

    [ClientRpc]
    private void BroadcastWinnerClientRpc(FixedString64Bytes winnerUsername, int teamIndex)
    {
        Debug.Log($"Game over! Winner: {winnerUsername}");
        winnerBroadcast?.Invoke(winnerUsername.ToString(),teamIndex);
    }

    public void RemoveUnit(int unitID)
    {
        Debug.Log("RemovedUnit ID: " + unitID);
        unitVisuals.Remove(unitID);
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
        return (environmentalObjects.TryGetValue(position, out var obj) && obj.MovementBlocker != null);
            
    }

    public bool IsAttackBlocked(Vector3Int position, Vector3Int attackDirection)
    {
        if (!environmentalObjects.TryGetValue(position, out var obj)) return false;
        return obj.AttackBlocker?.BlocksAttackFromDirection(attackDirection) ?? false;
    }

    public void TriggerAttackReaction(Vector3Int position, int attackerID)
    {
        if (environmentalObjects.TryGetValue(position, out var obj))
            obj.AttackReactor?.OnAttacked(this, attackerID);
    }

    public EnvironmentalObject GetEnvironmentalObject(Vector3Int position)
    {
        return environmentalObjects.TryGetValue(position, out var obj) ? obj : null;
    }
    public void RemoveDeadUnit(int unitID)
    {
        Vector3Int? pos = GetUnitPosition(unitID);
        if (pos.HasValue)
            unitPositions.Remove(pos.Value);
    }
}
