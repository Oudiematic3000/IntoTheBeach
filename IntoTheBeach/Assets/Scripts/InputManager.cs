using UnityEngine;

public class InputManager : MonoBehaviour
{
    
    void Start()
    {
        
    }
    void Update()
    {
        HoverInteract();
    }
    public void HoverInteract() 
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint( Input.mousePosition );
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider.TryGetComponent<Iinteractable>(out var hoverObject)) 
        {
            hoverObject.OnHover(mousePos2D);
        }
    }
    public void PressInteract() 
    {

    }
}
public interface Iinteractable
{
    public void OnHover(Vector2 mousePos);
    public void OnPress(Vector2 mousePos);

    
}