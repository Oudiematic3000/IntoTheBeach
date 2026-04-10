using System;
using UnityEngine;

public class BoardSyncTurnState : TurnState
{
    public static event Action OnSyncStart, OnSyncEnd;

    public BoardSyncTurnState(TurnStateMachine turnStateMachine) : base(turnStateMachine)
    {

    }

    public override void StartState()
    {
        Debug.Log("Syncing");

        OnSyncStart?.Invoke();
        //TEMP
        LeanTween.delayedCall(0f, () =>
        {
            UpdateState();

        });
        //TEMP
        
    }

    public override void UpdateState()
    {
        OnSyncEnd?.Invoke();
        turnStateMachine.currentState = new MovePlanTurnState(turnStateMachine);
    }

}
