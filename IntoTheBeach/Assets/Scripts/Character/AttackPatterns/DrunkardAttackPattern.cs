using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class DrunkardAttackPattern : AttackPattern
{
    public override int TypeID => AttackPatternRegistry.DrunkardAttack;
    static int range = 4;

    private static readonly Vector3Int[] DirectionVectors =
    {
        Vector3Int.left,   // 0
        Vector3Int.up,     // 1
        Vector3Int.right,  // 2
        Vector3Int.down    // 3
    };
    private static readonly Vector3Int[] PerpVectors =
    {
        Vector3Int.up,    // 0 left   spread on Y
        Vector3Int.right, // 1 up     spread on X
        Vector3Int.up,    // 2 right  spread on Y
        Vector3Int.right  // 3 down   spread on X
    };

    public override List<Vector3Int> AttackTilesVisual(Tilemap floor, Tilemap obstacles, Vector3Int position)
    {
        List<Vector3Int> tiles = new();
        Vector3Int center = new Vector3Int(position.x - range, position.y, 0);
        for (int s = -1; s <= 1; s++)
            for (int d = -1; d <= 1; d++)
                tiles.Add(new Vector3Int(center.x + d, center.y + s, 0));
        return tiles;
    }

    public override List<Vector3Int> GetHitTiles(GridState gridState, Vector3Int position, int direction)
    {
        List<Vector3Int> tiles = new();
        Vector3Int dir = DirectionVectors[direction];
        Vector3Int perp = PerpVectors[direction];

        Vector3Int center = position + dir * range;


        for (int d = -1; d <= 1; d++)
            for (int s = -1; s <= 1; s++)
            {
                Vector3Int tile = center + dir * d + perp * s;
                tiles.Add(tile); 
            }

        return tiles;
    }

    public override List<Vector3Int> GetBlockedTiles(GridState gridState, Vector3Int position, int direction)
    {
        List<Vector3Int> tiles = new();
        Vector3Int dir = DirectionVectors[direction];
        Vector3Int perp = PerpVectors[direction];

        Vector3Int center = position + dir * range;


        for (int d = -1; d <= 1; d++)
            for (int s = -1; s <= 1; s++)
            {
                Vector3Int tile = center + dir * d + perp * s;
                if(gridState.IsMovementBlocked(tile))
                tiles.Add(tile);
            }

        return tiles;
    }
       
}