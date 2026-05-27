using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GunslingerAttackAnimation : AttackAnimation
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float duration = 0.01f;

    public override void Play(Vector3Int attackerPos, int direction, List<Vector3Int> hitTiles, 
        Tilemap tilemap, Action onHitImpact, Action onComplete)
    {
        if (hitTiles.Count == 0) { onComplete?.Invoke(); Destroy(gameObject); return; }
        print("Start tile: " + tilemap.CellToWorld(attackerPos) + Vector3.one * 0.5f + " End tile: " + tilemap.CellToWorld(hitTiles.Last()) + Vector3.one * 0.5f);
        Vector3 start = tilemap.CellToWorld(attackerPos) + Vector3.up * 0.7f;
        Vector3 end = tilemap.CellToWorld(hitTiles.Last()) + Vector3.up * 0.7f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        AudioManager.instance.PlaySFX(onStartClip);
        onHitImpact?.Invoke();
        LeanTween.delayedCall(duration, () =>
        {
            
            onComplete?.Invoke();
            Destroy(gameObject);
        });
    }
}

