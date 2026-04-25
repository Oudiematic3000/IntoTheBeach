using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitAnimator : MonoBehaviour
{
    public Tilemap saloonTiles;
    [SerializeField] private float attackDisplayDuration = 1f;

    private Dictionary<int, CharacterVisual> unitMap;

    private void Awake()
    {
        
    }

    private void OnEnable() => BoardSyncTurnState.OnSyncStart += PlayResults;
    private void OnDisable() => BoardSyncTurnState.OnSyncStart -= PlayResults;

    private void PlayResults(NetUnitResult[] results)
    {
        unitMap = new Dictionary<int, CharacterVisual>();

        foreach (var unit in FindObjectsByType<CharacterVisual>(FindObjectsSortMode.None))
        {
            if (unitMap.ContainsKey(unit.unitID))
            {
                Debug.LogError($"CRITICAL ERROR: Duplicate Unit ID {unit.unitID} found on {unit.gameObject.name}! Overwrite prevented.");
            }
            else
            {
                unitMap[unit.unitID] = unit;
            }
        }
        var movingResults = results.Where(r =>
            unitMap.ContainsKey(r.unitID) && r.paths != null && r.paths.Length > 0).ToList();

        if (movingResults.Count == 0)
        {
            StartCoroutine(ShowAttackIntents(results));
            return;
        }

        int remaining = movingResults.Count;
        foreach (var result in movingResults)
        {
            unitMap.TryGetValue(result.unitID, out var unit);
            List<path> paths = result.paths.Select(p => p.ToPath()).ToList();
            unit.ExecuteMovement(paths, saloonTiles, () =>
            {
                remaining--;
                if (remaining <= 0)
                    StartCoroutine(ShowAttackIntents(results));
            });
        }
    }

    private IEnumerator ShowAttackIntents(NetUnitResult[] results)
    {
        var attackResults = results
            .Where(r => r.hasAttackAction && unitMap.ContainsKey(r.unitID))
            .ToList();

        var allHitTiles = new List<Vector3Int>();

        foreach (var result in attackResults)
        {
            AttackAction attack = result.ToAttackAction();
            Vector3Int attackerPos = result.finalPos.ToVector3Int();
            List<Vector3Int> hitTiles = attack.attackPattern.GetHitTiles(
                GameManager.Instance.GridState, attackerPos, attack.direction);

            foreach (var tile in hitTiles)
            {
                saloonTiles.SetTileFlags(tile, TileFlags.None);
                saloonTiles.SetColor(tile, Color.darkRed);
                allHitTiles.Add(tile);
            }

            yield return new WaitForSeconds(attackDisplayDuration);
        }

        foreach (var tile in allHitTiles)
        {
            saloonTiles.SetTileFlags(tile, TileFlags.None);
            saloonTiles.SetColor(tile, Color.white);
        }
        var reactedTiles = results
       .Where(r => r.reactedTiles != null)
       .SelectMany(r => r.reactedTiles)
       .Select(t => t.ToVector3Int())
       .Distinct();

        foreach (var tile in reactedTiles)
        {
            var envObj = GameManager.Instance.GridState.GetEnvironmentalObject(tile);
            Debug.Log($"Reacted tile {tile} — envObj: {envObj != null}, visual: {envObj?.AttackReactionVisual != null}");
            envObj?.AttackReactionVisual?.PlayReactionVisual();
        }
        foreach (var result in results)
        {
            if (result.damageTaken > 0 && unitMap.TryGetValue(result.unitID , out var unit))
                unit.TakeDamage(result.damageTaken);
        }

        TurnStateMachine.Instance.UpdateState();
    }
}