using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridVisual : MonoBehaviour, Iinteractable
{
    public Tilemap saloonTiles;
    public static event Action OnUnitMoved;

    private void OnDisable()
    {
        UIActions.OnMovement -= HighlightMovableTiles;
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
    public List<Vector3Int> HighlightMovableTiles()
    {
        List<Vector3Int> highlightedTiles = new List<Vector3Int>();
        var currentSelection = InputManager.Instance.GetCurrentSelection();
        if (currentSelection == null) return highlightedTiles;
        Vector3Int pos = currentSelection.GetTilePos(saloonTiles);

        for(int x=pos.x-currentSelection.moveRange; x <= pos.x + currentSelection.moveRange; x++)
        {

            for (int y=pos.y-currentSelection.moveRange;y<=pos.y + currentSelection.moveRange; y++)
            {
                if(Math.Abs(x-pos.x)+Math.Abs(y-pos.y)>currentSelection.moveRange) continue;
                if (Math.Abs(x - pos.x) + Math.Abs(y - pos.y) ==0) continue;
                highlightedTiles.Add(pos);
                saloonTiles.SetColor(new Vector3Int(x, y, 0), Color.rebeccaPurple);
              
            }
        }
        return highlightedTiles;
    }

}
