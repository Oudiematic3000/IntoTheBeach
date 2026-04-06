using UnityEngine;
using UnityEngine.Tilemaps;

public class GridVisual : MonoBehaviour, Iinteractable
{
    public Tilemap saloonTiles;
    private TurnState currentState = TurnState.Moving;
    public enum TurnState
    {
        None,
        Moving,
        Attacking,
    }
    public void OnHover(Vector2 mousePos)
    {
        Vector3Int tilePos = saloonTiles.WorldToCell(mousePos);
        print(tilePos);
    }

    public void OnPress(Vector2 mousePos)
    {
        Vector3Int tilePos = saloonTiles.WorldToCell(mousePos);
        print("pressed " + tilePos);
        if (currentState == TurnState.Moving) 
        {
            MoveUnit(tilePos);
        }
    }
    void Update()
    {
        
    }
    public void MoveUnit(Vector3Int TargetPos) 
    {
        InputManager.Instance.GetCurrentSelection().transform.position = saloonTiles.CellToWorld(TargetPos);
    }


}
