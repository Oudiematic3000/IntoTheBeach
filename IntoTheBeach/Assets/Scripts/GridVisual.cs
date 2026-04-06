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
        Vector3Int tilePos = saloonTiles.WorldToCell(mousePos);
        print("pressed " + tilePos);
        if (InputManager.Instance.GetState() == InputManager.TurnState.Moving) 
        {
            MoveUnit(tilePos);
        }
    }
    void Update()
    {
        
    }
    public void MoveUnit(Vector3Int TargetPos) 
    {
        var currentState = InputManager.Instance.GetCurrentSelection();
        if (currentState == null) return;

        currentState.transform.position = saloonTiles.CellToWorld(TargetPos);

        InputManager.Instance.SetState(InputManager.TurnState.None);
    }


}
