using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitAnimator : MonoBehaviour
{
    public Tilemap saloonTiles;
    private Dictionary<int, CharacterVisual> unitMap;

    private void Awake()
    {
        unitMap = new Dictionary<int, CharacterVisual>();
        foreach (var unit in FindObjectsByType<CharacterVisual>(FindObjectsSortMode.None))
            unitMap[unit.unitID] = unit;
    }

    private void OnEnable()
    {
        BoardSyncTurnState.OnSyncStart += PlayResults;
    }
    private void OnDisable()
    {
        BoardSyncTurnState.OnSyncStart -= PlayResults;
    }

    private void PlayResults(NetUnitResult[] results)
    {
        var movingResults = results.Where(r =>
    unitMap.ContainsKey(r.unitID) && r.paths != null && r.paths.Length > 0).ToList();

        if (movingResults.Count == 0)
        {
            TurnStateMachine.Instance.UpdateState();
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
                Debug.Log("Animations remaing: " + remaining+" State: "+TurnStateMachine.Instance.currentState);
                
                if (remaining <= 0)
                    TurnStateMachine.Instance.UpdateState();
            });
        }
    }


}
