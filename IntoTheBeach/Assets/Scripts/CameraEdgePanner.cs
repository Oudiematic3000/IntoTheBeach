using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraEdgePanner : MonoBehaviour
{
  
    [SerializeField] private int pixelsPerUnit = 32;


    [SerializeField] private float panSpeed = 5f;

    [SerializeField] private float edgeThickness = 3f;

    
    [SerializeField] private Vector2 minPanLimit = new Vector2(-3f, -3f);

    [SerializeField] private Vector2 maxPanLimit = new Vector2(3f, 3f);

    private Vector2 internalPosition, startPos;
    private float initialZ;
    bool locked = false;

    private void Start()
    {
        internalPosition = transform.position;
        startPos = transform.position;
        initialZ = transform.position.z;
    }

    private void Update()
    {
        if (locked) return;
        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x >= Screen.width - edgeThickness)
            internalPosition.x += panSpeed * Time.deltaTime;

        if (mousePos.x <= edgeThickness)
            internalPosition.x -= panSpeed * Time.deltaTime;

        if (mousePos.y >= Screen.height - edgeThickness)
            internalPosition.y += panSpeed * Time.deltaTime;

        if (mousePos.y <= edgeThickness)
            internalPosition.y -= panSpeed * Time.deltaTime;

        internalPosition.x = Mathf.Clamp(internalPosition.x, startPos.x+minPanLimit.x, startPos.x+maxPanLimit.x);
        internalPosition.y = Mathf.Clamp(internalPosition.y, startPos.y + minPanLimit.y, startPos.y + maxPanLimit.y);

        float snappedX = Mathf.Round(internalPosition.x * pixelsPerUnit) / pixelsPerUnit;
        float snappedY = Mathf.Round(internalPosition.y * pixelsPerUnit) / pixelsPerUnit;

        transform.position = new Vector3(snappedX, snappedY, initialZ);
    }

    public void PanToTile(Vector3Int tilePos, Tilemap tilemap)
    {
        Vector3 worldPos = tilemap.CellToWorld(tilePos);
        internalPosition.x = Mathf.Clamp(worldPos.x, startPos.x + minPanLimit.x, startPos.x + maxPanLimit.x);
        internalPosition.y = Mathf.Clamp(worldPos.y, startPos.y + minPanLimit.y, startPos.y + maxPanLimit.y);
        float snappedX = Mathf.Round(internalPosition.x * pixelsPerUnit) / pixelsPerUnit;
        float snappedY = Mathf.Round(internalPosition.y * pixelsPerUnit) / pixelsPerUnit;
        LeanTween.move(gameObject, new Vector3(snappedX, snappedY, initialZ), 0.2f).setEase(LeanTweenType.easeInOutCirc);
    }
    public void PanToCenter(Tilemap tilemap)
    {
        internalPosition.x = Mathf.Clamp(startPos.x, startPos.x + minPanLimit.x, startPos.x + maxPanLimit.x);
        internalPosition.y = Mathf.Clamp(startPos.y, startPos.y + minPanLimit.y, startPos.y + maxPanLimit.y);
        float snappedX = Mathf.Round(internalPosition.x * pixelsPerUnit) / pixelsPerUnit;
        float snappedY = Mathf.Round(internalPosition.y * pixelsPerUnit) / pixelsPerUnit;
        LeanTween.move(gameObject, new Vector3(snappedX, snappedY, initialZ), 0.2f).setEase(LeanTweenType.easeInOutCirc);
    }
    public void ToggleLockAndCenter()
    {
        locked = !locked;
    }
}