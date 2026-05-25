using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TurnPlan
{
    List<UnitPlan> unitPlans = new List<UnitPlan>();

    public List<UnitPlan> GetUnitPlans() {  return unitPlans; }
    public void ModifyUnitPlanMoveAction(int ID, MoveAction moveAction)
    {
        UnitPlan plan = unitPlans.FirstOrDefault(plan => plan.unitID == ID);
        if (plan != null)
            plan.moveAction = moveAction;
        else
            unitPlans.Add(new UnitPlan(ID, moveAction));
    }

    public void ModifyUnitPlanAttackAction(int ID, AttackAction attackAction)
    {
        UnitPlan plan = unitPlans.FirstOrDefault(plan => plan.unitID == ID);
        if (plan != null)
            plan.attackAction = attackAction;
        else
            unitPlans.Add(new UnitPlan(ID, null, attackAction));
    }

}

public class UnitPlan
{
    public int unitID;
    public MoveAction moveAction;
    public AttackAction attackAction;

    public UnitPlan(int unitID, MoveAction moveAction = null, AttackAction attackAction=null)
    {
        this.unitID = unitID;
        this.moveAction = moveAction;
        this.attackAction = attackAction;
    }
}

public class MoveAction
{
    public Vector3Int startPos, resultant;
    public List<path> paths;
    public CharacterVisual characterVisual;

    public MoveAction(Vector3Int startPos, Vector3Int resultant, GridState gridState = null, Tilemap floorTilemap = null)
    {
        this.startPos = startPos;
        this.resultant = resultant;
        this.paths = gridState != null
            ? AStarBreakdown(startPos, resultant, gridState, floorTilemap)
            : BreakdownMove(startPos, resultant);
    }

    private List<path> AStarBreakdown(Vector3Int from, Vector3Int to, GridState gridState, Tilemap floorTilemap)
    {
        var openSet = new List<Vector3Int> { from };
        var closedSet = new HashSet<Vector3Int>();
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        var gScore = new Dictionary<Vector3Int, int> { [from] = 0 };
        var fScore = new Dictionary<Vector3Int, int> { [from] = Heuristic(from, to) };

        Vector3Int[] neighbours = {
            Vector3Int.right, Vector3Int.left,
            Vector3Int.up,    Vector3Int.down
        };

        while (openSet.Count > 0)
        {
            Vector3Int current = openSet.OrderBy(n => fScore.TryGetValue(n, out int f) ? f : int.MaxValue).First();

            if (current == to)
                return ReconstructPaths(cameFrom, current);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (var dir in neighbours)
            {
                Vector3Int neighbour = current + dir;

                if (closedSet.Contains(neighbour)) continue;

                if (floorTilemap != null && !floorTilemap.HasTile(neighbour)) continue;

                if (neighbour != to && gridState.IsMovementBlocked(neighbour)) continue;

                int tentativeG = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbour) || tentativeG < gScore[neighbour])
                {
                    cameFrom[neighbour] = current;
                    gScore[neighbour] = tentativeG;
                    fScore[neighbour] = tentativeG + Heuristic(neighbour, to);

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return BreakdownMove(from, to);
    }

    private List<path> ReconstructPaths(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        var waypoints = new List<Vector3Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            waypoints.Insert(0, current);
        }

        var paths = new List<path>();
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Vector3Int move = waypoints[i + 1] - waypoints[i];
            paths.Add(new path(move, path.MoveType.walk));
        }
        return paths;
    }

    private int Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private List<path> BreakdownMove(Vector3Int from, Vector3Int to)
    {
        int xDistance = to.x - from.x;
        int yDistance = to.y - from.y;

        int move1Distance = Math.Min(xDistance, yDistance);
        int move2Distance = Math.Max(xDistance, yDistance);

        Vector3Int move1 = Vector3Int.zero, move2 = Vector3Int.zero;
        List<path> paths = new List<path>();

        if (xDistance == move1Distance)
        {
            move1 = new Vector3Int(move1Distance, 0, 0);
            move2 = new Vector3Int(0, move2Distance, 0);
        }
        else if (yDistance == move1Distance)
        {
            move1 = new Vector3Int(0, move1Distance, 0);
            move2 = new Vector3Int(move2Distance, 0, 0);
        }

        paths.Add(new path(move1, path.MoveType.walk));
        paths.Add(new path(move2, path.MoveType.walk));
        return paths;
    }

    public Vector3Int GetDirection()
    {

        Vector3Int dir = resultant - startPos;


        int x = Math.Sign(dir.x);
        int y = Math.Sign(dir.y);


        if (Math.Abs(dir.x) > Math.Abs(dir.y))
        {
            return x > 0 ? Vector3Int.right : Vector3Int.left;
        }
        else
        {
            return y > 0 ? Vector3Int.up : Vector3Int.down;
        }
    }

    public void KnockBack(Vector3Int direction, GridState gridState = null)
    {
        Vector3Int knockbackTarget = resultant + direction;

        if (gridState != null && gridState.IsMovementBlocked(knockbackTarget))
        {
            paths.Add(new path(Vector3Int.zero, path.MoveType.collision));
        }
        else
        {
            paths.Add(new path(direction, path.MoveType.collision));
            CalculateResultant();
        }
    }
    public void StopOneTileShort()
    {
        resultant = resultant - GetDirection();
        paths = BreakdownMove(startPos, resultant);
        paths.Add(new path(Vector3Int.zero, path.MoveType.collision));
    }

    public void CalculateResultant()
    {
        Vector3Int pathsResult =Vector3Int.zero;
        foreach (path path in paths) {
            pathsResult += path.move;
                }
        resultant = startPos + pathsResult;
    }
}

public class AttackAction
{
    public Vector3Int unitPos;
    public AttackPattern attackPattern;
    public int direction;
    public int unitID;

    public AttackAction(Vector3Int unitPos, AttackPattern attackPattern, int direction, int unitID)
    {
        this.unitPos = unitPos;
        this.attackPattern = attackPattern;
        this.direction = direction;
        this.unitID = unitID;
    }
}

public struct path
{
    public Vector3Int move;
    public MoveType moveType;
    public enum MoveType
    {
        walk,
        collision
    }
    public path(Vector3Int m, MoveType type)
    {
        move = m;
        moveType=type;
    }
}