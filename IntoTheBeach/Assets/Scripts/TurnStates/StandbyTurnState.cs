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
    }

    public override void UpdateState()
    {
        OnStandbyEnd?.Invoke();
       // turnStateMachine.currentState=new 
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
