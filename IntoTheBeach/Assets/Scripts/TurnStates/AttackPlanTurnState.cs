using System;
using UnityEngine;

public class AttackPlanTurnState : TurnState
{
    public static event Action OnAttackPlanStart, OnAttackPlanEnd;
    
    public AttackPlanTurnState(TurnStateMachine turnStateMachine) : base(turnStateMachine)
    {
    }
    

    public override void StartState()
    {
        OnAttackPlanStart?.Invoke();
        Debug.Log("Start of attack");
    }

    public override void UpdateState()
    {
        OnAttackPlanEnd?.Invoke();
       // turnStateMachine.currentState
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
