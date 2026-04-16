using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnResolver
{
    private GridState _gridState;

    public TurnResolver(GridState gridState)
    {
        _gridState = gridState;
    }

    public NetUnitResult[] Resolve(NetUnitPlan[] allPlans)
    {
        var workingPlans = new Dictionary<int, MoveAction>();
        foreach (var plan in allPlans)
            workingPlans[plan.unitID] = plan.ToMoveAction();

        ResolveCollisions(workingPlans);

        return workingPlans.Select(kvp => NetUnitResult.From(kvp.Key, kvp.Value)).ToArray();
    }


    private void ResolveCollisions(Dictionary<int, MoveAction> plans)
    {
        var destinationGroups = new Dictionary<Vector3Int, List<int>>();
        foreach (var kvp in plans)
        {
            if (!destinationGroups.ContainsKey(kvp.Value.resultant))
                destinationGroups[kvp.Value.resultant] = new List<int>();
            destinationGroups[kvp.Value.resultant].Add(kvp.Key);
        }

        foreach (var kvp in destinationGroups)
        {
            if (kvp.Value.Count <= 1) continue;
            ResolveContestedTile(kvp.Key, kvp.Value, plans);
        }
    }

    private void ResolveContestedTile(Vector3Int contestedTile, List<int> unitIDs, Dictionary<int, MoveAction> plans)
    {
        int winnerID = -1;
        int shortestDistance = int.MaxValue;
        bool isTied = false;

        foreach (int id in unitIDs)
        {
            int dist = ManhattanDistance(plans[id].startPos, contestedTile);
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                winnerID = id;
                isTied = false;
            }
            else if (dist == shortestDistance)
            {
                isTied = true;
            }
        }

        foreach (int id in unitIDs)
        {
            if (!isTied && id == winnerID) continue;
            plans[id].StopOneTileShort();
        }
    }

    private int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}