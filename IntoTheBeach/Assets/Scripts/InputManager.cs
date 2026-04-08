using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    [SerializeField] private CharacterVisual CurrentSelection;

    public static event Action OnClickNothing;

    [SerializeField] private TurnState currentState = TurnState.None;
    public enum TurnState
    {
        None,
        Moving,
        Attacking,
    }
    public TurnState GetState() 
    {
        return currentState;
    }
    public void SetState(TurnState state) 
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
        currentState = TurnState.Moving;
    }
    private void enterAttackMode() 
    { 
      if(CurrentSelection == null) return;
      currentState = TurnState.Attacking;
    }

    //Update method
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
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        return hit;
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

                hoverObject.OnPress(ray.point);
            }
        
    }
}
public interface Iinteractable
{
    public void OnHover(Vector2 mousePos);
    public void OnPress(Vector2 mousePos);

    
}