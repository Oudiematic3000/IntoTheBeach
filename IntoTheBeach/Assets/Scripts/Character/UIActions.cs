using System;
using UnityEngine;
using UnityEngine.UI;

public class UIActions : MonoBehaviour
{
    public static event Action OnMovement;
    public static event Action OnAttack;

    [SerializeField] CharacterVisual selectedCharacter;

    [SerializeField] GameObject classUIHolder, buttonsUIHolder;
    [SerializeField] Button moveButton, attackButton, endTurn;
    [SerializeField] Sprite[] classIcons;
    [SerializeField] GameObject[] pips;
    private void OnEnable()
    {
       
        CharacterVisual.OnClick += SetSelectedCharacter;
        InputManager.OnClickNothing += HideAll;
        
        GridVisual.OnUnitMoved += ShowUnitInfo;
        GridVisual.OnUnitMoved += HidePip;
    }
    private void OnDisable()
    {
        CharacterVisual.OnClick -= SetSelectedCharacter;
        InputManager.OnClickNothing -= HideAll;
        GridVisual.OnUnitMoved -= ShowUnitInfo;
        GridVisual.OnUnitMoved -= HidePip;

    }
    public void SetSelectedCharacter(CharacterVisual selectedCharacter)
    {    
        this.selectedCharacter = selectedCharacter;
        ShowUnitInfo();
    }
    public void EndTurn() 
    { 
        HideAll();
        TurnStateMachine.Instance.UpdateState();
       
       
    }

    public void MoveButtonPressed() 
    {
        OnMovement?.Invoke();
    }

    public void AttackButtonPressed() 
    {
        OnAttack?.Invoke();
    }

    public void ShowUnitInfo()
    {
        classUIHolder.SetActive(true);

        if(TurnStateMachine.Instance.currentState is MovePlanTurnState)
        {
            buttonsUIHolder.SetActive(true);
            moveButton.gameObject.SetActive(true);

            if (!TurnStateMachine.Instance.currentTurnInfo.CanMove() || selectedCharacter.hasMoved)
            {
                moveButton.interactable = false;
            }
            else moveButton.interactable = true;
        }

        if(TurnStateMachine.Instance.currentState is AttackPlanTurnState)
        {
            buttonsUIHolder.SetActive(true);
            attackButton.gameObject.SetActive(true);
            if(!TurnStateMachine.Instance.currentTurnInfo.CanAttack() || selectedCharacter.hasAttacked) attackButton.interactable = false;
            else attackButton.interactable = true;

        }

    }
    public void ShowAllPips()
    {
        foreach(var pip in pips)pip.SetActive(true);
    }
    public void HidePip()
    {
        if (TurnStateMachine.Instance.currentState is MovePlanTurnState)
        {
            if (TurnStateMachine.Instance.currentTurnInfo.GetMoveCount() > 2) return;
            pips[(pips.Length-1)-TurnStateMachine.Instance.currentTurnInfo.GetMoveCount()].SetActive(false);
        }else if(TurnStateMachine.Instance.currentState is AttackPlanTurnState)
        {
            pips[(pips.Length - 1) - TurnStateMachine.Instance.currentTurnInfo.GetAttackCount()].SetActive(false);
        }

    }
    public void HideAll()
    {
        var currentselection = InputManager.Instance.GetCurrentSelection();
        currentselection.RemoveOutline();
        moveButton.gameObject.SetActive(false);
        attackButton.gameObject.SetActive(false);
        classUIHolder.SetActive(false);
        buttonsUIHolder.SetActive(false);
    }
}
