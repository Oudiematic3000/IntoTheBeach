using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private void Awake()
    {
        InputManager.Instance = this;
    }
    private CharacterVisual CurrentSelection;
    private void OnEnable()
    {
        CharacterVisual.OnClick += SetCurrentSelection;
    }
    private void OnDisable()
    {
        CharacterVisual.OnClick -= SetCurrentSelection;
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
        this.CurrentSelection = current;
        
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
      
           
            if (ray.collider.TryGetComponent<Iinteractable>(out var hoverObject))
            {
                hoverObject.OnPress(ray.point);
            }
        
    }
}
public interface Iinteractable
{
    public void OnHover(Vector2 mousePos);
    public void OnPress(Vector2 mousePos);

    
}