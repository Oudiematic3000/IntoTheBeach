using System;
using UnityEngine;

public class StandbyTurnState : TurnState
{
    public static event Action OnStandbyStart, OnStandbyEnd;

    public StandbyTurnState(TurnStateMachine turnStateMachine) : base(turnStateMachine)
    {
    }

    public override void StartState()
    {
        OnStandbyStart?.Invoke();
        Debug.Log("Standing By...");
    }

    public override void UpdateState()
    {
        OnStandbyEnd?.Invoke();
    }

}
