using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnitAnimations;

public class TurnStateMachine : MonoBehaviour
{
    public static TurnStateMachine Instance;
    public TurnState currentState;
    public TurnInfo currentTurnInfo;
    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        BoardSyncTurnState.OnSyncStart += CreateTurnInfo;
        BoardSyncTurnState.OnGameStart += CreateTurnInfo;
    }
    private void OnDisable()
    {
        BoardSyncTurnState.OnSyncStart -= CreateTurnInfo;
        BoardSyncTurnState.OnGameStart -= CreateTurnInfo;
    }
    void Start()
    {
        currentState = new BoardSyncTurnState(this, null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public AnimState GetAnimState()
    {
        if (currentState is MovePlanTurnState)
            return AnimState.Move;

        if (currentState is AttackPlanTurnState)
            return AnimState.Attack;

        return AnimState.None;
    }
    public void CreateTurnInfo(NetUnitResult[] results)
    {
        currentTurnInfo = new TurnInfo();
    }
    public void CreateTurnInfo()
    {
        currentTurnInfo = new TurnInfo();
    }

    public void UpdateState() 
    {
        currentState.UpdateState();
    }
}

public abstract class TurnState
{
    protected TurnStateMachine turnStateMachine;
    public TurnState(TurnStateMachine turnStateMachine)
    {
        this.turnStateMachine = turnStateMachine;
        StartState();
    }
    public abstract void UpdateState();

    public abstract void StartState();

}

public class TurnInfo
{
    int moveActionCount = 0;
    int attackActionCount = 0;
    public List<UnitGhost> ghosts = new();
    
    public int GetMoveCount()
    {
        return moveActionCount;
    }
    public int GetAttackCount()
    {
        return attackActionCount;
    }
    public bool CanMove()
    {
        return moveActionCount < 2;
    }
    public bool CanAttack()
    {
        return attackActionCount < 2;
    }
    public void IncrementMoveCount(int count)
    {
        if (moveActionCount < 2 && count > 0) moveActionCount += count;
        else if (moveActionCount > 0 && count < 0) moveActionCount += count;
    }
    public void IncrementAttackCount(int count)
    {
        if (attackActionCount < 2 && count > 0) attackActionCount += count;
        else if (attackActionCount > 0 && count < 0) attackActionCount += count;
    }
}