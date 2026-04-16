using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnPlan
{
    List<UnitPlan> unitPlans = new List<UnitPlan>();
}

public class UnitPlan
{
    public int unitID;
    public MoveAction moveAction;
}

public class MoveAction
{
    public Vector3Int startPos, resultant;
    public List<path> paths;

    public MoveAction(Vector3Int startPos, Vector3Int resultant)
    {
        this.startPos = startPos;
        this.resultant = resultant;
        this.paths = BreakdownMove(startPos, resultant);
    }

    List<path> BreakdownMove(Vector3Int to, Vector3Int from)
    {
        int xDistance = to.x - from.x;
        int yDistance = to.y - from.y;

        int move1Distance, move2Distance;
        move1Distance = Math.Min(xDistance, yDistance);
        move2Distance = Math.Max(xDistance, yDistance);

        Vector3Int move1= Vector3Int.zero, move2=Vector3Int.zero;
        Vector2 move2Normalised = Vector2.zero;
        List<path> paths = new List<path>();

        if (xDistance == move1Distance)
        {
            move1 = new Vector3Int(move1Distance, 0, 0);
            move2 = new Vector3Int(0, move2Distance, 0);
            move2Normalised = new Vector2(move2.x, move2.y).normalized;

        }
        else if (yDistance == move1Distance)
        {
            move1 = new Vector3Int(0, move1Distance, 0);
            move2 = new Vector3Int(move2Distance, 0, 0);
            move2Normalised = new Vector2(move2.x, move2.y).normalized;

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

    public void KnockBack(Vector3Int direction)
    {
        paths.Add(new path(direction, path.MoveType.collision)); 
        CalculateResultant();
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