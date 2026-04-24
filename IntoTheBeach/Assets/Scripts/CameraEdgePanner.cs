using UnityEngine;

public class CameraEdgePanner : MonoBehaviour
{
    [Header("Pixel Perfect Settings")]
    [Tooltip("Match this to your game's global Pixels Per Unit (PPU) setting.")]
    [SerializeField] private int pixelsPerUnit = 32;

    [Header("Pan Settings")]
    [Tooltip("How fast the camera moves.")]
    [SerializeField] private float panSpeed = 5f;

    [Tooltip("How close the mouse needs to be to the edge (in pixels) to start panning.")]
    [SerializeField] private float edgeThickness = 3f;

    [Header("Pan Limits")]
    [Tooltip("The maximum distance the camera can travel to the Left (X) and Down (Y).")]
    [SerializeField] private Vector2 minPanLimit = new Vector2(-3f, -3f);

    [Tooltip("The maximum distance the camera can travel to the Right (X) and Up (Y).")]
    [SerializeField] private Vector2 maxPanLimit = new Vector2(3f, 3f);

    // We use this to track the smooth position behind the scenes
    private Vector2 internalPosition, startPos;
    private float initialZ;

    private void Start()
    {
        // Record our starting location so we don't snap wildly on frame 1
        internalPosition = transform.position;
        startPos = transform.position;
        initialZ = transform.position.z;
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        // Modify the smooth hidden position instead of the camera's actual transform
        if (mousePos.x >= Screen.width - edgeThickness)
            internalPosition.x += panSpeed * Time.deltaTime;

        if (mousePos.x <= edgeThickness)
            internalPosition.x -= panSpeed * Time.deltaTime;

        if (mousePos.y >= Screen.height - edgeThickness)
            internalPosition.y += panSpeed * Time.deltaTime;

        if (mousePos.y <= edgeThickness)
            internalPosition.y -= panSpeed * Time.deltaTime;

        // Clamp the hidden internal position
        internalPosition.x = Mathf.Clamp(internalPosition.x, startPos.x+minPanLimit.x, startPos.x+maxPanLimit.x);
        internalPosition.y = Mathf.Clamp(internalPosition.y, startPos.y + minPanLimit.y, startPos.y + maxPanLimit.y);

        // Snap the smooth position to the nearest exact pixel fraction!
        float snappedX = Mathf.Round(internalPosition.x * pixelsPerUnit) / pixelsPerUnit;
        float snappedY = Mathf.Round(internalPosition.y * pixelsPerUnit) / pixelsPerUnit;

        // Apply the newly snapped, pixel-perfect position to the camera
        transform.position = new Vector3(snappedX, snappedY, initialZ);
    }
}