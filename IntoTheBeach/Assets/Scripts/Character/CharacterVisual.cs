using System;
using UnityEngine;
using UnityEngine.Tilemaps;


public class CharacterVisual : MonoBehaviour, Iinteractable
{
    public static event Action<CharacterVisual> OnClick;
    public int moveRange=1;
    public int attackRange = 3;
    public bool hasMoved, hasAttacked;

    private void OnEnable()
    {
       // MovePlanTurnState.OnMovePlanStart += InitTile;
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

    //public void InitTile()
    //{

    //    Tilemap tilemap = GameObject.Find("FloorVisual").GetComponent<Tilemap>();

    //    Grid grid = tilemap.layoutGrid;
    //    Vector3Int cellPos = grid.WorldToCell(transform.position);
    //    cellPos.z = 0;
    //    Vector3 snappedPos = grid.WorldToCell(cellPos);
    //    transform.position = snappedPos;
    //    Debug.Log(transform.position);
    //}

}
