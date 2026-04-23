using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitGhost : MonoBehaviour
{
    Renderer objRenderer;
    SpriteRenderer spriteRenderer;
    public CharacterVisual owner;
    private void Awake()
    {
        objRenderer = gameObject.GetComponent<Renderer>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        objRenderer.material.SetFloat("_Alpha", 0.75f);
    }
    public void SetOwner(CharacterVisual owner)
    {
        this.owner = owner;
        SubscribeToUnit();
    }
    public void SubscribeToUnit()
    {
        Debug.Log("Ghost Owner: "+owner);
        owner.OnAnimUpdate += AnimUpdate;
    }
    public void UpdatePosition(Vector3 pos)
    {
        transform.position = pos;
    }
    public Vector3Int GetTilePos(Tilemap tilemap)
    {
        return tilemap.WorldToCell(transform.position);
    }
    public void AnimUpdate(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
    void Update()
    {
        
    }
}
