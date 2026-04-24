using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalObject
{
    public List<Vector3Int> OccupiedTiles { get; set; } = new();
}

public interface IMovementBlocker { }

public interface IAttackBlocker
{
    bool BlocksAttackFromDirection(Vector3Int direction);
}

public interface IAttackReactor
{
    void OnAttacked(GridState gridState, int attackerID);
}


