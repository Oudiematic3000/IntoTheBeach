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
    private void OnEnable()
    {
        CharacterVisual.OnClick += SetSelectedCharacter;
        InputManager.OnClickNothing += HideAll;
        GridVisual.OnUnitMoved += ShowUnitInfo;
    }
    private void OnDisable()
    {
        CharacterVisual.OnClick -= SetSelectedCharacter;
        InputManager.OnClickNothing -= HideAll;
        GridVisual.OnUnitMoved -= ShowUnitInfo;
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
            if (selectedCharacter.hasMoved) moveButton.interactable = false;
        }

        if(TurnStateMachine.Instance.currentState is AttackPlanTurnState)
        {
            buttonsUIHolder.SetActive(true);
            attackButton.gameObject.SetActive(true);
            if(selectedCharacter.hasAttacked) attackButton.interactable = false;
        }

    }
    public void HideAll()
    {
        moveButton.gameObject.SetActive(false);
        attackButton.gameObject.SetActive(false);
        classUIHolder.SetActive(false);
        buttonsUIHolder.SetActive(false);
    }
}
