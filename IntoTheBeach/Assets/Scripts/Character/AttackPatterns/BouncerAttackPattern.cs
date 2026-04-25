using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class BouncerAttackPattern : AttackPattern
{
    public override int TypeID => AttackPatternRegistry.BouncerAttack;
    static int rows = 2;
    static int width = 2;

    private static readonly Vector3Int[] DirectionVectors =
    {
        Vector3Int.left,   // 0
        Vector3Int.up,     // 1
        Vector3Int.right,  // 2
        Vector3Int.down    // 3
    };

    private static readonly Vector3Int[] PerpVectors =
    {
        Vector3Int.up,    // 0 left  spread on Y
        Vector3Int.right, // 1 up     spread on X
        Vector3Int.up,    // 2 right spread on Y
        Vector3Int.right  // 3 down   spread on X
    };

    public override List<Vector3Int> AttackTilesVisual(Tilemap floor, Tilemap obstacles, Vector3Int position)
    {
        List<Vector3Int> tiles = new();

        for (int row = 1; row <= rows; row++)
        {

            int spread = (width - 1) + (row - 1);
            Vector3Int rowCenter = new Vector3Int(position.x - row, position.y);

            for (int s = -spread; s <= spread; s++)
                tiles.Add(new Vector3Int(rowCenter.x, rowCenter.y + s, 0));
        }

        return tiles;
    }

    public override List<Vector3Int> GetHitTiles(GridState gridState, Vector3Int position, int direction)
    {
        List<Vector3Int> tiles = new();
        Vector3Int dir = DirectionVectors[direction];
        Vector3Int perp = PerpVectors[direction];

        var blockedLanes = new HashSet<int>();

        for (int row = 1; row <= rows; row++)
        {
            int spread = (width - 1) + (row - 1);
            Vector3Int rowCenter = position + dir * row;

            for (int s = -spread; s <= spread; s++)
            {
                
                if (blockedLanes.Contains(s)) continue;

                Vector3Int tile = rowCenter + perp * s;

                if (gridState.IsAttackBlocked(tile, dir))
                {
                    blockedLanes.Add(s);  
                    continue;
                }
                if (gridState.GetEnvironmentalObject(tile)!=null)
                {
                    continue;
                }
                tiles.Add(tile);
            }
        }

        return tiles;
    }
}