using System;
using UnityEngine;

public class MovePlanTurnState : TurnState
{

    public static event Action OnMovePlanStart, OnMovePlanEnd;
    public MovePlanTurnState(TurnStateMachine turnStateMachine) : base(turnStateMachine)
    {
    }

    public override void StartState()
    {
        OnMovePlanStart?.Invoke();
        Debug.Log("MovePlan");

    }

    public override void UpdateState()
    {
        OnMovePlanEnd?.Invoke();
        turnStateMachine.currentState=new AttackPlanTurnState(turnStateMachine);
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
