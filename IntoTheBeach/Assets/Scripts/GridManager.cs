using UnityEngine;
using Unity.Netcode;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridManager : NetworkBehaviour
{
    public Tilemap saloonTiles;
    public static GridManager instance;
    public Dictionary<Vector3Int, TileData> tileDic = new Dictionary<Vector3Int, TileData>();

    public override void OnNetworkSpawn()
    {
        
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
       
    }

    private void PopulateDictionary()
    {
        tileDic.Clear();

        foreach(Vector3Int tile in saloonTiles.cellBounds.allPositionsWithin)
        {
            if (!saloonTiles.HasTile(tile)) continue;
            tileDic[tile] = new TileData(null);
        }


    }


}

public class TileData 
{
    public Entity occupant;
    public TileData(Entity o)
    {
        occupant = o;
    }
   
}
