using System;
using UnityEngine;
using UnityEngine.Tilemaps;


public class CharacterVisual : MonoBehaviour, Iinteractable
{
    public static event Action<CharacterVisual> OnClick;
    public UnitClass unitClass;
    public UnitGhost ghost;
    public bool hasMoved, hasAttacked;
    Renderer objRenderer;
    private void Start()
    {
        objRenderer = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
       //MovePlanTurnState.OnMovePlanStart += InitTile;
    }
    public void OnHover(Vector2 mousePos)
    {
    }

    public void OnPress(Vector2 mousePos)
    {
        OnClick?.Invoke(this);
    }

   public Vector3Int GetTilePos(Tilemap tilemap)
    {
        return tilemap.WorldToCell(transform.position);
    }

    public UnitGhost GenerateMoveGhost(Vector3 pos)
    {
        var moveGhost = Instantiate(unitClass.unitGhost,pos,Quaternion.identity);
        return moveGhost.GetComponent<UnitGhost>();
    }

    public void ShowOutline()
    {
        objRenderer.material.SetFloat("_OutlineThickness", 1f);
        objRenderer.material.SetColor("_OutlineColour", Color.white);
    }

}
