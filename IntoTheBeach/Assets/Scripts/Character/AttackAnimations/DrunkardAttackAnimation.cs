using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrunkardAttackAnimation : AttackAnimation
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] SpriteRenderer bottleRenderer;
    public Sprite[] animationFrames;
    public Sprite bottle;
    public float speed = 0.1f;
    public float throwDuration = 1.2f;
    public float arcHeight = 1.5f;
    private int currentFrame = 0;
    public AudioClip shatterSound;

    public override void Play(Vector3Int attackerPos, int direction, List<Vector3Int> hitTiles,
        Tilemap tilemap, Action onHitImpact, Action onComplete)
    {
        spriteRenderer.enabled = false;

        Vector3Int[] directionVectors = {
        Vector3Int.left, Vector3Int.up, Vector3Int.right, Vector3Int.down
    };
        Vector3Int centerTile = attackerPos + directionVectors[direction] * 4; 
        Vector3 cellCenter = tilemap.GetCellCenterWorld(centerTile);
        Vector3 targetWorld = cellCenter;
        Vector3 startWorld = tilemap.GetCellCenterWorld(attackerPos);


        bottleRenderer.transform.position = startWorld;
        bottleRenderer.sprite = bottle;
        bottleRenderer.enabled = true;

        float frameInterval = 1f / 12f;
        float accumulated = 0f;
        Vector3 displayPos = startWorld;

        LeanTween.value(gameObject, 0f, 1f, throwDuration)
            .setEase(LeanTweenType.linear)
            .setOnUpdate((float t) =>
            {
                accumulated += Time.deltaTime;

                if (accumulated >= frameInterval)
                {
                    accumulated = 0f;

                    Vector3 flatPos = Vector3.Lerp(startWorld, targetWorld, t);
                    float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;
                    displayPos = new Vector3(flatPos.x, flatPos.y + arc, flatPos.z);
                    bottleRenderer.transform.rotation = Quaternion.Euler(0f, 0f, -360f * t * 2f);
                }

                bottleRenderer.transform.position = displayPos;
            })
            .setOnComplete(() =>
            {
                bottleRenderer.enabled = false;
                AudioManager.instance.PlaySFX(shatterSound);
                transform.position = targetWorld;
                spriteRenderer.enabled = true;
                currentFrame = 0;
                AdvanceFrame();

                LeanTween.delayedCall(speed * 2f, () => onHitImpact?.Invoke());
                LeanTween.delayedCall(speed * animationFrames.Length, () =>
                {
                    onComplete?.Invoke();
                    transform.localScale=Vector3.zero;
                });
                LeanTween.delayedCall(10f, () => { Destroy(gameObject); });

            });
    }

    private void AdvanceFrame()
    {
        if (animationFrames == null || animationFrames.Length == 0) return;
        spriteRenderer.sprite = animationFrames[currentFrame];
        currentFrame++;
        if (currentFrame == 2)
            AudioManager.instance.PlaySFX(onStartClip);
        if (currentFrame < animationFrames.Length)
            LeanTween.delayedCall(speed, AdvanceFrame);
    }
}