using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
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
            InitialiseMatch();
    }


    private void InitialiseMatch()
    {
        var allVisuals = FindObjectsByType<CharacterVisual>(FindObjectsSortMode.None);
        var players = NetworkManager.Singleton.ConnectedClientsList
            .Select(c => c.PlayerObject?.GetComponent<PlayerData>())
            .Where(pd => pd != null)
            .ToList();

        foreach (var visual in allVisuals)
        {
            int unitID = nextUnitID++;
            visual.unitID = unitID;  
            unitVisuals[unitID] = visual;

            PlayerData owner = players.FirstOrDefault(p => p.TeamIndex.Value == visual.teamIndex);
            if (owner != null)
                owner.RegisterUnit(unitID);

            GridState.RegisterUnit(unitID, visual.GetTilePos(floorTilemap), visual.unitClass.health);
        }
    }


    public CharacterVisual GetVisual(int unitID)
    {
        return unitVisuals.TryGetValue(unitID, out var visual) ? visual : null;
    }

    public IReadOnlyDictionary<int, CharacterVisual> GetAllVisuals() => unitVisuals;
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
