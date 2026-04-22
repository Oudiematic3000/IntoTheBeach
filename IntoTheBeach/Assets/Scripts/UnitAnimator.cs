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

    private void OnEnable() => NetworkTurnManager.OnTurnResolved += PlayResults;
    private void OnDisable() => NetworkTurnManager.OnTurnResolved -= PlayResults;

    private void PlayResults(NetUnitResult[] results)
    {
        foreach (var result in results)
        {
            if (!unitMap.TryGetValue(result.unitID, out var unit)) continue;

            List<path> paths = result.paths.Select(p => p.ToPath()).ToList();
            unit.ExecuteMovement(paths, saloonTiles);
        }
    }

   
}
