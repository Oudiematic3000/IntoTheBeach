using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BouncerAttackAnimation : AttackAnimation
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public Sprite[] animationFrames;
    public float speed = 0.1f;
    private int currentFrame = 0;
    public override void Play(Vector3Int attackerPos, int direction, List<Vector3Int> hitTiles, Tilemap tilemap, Action onHitImpact, Action onComplete)
    {
        switch (direction)
        {
            case 0:
                transform.localPosition = new Vector2(-0.04f, 0.18f);
                transform.rotation = Quaternion.Euler(0f, 0f, 188.18f);
                break;
            case 1:
                transform.localPosition = new Vector2(-0.32f, 0.83f);
                transform.rotation = Quaternion.Euler(0f, 0f, 77.4f);
                break;
            case 2:
                transform.localPosition = new Vector2(0.5f, 0.66f);
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case 3:
                transform.localPosition = new Vector2(0.54f, -0.05f);
                transform.rotation = Quaternion.Euler(0f, 0f, 245.75f);
                break;
        }
        currentFrame = 0;
        AdvanceFrame();
        LeanTween.delayedCall(speed * 2f, () =>
        {
            onHitImpact?.Invoke();
        });
        LeanTween.delayedCall(speed * 5, () =>
        {
            onComplete?.Invoke();
            Destroy(gameObject);
        });
    }

    private void AdvanceFrame()
    {
        if (animationFrames == null || animationFrames.Length == 0) return;

        spriteRenderer.sprite = animationFrames[currentFrame];
        currentFrame++;
        if (currentFrame == 2) 
        {
            AudioManager.instance.PlaySFX(onStartClip);
        }
        if (currentFrame < animationFrames.Length)
            LeanTween.delayedCall(speed, AdvanceFrame);

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
