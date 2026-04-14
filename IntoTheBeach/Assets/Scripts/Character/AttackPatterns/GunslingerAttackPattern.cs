using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using static UnityEditor.PlayerSettings;

[System.Serializable]
public class GunslingerAttackPattern : AttackPattern
{
    static int range=2;
    
    public override List<Vector3Int> AttackTilesVisual(Tilemap floor, Tilemap obstacles, Vector3Int position)
    {
        List<Vector3Int> tiles = new();

        for(int i = 0; i <= range; i++)
        {
            Vector3Int currentTile = new Vector3Int(position.x - i, position.y);
            if (currentTile==position)continue;

            tiles.Add(currentTile);
        }

        return tiles;
    }

  
}
