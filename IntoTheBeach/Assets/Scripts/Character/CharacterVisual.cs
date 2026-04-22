using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class CharacterVisual : MonoBehaviour, Iinteractable
{
    public static event Action OnMoveSelected;
    public static event Action OnAttackSelected;
    public static event Action<CharacterVisual> OnClick;
    public UnitClass unitClass;
    public UnitGhost ghost;
    public bool hasMoved, hasAttacked;
    private Renderer objRenderer;
    private SpriteRenderer spriteRenderer;
    public int direction;
    private UIActions actions;
    private TurnStateMachine turnState;
    public int unitID;

    private void Start()
    {
        objRenderer = GetComponent<Renderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        actions = FindAnyObjectByType<UIActions>();
         turnState = FindAnyObjectByType<TurnStateMachine>();

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


        if (turnState.currentState is MovePlanTurnState)
        {
            OnMoveSelected?.Invoke();
        }
        if (turnState.currentState is AttackPlanTurnState) 
        {
            OnAttackSelected?.Invoke();
        }
            
       
        
        
            currentSelection.ShowOutline();


    }
    public void ExecuteMovement(List<path> paths, Tilemap tilemap, Action onComplete = null)
    {
        StartCoroutine(MoveRoutine(paths, tilemap, onComplete));
    }

    private IEnumerator MoveRoutine(List<path> paths, Tilemap tilemap, Action onComplete)
    {
        int directionBeforeCollision = direction;

        foreach (var step in paths)
        {
            if (step.moveType == path.MoveType.walk)
            {
                yield return StartCoroutine(WalkStep(step, tilemap));
                directionBeforeCollision = direction;
            }
            else if (step.moveType == path.MoveType.collision)
            {
                yield return StartCoroutine(CollisionStep(step, directionBeforeCollision, tilemap));
            }
        }

        UpdateStepAnimation(false);
        onComplete?.Invoke();
    }

    private IEnumerator WalkStep(path step, Tilemap tilemap)
    {
        Vector3 startPos = transform.position;
        Vector3Int currentCell = tilemap.WorldToCell(startPos);
        Vector3 targetPos = tilemap.CellToWorld(currentCell + step.move);

        SetDirection(step.move);

        float duration = 1f;
        float time = 0f;
        float animTimer = 0f;
        bool useWalkSprite = true;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, time / duration);

            animTimer += Time.deltaTime;
            if (animTimer >= 0.5f)
            {
                useWalkSprite = !useWalkSprite;
                animTimer = 0f;
                UpdateStepAnimation(useWalkSprite);
            }

            yield return null;
        }

        transform.position = targetPos;
    }

    private IEnumerator CollisionStep(path step, int directionBeforeCollision, Tilemap tilemap)
    {
        Vector3 startPos = transform.position;
        Vector3Int currentCell = tilemap.WorldToCell(startPos);
        Vector3 targetPos = tilemap.CellToWorld(currentCell + step.move);

        direction = directionBeforeCollision;

        float duration = 0.3f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, time / duration);
            yield return null;
        }

        transform.position = targetPos;
    }

    private void UpdateStepAnimation(bool isWalking)
    {
        var anims = unitClass.animations;
        if (isWalking)
        {
            switch (direction)
            {
                case 0: spriteRenderer.sprite = anims.walkLeft; break;
                case 1: spriteRenderer.sprite = anims.walkTop; break;
                case 2: spriteRenderer.sprite = anims.walkRight; break;
                case 3: spriteRenderer.sprite = anims.walkBottom; break;
            }
        }
        else
        {
            switch (direction)
            {
                case 0: spriteRenderer.sprite = anims.idleLeft; break;
                case 1: spriteRenderer.sprite = anims.idleTop; break;
                case 2: spriteRenderer.sprite = anims.idleRight; break;
                case 3: spriteRenderer.sprite = anims.idleBottom; break;
            }
        }
    }
    private void SetDirection(Vector3Int move)
    {
        if (move == Vector3Int.zero) return;

        if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
            direction = move.x > 0 ? 2 : 0; 
        else
            direction = move.y > 0 ? 1 : 3; 
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
