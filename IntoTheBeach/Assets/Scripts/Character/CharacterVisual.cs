using System;
using UnityEngine;
using UnityEngine.Tilemaps;


public class CharacterVisual : MonoBehaviour, Iinteractable
{
    public static event Action<CharacterVisual> OnClick;
    public UnitClass unitClass;
    public UnitGhost ghost;
    public bool hasMoved, hasAttacked;
    private Renderer objRenderer;
    private SpriteRenderer spriteRenderer;
    public int direction;


    private void Start()
    {
        objRenderer = GetComponent<Renderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        MovePlanTurnState.OnMovePlanStart += AnimUpdate;
        AttackPlanTurnState.OnAttackPlanStart += AnimUpdate;

    }
    private void OnDisable()
    {
        MovePlanTurnState.OnMovePlanStart -= AnimUpdate;
        AttackPlanTurnState.OnAttackPlanStart -= AnimUpdate;
    }
    public void OnHover(Vector2 mousePos)
    {
    }

    public void OnPress(Vector2 mousePos)
    {
        OnClick?.Invoke(this);
        var currentSelection = InputManager.Instance.GetCurrentSelection();
        currentSelection.ShowOutline();


    }
    public void AnimUpdate() 
    {

        UnitAnimations.AnimState state = TurnStateMachine.Instance.GetAnimState();

        spriteRenderer.sprite = unitClass.animations.GetSpriteAnim(state, direction);
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
        objRenderer.material.SetColor("_OutlineColour", Color.green);
    }
    public void RemoveOutline()
    {
        objRenderer.material.SetFloat("_OutlineThickness", 0f);
        objRenderer.material.SetColor("_OutlineColour", Color.black);
    }
}
