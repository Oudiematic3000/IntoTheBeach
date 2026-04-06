using UnityEngine;
using UnityEngine.Tilemaps;

public class GridVisual : MonoBehaviour, Iinteractable
{
    public Tilemap saloonTiles;

    public void OnHover(Vector2 mousePos)
    {
        Vector3Int tilePos = saloonTiles.WorldToCell(mousePos);
        print(tilePos);
    }

    public void OnPress(Vector2 mousePos)
    {
        throw new System.NotImplementedException();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }


}
