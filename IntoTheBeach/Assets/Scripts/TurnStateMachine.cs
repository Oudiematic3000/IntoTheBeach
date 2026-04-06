using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TurnStateMachine : MonoBehaviour
{
    public TurnState currentState;
    public TurnInfo currentTurnInfo;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    public List<GameObject> ghosts = new();

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