using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnResolver
{
    private GridState gridState;
    Dictionary<int,int> pendingDamage = new Dictionary<int, int>();

    public TurnResolver(GridState gridState)
    {
        this.gridState = gridState;
    }

    public NetUnitResult[] Resolve(NetUnitPlan[] allPlans)
    {
        var workingMoves = new Dictionary<int, MoveAction>();
        var workingAttacks = new Dictionary<int, AttackAction>();

        foreach (var plan in allPlans)
        {
            if (plan.hasMoveAction)
                workingMoves[plan.unitID] = plan.ToMoveAction();
            else
            {
                Vector3Int currentPos = gridState.GetUnitPosition(plan.unitID) ?? Vector3Int.zero;
                workingMoves[plan.unitID] = new MoveAction(currentPos, currentPos);
            }

            if (plan.hasAttackAction)
                workingAttacks[plan.unitID] = plan.ToAttackAction();
        }

        foreach (int unitID in gridState.GetAllUnitIDs())
        {
            if (!workingMoves.ContainsKey(unitID))
            {
                Vector3Int currentPos = gridState.GetUnitPosition(unitID) ?? Vector3Int.zero;
                workingMoves[unitID] = new MoveAction(currentPos, currentPos);
            }
        }

        ResolveCollisions(workingMoves);
        ApplyMovesToGridState(workingMoves);
        ResolveAttacks(workingMoves, workingAttacks);

        return workingMoves.Select(kvp => NetUnitResult.From(
            kvp.Key,
            kvp.Value,
            workingAttacks.TryGetValue(kvp.Key, out var attack) ? attack : null,
            pendingDamage.TryGetValue(kvp.Key, out int dmg) ? dmg : 0
        )).ToArray();
    }

    private void ApplyMovesToGridState(Dictionary<int, MoveAction> moves)
    {
        foreach (var kvp in moves)
            gridState.UpdateUnitPosition(kvp.Key, kvp.Value.startPos, kvp.Value.resultant);
    }

    private void ResolveAttacks(Dictionary<int, MoveAction> moves, Dictionary<int, AttackAction> attacks)
    {

        foreach (var kvp in attacks)
        {
            int attackerID = kvp.Key;
            AttackAction attack = kvp.Value;

            Vector3Int attackerPos = moves[attackerID].resultant;

            List<Vector3Int> hitTiles = attack.attackPattern.GetHitTiles(gridState, attackerPos, attack.direction);

            foreach (var tile in hitTiles)
            {
                int? hitUnitID = gridState.GetUnitAtPosition(tile);
                if (hitUnitID.HasValue)
                {
                    Debug.Log("HITHITHIT");
                    if (!pendingDamage.ContainsKey(hitUnitID.Value))
                        pendingDamage[hitUnitID.Value] = 0;
                    pendingDamage[hitUnitID.Value]++;
                }

                gridState.TriggerAttackReaction(tile, attackerID);
            }
        }

        foreach (var kvp in pendingDamage)
            gridState.ApplyDamage(kvp.Key, kvp.Value);
    }

    private void ResolveCollisions(Dictionary<int, MoveAction> moves)
    {
        var destinationGroups = new Dictionary<Vector3Int, List<int>>();
        foreach (var kvp in moves)
        {
            if (!destinationGroups.ContainsKey(kvp.Value.resultant))
                destinationGroups[kvp.Value.resultant] = new List<int>();
            destinationGroups[kvp.Value.resultant].Add(kvp.Key);
        }
        foreach (var kvp in destinationGroups)
        {
            if (kvp.Value.Count <= 1) continue;
            ResolveContestedTile(kvp.Key, kvp.Value, moves);
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