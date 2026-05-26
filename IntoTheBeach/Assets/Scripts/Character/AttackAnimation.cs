using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class AttackAnimation : MonoBehaviour
{
    public AudioClip onStartClip;

    public abstract void Play(Vector3Int attackerPos, List<Vector3Int> hitTiles, 
        Tilemap tilemap, Action onHitImpact, Action onComplete);
}