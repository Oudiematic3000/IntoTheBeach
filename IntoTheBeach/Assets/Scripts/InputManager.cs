using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    [SerializeField] private CharacterVisual CurrentSelection;
    public bool teamExclusiveSelection;
   

    public static event Action OnClickNothing;

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
        CharacterVisual.OnClick += SetCurrentSelection;
        UIActions.OnMovement += enterMoveMode;
        UIActions.OnAttack += enterAttackMode;
       
    }
    private void OnDisable()
    {
        CharacterVisual.OnClick -= SetCurrentSelection;
        UIActions.OnMovement -= enterMoveMode;
        UIActions.OnAttack -= enterAttackMode;
        
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

   
    void Update()
    {
        HoverInteract();
        if (Input.GetMouseButtonDown(0))
        {
            PressInteract();
        }
    }
    public CharacterVisual GetCurrentSelection() 
    {
        return CurrentSelection;
    }
    public void SetCurrentSelection(CharacterVisual current) 
    {
        CurrentSelection = current;
        
    }
   

    private RaycastHit2D InteractMouse()
{
    int unitLayer = LayerMask.NameToLayer("Unit");
    int unitLayerMask = 1 << unitLayer;
    int invertedMaskInt = ~unitLayerMask;

    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
    
    if (currentState == TurnStates.Attacking)
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
    public void HoverInteract() 
    {
        RaycastHit2D ray = InteractMouse();

        if (ray.collider == null) return;

        if (ray.collider.TryGetComponent<Iinteractable>(out var hoverObject))
            {
                hoverObject.OnHover(ray.point);
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
            CurrentSelection = null;
                return;
            }
           
            if (ray.collider.TryGetComponent<Iinteractable>(out var hoverObject))
            {
                if (hoverObject==null) return;
            if (CurrentSelection)
            CurrentSelection.RemoveOutline();

            if (currentState == TurnStates.Attacking || currentState == TurnStates.Moving) 
            {
                if(ray.collider.GetComponent<CharacterVisual>()) return;
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

    
}