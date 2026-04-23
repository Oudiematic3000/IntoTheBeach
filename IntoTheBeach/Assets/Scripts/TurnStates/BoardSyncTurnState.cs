using System;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardSyncTurnState : TurnState
{
    public static event Action<NetUnitResult[]> OnSyncStart;
    public static event Action OnGameStart, OnSyncEnd;
    NetUnitResult[] results;
    bool constructed=false;
    public BoardSyncTurnState(TurnStateMachine turnStateMachine, NetUnitResult[] results) : base(turnStateMachine)
    {
        this.results = results;
        constructed = true;
        StartState();
    }

    public override void StartState()
    {
        if (!constructed) return;
        Debug.Log("Syncing");

        if (results!=null)
        {
            Debug.Log(results +" Results not null");
            OnSyncStart?.Invoke(results);

        }
        else
        {
            Debug.Log("Results are null");
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
