using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Custom Tile")]
public class Obstacle : Tile
{
    public List<Vector3Int> blockDirections;
  
}

public struct BlockPositionDirection
{
    public List<Vector3Int> directions;
    

    public BlockPositionDirection(List<Vector3Int> blockDirections) : this()
    {
        directions = new List<Vector3Int>(blockDirections);
    }

    public bool CheckBlockDirection(Vector3Int from, Vector3Int to)
    {
        Vector3Int direction = to - from;
        if (directions.Contains(direction)) return true;
        else return false;
    }
}