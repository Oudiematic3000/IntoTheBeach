using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitGhost : MonoBehaviour
{
    Renderer objRenderer;
    private void Awake()
    {
        objRenderer = gameObject.GetComponent<Renderer>();
    }
    void Start()
    {
        objRenderer.material.SetFloat("_Alpha", 0.75f);
    }
    public void UpdatePosition(Vector3 pos)
    {
        transform.position = pos;
    }
    public Vector3Int GetTilePos(Tilemap tilemap)
    {
        return tilemap.WorldToCell(transform.position);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
