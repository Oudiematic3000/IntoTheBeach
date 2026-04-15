using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TurnSubmitter : MonoBehaviour
{
    public Tilemap saloonTiles;

    public void SubmitCurrentPlan()
    {
        var plans = BuildNetPlans(TurnStateMachine.Instance.currentTurnInfo);
        NetworkTurnManager.Instance.SubmitTurnPlanServerRpc(plans);
    }

    private NetUnitPlan[] BuildNetPlans(TurnInfo turnInfo)
    {
        var plans = new List<NetUnitPlan>();

        foreach (var ghost in turnInfo.ghosts)
        {
            var unit = ghost.owner;
            var action = new MoveAction(
                unit.GetTilePos(saloonTiles),
                ghost.GetTilePos(saloonTiles)
            );
            plans.Add(NetUnitPlan.From(unit.unitID, action));
        }

        return plans.ToArray();
    }
}