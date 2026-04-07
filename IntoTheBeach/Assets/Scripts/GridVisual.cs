using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridVisual : MonoBehaviour, Iinteractable
{
    public Tilemap saloonTiles, obstacles;
    public static event Action OnUnitMoved;
    public List<Vector3Int> HighlightedTiles = new List<Vector3Int>();

    private void OnDisable()
    {
        UIActions.OnMovement -= HighlightMovableTiles;
        UIActions.OnAttack -= highlightAttackableTiles;
    }
    private void OnEnable()
    {
        UIActions.OnMovement += HighlightMovableTiles;
        UIActions.OnAttack += highlightAttackableTiles;
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
        if (InputManager.Instance.GetState() == InputManager.TurnState.Attacking) 
        {

        }
    }
 
    public void unitattack(Vector3Int targetTile) 
    {
        var currentSelection = InputManager.Instance.GetCurrentSelection();
        if (currentSelection != null) return;



    }
    public void MoveUnit(Vector3Int TargetPos) 
    {
        var currentSelection = InputManager.Instance.GetCurrentSelection();
        if (currentSelection == null) return;

        currentSelection.transform.position = saloonTiles.CellToWorld(TargetPos);
        currentSelection.hasMoved = true;
        InputManager.Instance.SetState(InputManager.TurnState.None);
        
        OnUnitMoved?.Invoke();

        ResetTiles();

    }
    private void ResetTiles() 
    {
        foreach (var tile in HighlightedTiles) 
        {
            saloonTiles.SetColor(tile, Color.white);
        }
    }
    public void highlightAttackableTiles() 
    {
        List<Vector3Int> highlightedTiles = new List<Vector3Int>();
        var currentSelection = InputManager.Instance.GetCurrentSelection();
        if (currentSelection == null) return;
        Vector3Int pos = currentSelection.GetTilePos(saloonTiles);

        List<Vector3Int> tilesToHighlight = AttackPattern.GetAllAttackHighlightTiles(currentSelection.unitClass.attackPattern.AttackTilesVisual(saloonTiles, obstacles, pos), pos, obstacles);
        foreach (var tile in tilesToHighlight)
        {
            saloonTiles.SetColor(tile, Color.darkGreen);

        }


    }
    public void HighlightMovableTiles()
    {
        List<Vector3Int> highlightedTiles = new List<Vector3Int>();
        var currentSelection = InputManager.Instance.GetCurrentSelection();
        if (currentSelection == null) return;
        Vector3Int pos = currentSelection.GetTilePos(saloonTiles);

        for(int x=pos.x-currentSelection.unitClass.moveRange; x <= pos.x + currentSelection.unitClass.moveRange; x++)
        {

            for (int y=pos.y-currentSelection.unitClass.moveRange;y<=pos.y + currentSelection.unitClass.moveRange; y++)
            {
                if(Math.Abs(x-pos.x)+Math.Abs(y-pos.y)>currentSelection.unitClass.moveRange) continue;
                if (Math.Abs(x - pos.x) + Math.Abs(y - pos.y) ==0) continue;
                highlightedTiles.Add(new Vector3Int(x, y, 0));
                saloonTiles.SetColor(new Vector3Int(x, y, 0), Color.darkGreen);
              
            }
        }
        HighlightedTiles = highlightedTiles;

        
    }

}
