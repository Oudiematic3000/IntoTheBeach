using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour, IMovementBlocker, IAttackBlocker, IAttackReactor, IAttackReactionVisual
{
    public EnvironmentalObject envObj;
    private GridState gridState;
    [SerializeField] List<Vector3Int> blockDirections;
    [SerializeField] List<GameObject> extraPositions = new List<GameObject>();

    public Sprite[] animationFrames;
    public float speed = 0.1f;
    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;

    void Start()
    {
        gridState = GameManager.Instance.GridState;

        envObj = new EnvironmentalObject();
        envObj.OccupiedTiles.Add(gridState.WorldToCell(transform.position));

        if (extraPositions.Count > 0)
        {
            foreach (GameObject go in extraPositions) envObj.OccupiedTiles.Add(gridState.WorldToCell(go.transform.position));

        }
        envObj.MovementBlocker = this;
        envObj.AttackBlocker = this;
        gridState.RegisterEnvironmentalObject(envObj);
    }

    void OnDestroy()
    {
        gridState.UnregisterEnvironmentalObject(envObj);
    }
    public bool BlocksAttackFromDirection(Vector3Int direction)
    {
        if (blockDirections.Contains(direction))
        {
            return true;
        }
        return false;
    }
    public void PlayAnimation()
    {
        currentFrame = 0;
        AdvanceFrame();
    }
    private void AdvanceFrame()
    {
        if (animationFrames == null || animationFrames.Length == 0) return;

        spriteRenderer.sprite = animationFrames[currentFrame];
        currentFrame++;

        if (currentFrame < animationFrames.Length)
        {
            LeanTween.delayedCall(speed, AdvanceFrame);
        }
        Destroy(gameObject);
    }
    public void OnAttacked(GridState gridState, int attackerID)
    {
        
    }

    public void PlayReactionVisual()
    {
        PlayAnimation();
    }
}
