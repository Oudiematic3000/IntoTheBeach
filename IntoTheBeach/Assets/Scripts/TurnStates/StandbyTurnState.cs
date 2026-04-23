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
        Debug.Log("Standing By...");
        OnStandbyStart?.Invoke();
    }

    public override void UpdateState()
    {
        OnStandbyEnd?.Invoke();
    }

}
