using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitAnimator : MonoBehaviour
{
    public Tilemap saloonTiles;
    [SerializeField] private float attackDisplayDuration = 4f;

    private Dictionary<int, CharacterVisual> unitMap;
    CameraEdgePanner cameraEdgePanner;
    [SerializeField] AudioClip[] nobodyMove, draw, walk;
    private void Awake()
    {
        cameraEdgePanner = FindAnyObjectByType<CameraEdgePanner>();
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
        if (movingResults.Count > 0) AudioManager.instance.PlaySFX(walk[0]);
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
        cameraEdgePanner.ToggleLockAndCenter();
        var attackResults = results
            .Where(r => r.hasAttackAction && unitMap.ContainsKey(r.unitID))
            .ToList();

        var allHitTiles = new List<Vector3Int>();

        var attackingUnits = attackResults
        .Select(r => unitMap.TryGetValue(r.unitID, out var u) ? u : null)
        .Where(u => u != null)
        .ToList();
        AudioManager.instance.PlaySFX(nobodyMove[Random.Range(0, nobodyMove.Length)]);
        foreach (var result in attackResults)
        {

            AttackAction attack = result.ToAttackAction();
            Vector3Int attackerPos = result.finalPos.ToVector3Int();
            cameraEdgePanner.PanToTile(attackerPos, saloonTiles);
            List<Vector3Int> hitTiles = attack.attackPattern.GetHitTiles(
                GameManager.Instance.GridState, attackerPos, attack.direction);
            var visual = GameManager.Instance.GetVisual(result.unitID);
            if (visual != null) visual.direction = attack.direction;
            foreach (var tile in hitTiles)
            {
                saloonTiles.SetTileFlags(tile, TileFlags.None);
                saloonTiles.SetColor(tile, Color.red);
                GameManager.Instance.GetVisual(result.unitID).ShowAttackOwner();
                LeanTween.delayedCall(0.33f, () => {                    
                    saloonTiles.SetColor(tile, Color.darkRed);
                });
                LeanTween.delayedCall(0.66f, () => {
                    saloonTiles.SetColor(tile, Color.red);
                });
                LeanTween.delayedCall(0.99f, () => {
                    saloonTiles.SetColor(tile, Color.darkRed);
                });
                allHitTiles.Add(tile);
            }

            yield return new WaitForSeconds(attackDisplayDuration);
        }

        cameraEdgePanner.PanToCenter(saloonTiles);
        AudioManager.instance.PlaySFX(draw[Random.Range(0, nobodyMove.Length)]);
        LeanTween.delayedCall(0.5f, () => {
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
                Debug.Log($"Unit {result.unitID} — damageTaken: {result.damageTaken}, isDead: {result.isDead}");
                if (result.damageTaken > 0 && unitMap.TryGetValue(result.unitID, out var unit))
                {
                    unit.TakeDamage(result.damageTaken);
                    AudioManager.instance.PlayHitSound(volume: 0.3f);
                    if (result.isDead)
                    {
                        AudioManager.instance.PlayRandomDeathSound(volume: 0.3f);
                        unitMap.Remove(result.unitID);
                        Destroy(unit.gameObject);
                        continue;
                    }

                    GameManager.Instance.GetVisual(result.unitID).FlashWhite();
                }
                foreach (var attacker in attackingUnits)
                {
                    AudioManager.instance.PlaySFX(attacker.unitClass.attackSound, volume: 0.3f);
                }
            }

        });
        

        TurnStateMachine.Instance.UpdateState();
        cameraEdgePanner.ToggleLockAndCenter();

    }
}