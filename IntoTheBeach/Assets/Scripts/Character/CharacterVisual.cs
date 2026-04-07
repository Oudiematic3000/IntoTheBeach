using System;
using UnityEngine;
using UnityEngine.Tilemaps;


public class CharacterVisual : MonoBehaviour, Iinteractable
{
    public static event Action<CharacterVisual> OnClick;
    public int moveRange=1;
    public bool hasMoved, hasAttacked;

    private void OnEnable()
    {
        MovePlanTurnState.OnMovePlanStart += InitTile;
    }
    public void OnHover(Vector2 mousePos)
    {
        print("My mom");
    }

    public void OnPress(Vector2 mousePos)
    {
        OnClick?.Invoke(this);
    }

   public Vector3Int GetTilePos(Tilemap tilemap)
    {
        return tilemap.WorldToCell(transform.position);
    }

    public void InitTile()
    {
        Tilemap tilemap = FindFirstObjectByType<Tilemap>();
        transform.position = tilemap.WorldToCell(transform.position);
    }

}
