using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridVisual : MonoBehaviour, Iinteractable
{
    public Tilemap saloonTiles;
    public static event Action OnUnitMoved;
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
        var currentSelection = InputManager.Instance.GetCurrentSelection();
        if (currentSelection == null) return;

        currentSelection.transform.position = saloonTiles.CellToWorld(TargetPos);
        currentSelection.hasMoved = true;
        InputManager.Instance.SetState(InputManager.TurnState.None);
        OnUnitMoved?.Invoke();
    }


}
