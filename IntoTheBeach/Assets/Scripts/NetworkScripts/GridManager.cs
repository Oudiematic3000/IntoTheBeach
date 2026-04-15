using UnityEngine;
using Unity.Netcode;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridManager : NetworkBehaviour
{
    public static GridManager instance;
    public GridState gridState;
    private Tilemap floor, obstacles;
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        gridState = new GridState();
        floor = GameObject.Find("FloorVisual").GetComponent<Tilemap>();
        obstacles = GameObject.Find("Obstacles").GetComponent<Tilemap>();
        PopulateGridstate();
    }

    public override void OnNetworkDespawn()
    {
        if(instance==this)instance = null;
    }

    public void RegisterUnitIDs()
    {
        foreach(var unit in FindObjectsByType<CharacterVisual>(FindObjectsSortMode.InstanceID))
        {   
            //gridState.SetOccupant(floor.WorldToCell(unit.transform.position), unit.unitID);
        }
    }

    public void PopulateGridstate()
    {
        foreach(var tile in obstacles.cellBounds.allPositionsWithin)
        {
            
            TileBase currentTile = obstacles.GetTile(tile);
            if (currentTile == null) continue;
            if (currentTile is Obstacle obstacle)
            {
                gridState.AddObstacle(tile, new BlockPositionDirection(obstacle.blockDirections));
                Debug.Log("OBBY");
            }
        }
    }

}
public class GridState
{
    private Dictionary<Vector3Int, int> entities = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, BlockPositionDirection> obstacles = new Dictionary<Vector3Int, BlockPositionDirection>();
    public void Clear()
    {
        entities.Clear();
        obstacles.Clear();
    }
    public void AddObstacle(Vector3Int pos, BlockPositionDirection direction)
    {
        obstacles[pos]=direction;
    }
    public void SetOccupant(Vector3Int pos, int entityID)
    {
        entities[pos]=entityID;
    }
    public bool IsUnitOccupied(Vector3Int pos)
    {
        if(entities.ContainsKey(pos))return true;
        else return false;
    }
    public bool IsObstacleOccupied(Vector3Int pos)
    {
        if (obstacles.ContainsKey(pos)) return true;
        else return false;

    }
    public int GetUnitOccupantID(Vector3Int pos)
    {    
        return entities[pos];
    }
    public List<Vector3Int> GetObstacleOccupantDirections(Vector3Int pos)
    {
        return obstacles[pos].directions;
    }
}

