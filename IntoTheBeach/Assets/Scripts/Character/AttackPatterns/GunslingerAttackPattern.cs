using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class GunslingerAttackPattern : AttackPattern
{
    public override int TypeID => AttackPatternRegistry.GunslingerAttack;
    static int range = 5;

    private static readonly Vector3Int[] DirectionVectors =
    {
        Vector3Int.left,   // 0
        Vector3Int.up,     // 1
        Vector3Int.right,  // 2
        Vector3Int.down    // 3
    };

    public override List<Vector3Int> AttackTilesVisual(Tilemap floor, Tilemap obstacles, Vector3Int position)
    {
        List<Vector3Int> tiles = new();
        for (int i = 1; i <= range; i++)
        {
            Vector3Int currentTile = new Vector3Int(position.x - i, position.y);
            tiles.Add(currentTile);
        }
        return tiles;
    }

    public override List<Vector3Int> GetHitTiles(GridState gridState, Vector3Int position, int direction)
    {
        List<Vector3Int> tiles = new();
        Vector3Int dir = DirectionVectors[direction];

        for (int i = 1; i <= range; i++)
        {
            Vector3Int current = position + dir * i;

            if (gridState.IsAttackBlocked(current, dir)) break;

            tiles.Add(current);

            if (gridState.GetUnitAtPosition(current).HasValue) break;
        }
        return tiles;
    }
}