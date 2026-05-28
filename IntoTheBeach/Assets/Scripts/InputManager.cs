using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    [SerializeField] private CharacterVisual CurrentSelection;
    public bool teamExclusiveSelection;
   

    public static event Action OnClickNothing;
    public static event Action OnRemove;
    public static event Action Pause;


    [SerializeField] private TurnStates currentState = TurnStates.None;
    public enum TurnStates
    {
        None,
        Moving,
        Attacking,
    }
    public TurnStates GetState() 
    {
        return currentState;
    }
    public void SetState(TurnStates state) 
    {
        this.currentState = state;
    }
    private void Awake()
    {
        InputManager.Instance = this;
        
    }
   
    private void OnEnable()
    {
        MovePlanTurnState.OnMovePlanStart += pressUnselect;
        AttackPlanTurnState.OnAttackPlanStart += pressUnselect;
        CharacterVisual.OnClick += SetCurrentSelection;
        UIActions.OnMovement += enterMoveMode;
        UIActions.OnAttack += enterAttackMode;
        GridVisual.OnGridClick += pressUnselect;
    }
    private void OnDisable()
    {
        MovePlanTurnState.OnMovePlanStart -= pressUnselect;
        AttackPlanTurnState.OnAttackPlanStart -= pressUnselect;
        CharacterVisual.OnClick -= SetCurrentSelection;
        UIActions.OnMovement -= enterMoveMode;
        UIActions.OnAttack -= enterAttackMode;
        GridVisual.OnGridClick -= pressUnselect;
    }
    private void enterMoveMode()
    {
      
        if (CurrentSelection == null) return;
        currentState = TurnStates.Moving;
    }
    private void enterAttackMode() 
    { 
      
      if(CurrentSelection == null) return;
      currentState = TurnStates.Attacking;
    }

    private GameObject lastSelected;
    void Update()
    {
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected != lastSelected)
        {
            Debug.Log($"Selection changed: {lastSelected?.name} to {selected?.name}", selected);
            lastSelected = selected;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            LineGenerator.Instance.engaged = true;
            return;
        }
        LineGenerator.Instance.engaged = false;
        HoverInteract();
        if (Input.GetMouseButtonDown(0))
        {
            PressInteract();
        }
        if (Input.GetMouseButtonDown(1)) 
        {
            if (FindAnyObjectByType<Tutorial>())
            {
                var tutorial = FindAnyObjectByType<Tutorial>();
                if (tutorial.currentPhase != Tutorial.TutorialPhases.ViewAttackRange)
                    return;
            }
            pressUnselect();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            Pause?.Invoke();
            print("Is Pausing");
        }
    }

    public void pressUnselect() 
    {
       
        if(CurrentSelection)
        CurrentSelection.RemoveOutline();
        CurrentSelection = null;
        currentState = TurnStates.None;
        
        OnRemove?.Invoke();
    }
    public CharacterVisual GetCurrentSelection() 
    {
        return CurrentSelection;
    }
    public void SetCurrentSelection(CharacterVisual current) 
    {
        if(current==CurrentSelection)return;
        if(TurnStateMachine.Instance.currentTurnInfo.GetMoveCount() <= 0|| TurnStateMachine.Instance.currentTurnInfo.GetAttackCount() <= 0)
        CurrentSelection = current;
        AudioManager.instance.PlaySFX(current.unitClass.GetRandomSelectSound());
    }

    private bool IsPointerOverPauseUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.layer == LayerMask.NameToLayer("Pause"))
            {
                return true;
            }
        }

        return false;
    }
    private RaycastHit2D InteractMouse()
    {
        if(IsPointerOverPauseUI())return new RaycastHit2D();
    int unitLayer = LayerMask.NameToLayer("Unit");
    int furnitureLayer = LayerMask.NameToLayer("Furniture");

        int unitLayerMask = 1 << unitLayer;
    int invertedMaskInt = ~unitLayerMask;
        unitLayerMask &= ~(1<<furnitureLayer);
        int furnitureLayerMask = 1 << furnitureLayer;
        int invertedfurnitureLayer = ~furnitureLayer;
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
     

    if (currentState == TurnStates.Attacking || currentState == TurnStates.Moving)
    {
        return Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, invertedMaskInt);
    }
    else
    {
        RaycastHit2D unitHit = Physics2D.Raycast(mousePos2D, Vector2.zero, Mathf.Infinity, unitLayerMask);
        
        if (unitHit.collider != null)
        {
            return unitHit;
        }

        return Physics2D.Raycast(mousePos2D, Vector2.zero);
    }
    }
    private Iinteractable currentHovered;

    public void HoverInteract()
    {
        RaycastHit2D ray = InteractMouse();

        Iinteractable newHovered = null;
        if (ray.collider != null && ray.collider.TryGetComponent<Iinteractable>(out var hoverObject))
        {
            newHovered = hoverObject;
        }

        if (newHovered != currentHovered)
        {
            currentHovered?.OnEndHover();
            newHovered?.OnHover(ray.point);
            currentHovered = newHovered;
        }
        else if (currentHovered != null)
        {
            currentHovered.OnHover(ray.point);
        }
    }
    public void PressInteract() 
    { 
        RaycastHit2D ray = InteractMouse();


            if (EventSystem.current.IsPointerOverGameObject())
            { 
                 return;
            }
            if (ray.collider == null)
            {
                OnClickNothing?.Invoke();
                 pressUnselect();
                return;
            }
           
            if (ray.collider.TryGetComponent<Iinteractable>(out var hoverObject))
            {
                if (hoverObject==null) return;
           
            if (ray.collider.GetComponent<CharacterVisual>())
            {
                pressUnselect();
                if (ray.collider.GetComponent<CharacterVisual>().teamIndex != PlayerData.Local.TeamIndex.Value)
                {
                    if(teamExclusiveSelection)
                    return; 
                }
                
            }

            hoverObject.OnPress(ray.point);
            }
     
    }
    public int GetCursorDirectionFromCharacter(CharacterVisual character, Tilemap tilemap)
    {
        var point = InteractMouse().point;

        Vector3Int characterCell = character.GetTilePos(tilemap);
        Vector3Int mouseCell = tilemap.WorldToCell(point);

        int dx = mouseCell.x - characterCell.x;
        int dy = mouseCell.y - characterCell.y;

        if (Mathf.Abs(dx) >= Mathf.Abs(dy))
        {
            return dx < 0 ? 0 : 2; 
        }
        else
        {
            return dy < 0 ? 3 : 1;
        }
    }
    public int GetCursorDirectionFromCharacter(Vector3Int pos, Tilemap tilemap)
    {
        var point = InteractMouse().point;

        Vector3Int characterCell = pos;
        Vector3Int mouseCell = tilemap.WorldToCell(point);

        int dx = mouseCell.x - characterCell.x;
        int dy = mouseCell.y - characterCell.y;

        if (Mathf.Abs(dx) >= Mathf.Abs(dy))
        {
            return dx < 0 ? 0 : 2;
        }
        else
        {
            return dy < 0 ? 3 : 1;
        }
    }
}
public interface Iinteractable
{
    public void OnHover(Vector2 mousePos);
    public void OnPress(Vector2 mousePos);

    public void OnEndHover();
}