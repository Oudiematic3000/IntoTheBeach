using System;
using UnityEngine;

public class BoardSyncTurnState : TurnState
{
    public static event Action<NetUnitResult[]> OnSyncStart;
    public static event Action OnGameStart, OnSyncEnd;
    NetUnitResult[] results;
    public BoardSyncTurnState(TurnStateMachine turnStateMachine, NetUnitResult[] results) : base(turnStateMachine)
    {
        this.results = results;
    }

    public override void StartState()
    {
        Debug.Log("Syncing");

        if (!results.Equals(null))
            OnSyncStart?.Invoke(results);
        else
        {
            OnGameStart?.Invoke();
            LeanTween.delayedCall(0f, () =>
            {
                UpdateState();

            });
        }
        
    }

    public override void UpdateState()
    {
        OnSyncEnd?.Invoke();
        turnStateMachine.currentState = new MovePlanTurnState(turnStateMachine);
    }

}
