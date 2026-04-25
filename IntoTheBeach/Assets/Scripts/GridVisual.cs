using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class GridVisual : MonoBehaviour, Iinteractable
{
    public Tilemap saloonTiles, obstacles;
    public static event Action OnUnitMoved, OnUnitAttacked;
    public static event Action onMoveText;
    public static event Action OnGridClick;
    public static event Action OnResetPip;
    public List<Vector3Int> HighlightedTiles = new List<Vector3Int>();
    public List<Vector3Int> LockedAttackTiles = new List<Vector3Int>();
   
    //CharacterVisual selectedUnit; TODO: Use event to update this rather than referencing singleton.
    UnitGhost ghost;

    private void Awake()
    {
      
    }
    private void OnEnable()
    {
        UIActions.OnMovement += HighlightMovableTiles;
        
        UIActions.OnAttack += HighlightAttackableTiles;
        StandbyTurnState.OnStandbyStart += ResetTiles;
        InputManager.OnRemove += RemoveGhost;
    }
    private void OnDisable()
    {
        UIActions.OnMovement -= HighlightMovableTiles;
        UIActions.OnAttack -= HighlightAttackableTiles;
        StandbyTurnState.OnStandbyStart -= ResetTiles;
        
        InputManager.OnRemove -= RemoveGhost;
    }
    
    public static void resetPip() 
    {
        OnResetPip?.Invoke();
    }
    public void OnHover(Vector2 mousePos)
    {
        Vector3Int tilePos = saloonTiles.WorldToCell(mousePos);
        Vector3 worldPos = saloonTiles.CellToWorld(tilePos);
        var currentSelection = InputManager.Instance.GetCurrentSelection();
        //print(tilePos);

        if (InputManager.Instance.GetState() == InputManager.TurnStates.Moving)
        {
            if(!ghost) return;
            if (!HighlightedTiles.Contains(tilePos)) return;
            ghost.UpdatePosition(worldPos);
            currentSelection.direction = InputManager.Instance.GetCursorDirectionFromCharacter(currentSelection, saloonTiles);
            currentSelection.AnimUpdate();
            
        }
        if (InputManager.Instance.GetState() == InputManager.TurnStates.Attacking)
        {
            Vector3Int pos = currentSelection.GetTilePos(saloonTiles);
            if (currentSelection.ghost) pos = currentSelection.ghost.GetTilePos(saloonTiles);
            int direction = InputManager.Instance.GetCursorDirectionFromCharacter(pos, saloonTiles);

            List<List<Vector3Int>> directionSeperatedList = AttackPattern.GetDirectionSeparatedList(currentSelection.unitClass.attackPattern.AttackTilesVisual(saloonTiles, obstacles, pos), pos);

            if (!directionSeperatedList[direction].Contains(tilePos))
            {
                foreach (var tile in HighlightedTiles) saloonTiles.SetColor(tile, Color.darkRed);

                return;
            }
            foreach (var tile in directionSeperatedList[direction])
            {
                saloonTiles.SetColor(tile, Color.darkGreen);

            }

        }
    }
    public List<Vector3Int> GetDirectionedAttackTiles(Vector3Int tilePos)
    {
       
        var currentSelection = InputManager.Instance.GetCurrentSelection();

        Vector3Int pos = currentSelection.GetTilePos(saloonTiles);
        if (currentSelection.ghost) pos = currentSelection.ghost.GetTilePos(saloonTiles);
        int direction = InputManager.Instance.GetCursorDirectionFromCharacter(pos, saloonTiles);

        List<List<Vector3Int>> directionSeperatedList = AttackPattern.GetDirectionSeparatedList(currentSelection.unitClass.attackPattern.AttackTilesVisual(saloonTiles, obstacles, pos), pos);


        if (directionSeperatedList[direction].Contains(tilePos))
        {
            return directionSeperatedList[direction];
        }
        else
        {
            return new List<Vector3Int>();
        }


    }
    public void OnPress(Vector2 mousePos)
    {
        Vector3Int tilePos = saloonTiles.WorldToCell(mousePos);

        var currentSelection = InputManager.Instance.GetCurrentSelection();
        if (currentSelection == null) return;

        if (InputManager.Instance.GetState() == InputManager.TurnStates.Moving)
        {
            MoveUnit(tilePos);
        }
        else if (InputManager.Instance.GetState() == InputManager.TurnStates.Attacking)
        {
            Unitattack(tilePos);
        }
        else
        {

            OnGridClick?.Invoke();
            InputManager.Instance.SetCurrentSelection(null);
        }
    }
 
    public void Unitattack(Vector3Int tilePos) 
    {
        var currentSelection = InputManager.Instance.GetCurrentSelection();
        if (currentSelection == null) return;
        if (!HighlightedTiles.Contains(tilePos))
        {
            OnGridClick?.Invoke();
            return;
        }
        Vector3Int pos = currentSelection.GetTilePos(saloonTiles);
        if (currentSelection.ghost) pos = currentSelection.ghost.GetTilePos(saloonTiles);
        int direction = InputManager.Instance.GetCursorDirectionFromCharacter(pos, saloonTiles);
        var attack= GetDirectionedAttackTiles(tilePos);
        if(attack.Count == 0) return;
        foreach (var tile in attack)
        {
            saloonTiles.SetColor(tile, Color.darkRed);
            LockedAttackTiles.Add(tile);
        }

        currentSelection.hasAttacked = true;
        currentSelection.direction = direction;
        currentSelection.AnimUpdate();
        TurnStateMachine.Instance.currentTurnInfo.IncrementAttackCount(1);
        AttackAction attackAction = new AttackAction(currentSelection.GetTilePos(saloonTiles), currentSelection.unitClass.attackPattern, direction, currentSelection.unitID);
        TurnStateMachine.Instance.currentTurnInfo.turnPlan.ModifyUnitPlanAttackAction(currentSelection.unitID, attackAction);
        ResetTiles();
        OnUnitAttacked?.Invoke();
        currentSelection.RemoveOutline();
        OnGridClick?.Invoke();
        InputManager.Instance.SetState(InputManager.TurnStates.None);

    }
    public void MoveUnit(Vector3Int TargetPos) 
    {
        var currentSelection = InputManager.Instance.GetCurrentSelection();
      
        if (currentSelection == null) return;
        if (!HighlightedTiles.Contains(TargetPos)) 
        {
            OnGridClick?.Invoke();
            return;
        }

        // currentSelection.transform.position = saloonTiles.CellToWorld(TargetPos);

        TurnStateMachine.Instance.currentTurnInfo.ghosts.Add(ghost);
        currentSelection.hasMoved = true;
        Debug.Log("DIRECTION: "+GetDirection(currentSelection.GetTilePos(saloonTiles), ghost.GetTilePos(saloonTiles)));
        currentSelection.direction=GetDirection(currentSelection.GetTilePos(saloonTiles), ghost.GetTilePos(saloonTiles));
        currentSelection.AnimUpdate(); 
        TurnStateMachine.Instance.currentTurnInfo.IncrementMoveCount(1);
        InputManager.Instance.SetState(InputManager.TurnStates.None);
        
        OnUnitMoved?.Invoke();
        MoveAction moveAction = new MoveAction(currentSelection.GetTilePos(saloonTiles),ghost.GetTilePos(saloonTiles));
        TurnStateMachine.Instance.currentTurnInfo.turnPlan.ModifyUnitPlanMoveAction(currentSelection.unitID, moveAction);
        ResetTiles();
        currentSelection.RemoveOutline();
       onMoveText?.Invoke();
    }
    private void ResetTiles()
    {
        bool isAttackState = TurnStateMachine.Instance.currentState is AttackPlanTurnState;

        for (int i = HighlightedTiles.Count - 1; i >= 0; i--)
        {
            var tile = HighlightedTiles[i];

            if (isAttackState && LockedAttackTiles.Contains(tile))
            {
                continue;
            }

            saloonTiles.SetColor(tile, Color.white);
            HighlightedTiles.RemoveAt(i);
        }

        if (!isAttackState)
        {
            foreach (var lockedTile in LockedAttackTiles)
            {
                saloonTiles.SetColor(lockedTile, Color.white);
            }

            LockedAttackTiles.Clear();
        }

        if (ghost)
        {
            ghost = null;
        }
    }
    public void RemoveGhost() 
    {
        bool isAttackState = TurnStateMachine.Instance.currentState is AttackPlanTurnState;

        for (int i = HighlightedTiles.Count - 1; i >= 0; i--)
        {
            var tile = HighlightedTiles[i];

            if (isAttackState && LockedAttackTiles.Contains(tile))
            {
                continue;
            }

            saloonTiles.SetColor(tile, Color.white);
            HighlightedTiles.RemoveAt(i);
        }

        if (!isAttackState)
        {
            foreach (var lockedTile in LockedAttackTiles)
            {
                saloonTiles.SetColor(lockedTile, Color.white);
            }

            LockedAttackTiles.Clear();
        }

        if (ghost)
        {  
            Destroy(ghost.gameObject);  
            ghost = null;
           
        }
        if (ghost) 
        {
            
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
            saloonTiles.SetColor(tile, Color.darkRed);
            highlightedTiles.Add(tile);
        }
        HighlightedTiles = highlightedTiles;

    }
    public void HighlightMovableTiles()
    {   

        List<Vector3Int> highlightedTiles = new List<Vector3Int>();
        var currentSelection = InputManager.Instance.GetCurrentSelection();
        if (currentSelection == null) return;
        Vector3Int pos = currentSelection.GetTilePos(saloonTiles);
        ghost = InputManager.Instance.GetCurrentSelection().GenerateMoveGhost(pos);
        currentSelection.ghost = ghost;
        ghost.SetOwner(currentSelection);

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
