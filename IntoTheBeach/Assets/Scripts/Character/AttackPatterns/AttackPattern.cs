using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public abstract class AttackPattern
{
    public abstract List<Vector3Int> AttackTilesVisual(Tilemap floor, Tilemap obstacles, Vector3Int position);
    public static Vector3Int RotateAroundPivot(Vector3Int tile, Vector3Int pivot, int rotation)
    {
        Vector3Int offset = tile - pivot;
        Vector3Int rotated = rotation switch
        {
            0 => offset,
            1 => new Vector3Int(offset.y, -offset.x, 0),
            2 => new Vector3Int(-offset.x, -offset.y, 0),
            3 => new Vector3Int(-offset.y, offset.x, 0),
            _ => offset
        };
        return rotated + pivot;
    }

    public static List<Vector3Int> GetRotatedAttackTiles(List<Vector3Int> absoluteTiles, Vector3Int unitPosition, int rotation)
    {
        var tiles = new List<Vector3Int>();
        foreach (var tile in absoluteTiles)
        {
            tiles.Add(RotateAroundPivot(tile, unitPosition, rotation));
        }
        return tiles;
    }
    public static List<List<Vector3Int>> GetDirectionSeparatedList(List<Vector3Int> absoluteTiles, Vector3Int unitPosition)
    {
        List<List<Vector3Int>> directionSeperatedList = new List<List<Vector3Int>>();
        for(int i = 0; i < 4; i++)
        {
            directionSeperatedList.Add(GetRotatedAttackTiles(absoluteTiles, unitPosition, i));
        }
        return directionSeperatedList;
    }
    public static List<Vector3Int> GetAllAttackHighlightTiles(List<Vector3Int> absoluteTiles, Vector3Int unitPosition, Tilemap obstacles)               
    {
        var allTiles = new HashSet<Vector3Int>();
        for (int r = 0; r < 4; r++)
        {
            foreach (var tile in absoluteTiles)
            {
                Vector3Int rotated = RotateAroundPivot(tile, unitPosition, r);
                if (obstacles.GetTile(rotated) is Obstacle) continue;
                allTiles.Add(rotated);
            }
        }
        return new List<Vector3Int>(allTiles);
    }
}
