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
    public UIActions Actions;
    //CharacterVisual selectedUnit; TODO: Use event to update this rather than referencing singleton.
    UnitGhost ghost;

    private void OnDisable()
    {
        UIActions.OnMovement -= HighlightMovableTiles;
        UIActions.OnAttack -= HighlightAttackableTiles;
    }
    private void OnEnable()
    {
        UIActions.OnMovement += HighlightMovableTiles;
        UIActions.OnAttack += HighlightAttackableTiles;
    }
    public void OnHover(Vector2 mousePos)
    {
        Vector3Int tilePos = saloonTiles.WorldToCell(mousePos);
        Vector3 worldPos = saloonTiles.CellToWorld(tilePos);
        if (InputManager.Instance.GetState() == InputManager.TurnState.Moving)
        {
            if(!ghost) return;
            if (!HighlightedTiles.Contains(tilePos)) return;
            ghost.UpdatePosition(worldPos);
        }

        //print(tilePos);
    }

    public void OnPress(Vector2 mousePos)
    {
        Vector3Int tilePos = saloonTiles.WorldToCell(mousePos);
        var currentSelection = InputManager.Instance.GetCurrentSelection();
       
        print("pressed " + tilePos);
        if (InputManager.Instance.GetState() == InputManager.TurnState.None )
        {

            Actions.HideAll();
        }
        if (InputManager.Instance.GetState() == InputManager.TurnState.Moving) 
        {
           
            MoveUnit(tilePos);
        }
        if (InputManager.Instance.GetState() == InputManager.TurnState.Attacking) 
        {

        }
       
    }
 
    public void Unitattack(Vector3Int targetTile) 
    {
        var currentSelection = InputManager.Instance.GetCurrentSelection();
        if (currentSelection != null) return;



    }
    public void MoveUnit(Vector3Int TargetPos) 
    {
        var currentSelection = InputManager.Instance.GetCurrentSelection();
      
        if (currentSelection == null) return;
        if (!HighlightedTiles.Contains(TargetPos)) return;

       // currentSelection.transform.position = saloonTiles.CellToWorld(TargetPos);
        currentSelection.ghost=ghost;
        TurnStateMachine.Instance.currentTurnInfo.ghosts.Add(ghost);
        currentSelection.hasMoved = true;
        Debug.Log("DIRECTION: "+GetDirection(currentSelection.GetTilePos(saloonTiles), ghost.GetTilePos(saloonTiles)));
        currentSelection.direction=GetDirection(currentSelection.GetTilePos(saloonTiles), ghost.GetTilePos(saloonTiles));
        currentSelection.AnimUpdate();
        TurnStateMachine.Instance.currentTurnInfo.IncrementMoveCount(1);
        InputManager.Instance.SetState(InputManager.TurnState.None);
        
        OnUnitMoved?.Invoke();

        ResetTiles();
        currentSelection.RemoveOutline();

    }
    private void ResetTiles() 
    {
        foreach (var tile in HighlightedTiles) 
        {
            saloonTiles.SetColor(tile, Color.white);
        }
        HighlightedTiles.Clear();
        if (ghost)
        {
            ghost=null;
        }
    }
    public void HighlightAttackableTiles() 
    {
        List<Vector3Int> highlightedTiles = new List<Vector3Int>();
        var currentSelection = InputManager.Instance.GetCurrentSelection();
        if (currentSelection == null) return;
        Vector3Int pos = currentSelection.GetTilePos(saloonTiles);

        if (currentSelection.ghost)
        {
            pos = currentSelection.ghost.GetTilePos(saloonTiles);
        }

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
        ghost = InputManager.Instance.GetCurrentSelection().GenerateMoveGhost(pos);

        for (int x=pos.x-currentSelection.unitClass.moveRange; x <= pos.x + currentSelection.unitClass.moveRange; x++)
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

    public int GetDirection(Vector3Int from, Vector3Int to)
    {
        //int xDistance = to.x - from.x;
        //int yDistance = to.y - from.y;

        //int move1Distance, move2Distance;
        //move1Distance = Math.Min(xDistance, yDistance);
        //move2Distance = Math.Max(xDistance, yDistance);

        //Vector3Int move1, move2;
        //Vector2 move2Normalised = Vector2.zero;

        //if (xDistance == move1Distance)
        //{
        //    move1 = new Vector3Int(move1Distance, 0, 0);
        //    move2 = new Vector3Int(0, move2Distance, 0);
        //    move2Normalised = new Vector2(move2.x, move2.y).normalized;

        //}
        //else if (yDistance == move1Distance) 
        //{
        //    move1 = new Vector3Int(0, move1Distance, 0);
        //    move2 = new Vector3Int(move2Distance, 0, 0);
        //    move2Normalised = new Vector2(move2.x, move2.y).normalized;

        //}

        //if (move2Normalised == Vector2.left) return 0;
        //if(move2Normalised == Vector2.up) return 1;
        //if (move2Normalised == Vector2.right) return 2;
        //if(move2Normalised == Vector2.down) return 3;
        //else return 0;

        Vector3Int dir = to - from;

       
        int x = Math.Sign(dir.x);
        int y = Math.Sign(dir.y);


        if (Math.Abs(dir.x) > Math.Abs(dir.y))
        {
            return x > 0 ? 2 : 0; 
        }
        else
        {
            return y > 0 ? 1 : 3; 
        }
    }

}
