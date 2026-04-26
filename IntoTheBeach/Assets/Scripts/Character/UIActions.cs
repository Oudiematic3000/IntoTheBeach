using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIActions : MonoBehaviour
{
    public static event Action OnMovement;
    public static event Action OnAttack;
    

    [SerializeField] CharacterVisual selectedCharacter;

    [SerializeField] GameObject classUIHolder, buttonsUIHolder;
    [SerializeField] Button moveButton, attackButton, endTurn;
    [SerializeField] Image attackText, moveText;
    [SerializeField] GameObject endPhaseButton;
    [SerializeField] GameObject[] pips;
    public GameObject selectUnit;
    public GameObject moveUnitText;
    public GameObject attackUnitText;
    public GameObject endTurnUnitText;
    public GameObject tileSelect;
    public GameObject attackTileSelect;
    public GameObject standbyText;
  
    public Image objectIcon;
    private void OnEnable()
    {
       
        CharacterVisual.OnClick += SetSelectedCharacter;
        CharacterVisual.OnClick += updateIcon;
      
        InputManager.OnClickNothing += HideAll;
        GridVisual.OnGridClick += HideAll;
        GridVisual.OnResetPip += ShowAllPips;
        GridVisual.OnUnitMoved += ShowUnitInfo;
        GridVisual.OnUnitMoved += HidePip;
        GridVisual.OnUnitAttacked += HidePip;
        MovePlanTurnState.OnMovePlanStart += ShowEndTurn;
        GridVisual.onMoveText += hideAllText;
        InputManager.OnRemove += HideAll;
        StandbyTurnState.OnStandbyStart += ShowStandbyText;
        BoardSyncTurnState.OnSyncStart += hideStandbyText;
        BoardSyncTurnState.OnSyncStart += hideSelectUnitText;
        MovePlanTurnState.OnMovePlanStart += ShowSelectUnitText;
        StandbyTurnState.OnStandbyStart += HideAllPips;
        MovePlanTurnState.OnMovePlanStart += ShowAllPips;
    }
    private void OnDisable()
    {
        CharacterVisual.OnClick -= SetSelectedCharacter;
       
        CharacterVisual.OnClick -= updateIcon;
        GridVisual.OnGridClick -= HideAll;
        GridVisual.OnResetPip -= ShowAllPips;
        InputManager.OnClickNothing -= HideAll;
        GridVisual.OnUnitMoved -= ShowUnitInfo;
        GridVisual.OnUnitMoved -= HidePip;
        GridVisual.onMoveText -= hideAllText;
        MovePlanTurnState.OnMovePlanStart -= ShowEndTurn;
        GridVisual.OnUnitAttacked -= HidePip;
        InputManager.OnRemove -= HideAll;
        StandbyTurnState.OnStandbyStart -=ShowStandbyText;
        BoardSyncTurnState.OnSyncStart -= hideStandbyText;
        BoardSyncTurnState.OnSyncStart -= hideSelectUnitText;
        MovePlanTurnState.OnMovePlanStart -= ShowSelectUnitText;
        StandbyTurnState.OnStandbyStart -= HideAllPips;
        MovePlanTurnState.OnMovePlanStart -= ShowAllPips;

    }
    public void updateIcon(CharacterVisual character) 
    {
        if(character == null) return;

        objectIcon.sprite = character.unitClass.icon;
        objectIcon.enabled = true;
    }
    public void SetSelectedCharacter(CharacterVisual selectedCharacter)
    {    
        this.selectedCharacter = selectedCharacter;
        ShowUnitInfo();
    }
    public void EndTurn() 
    {
        TurnStateMachine.Instance.UpdateState();
        HideAll();

        GridVisual.resetPip();

        if(TurnStateMachine.Instance.currentState is StandbyTurnState) 
        {
            endPhaseButton.SetActive(false);
        }

    }
    public void ShowEndTurn() 
    {
        endPhaseButton.SetActive(true);
        
    }

    public void MoveButtonPressed() 
    {
        
        moveUnitText.SetActive(false);
        tileSelect.SetActive(true);
        OnMovement?.Invoke();
    }

    public void AttackButtonPressed() 
    {
        //hideAllText();
        attackTileSelect.SetActive(true);
        attackUnitText.SetActive(false);
        selectUnit.SetActive(false);
        OnAttack?.Invoke();
    }

    public void ShowUnitInfo()
    {
        classUIHolder.SetActive(true);

        if(TurnStateMachine.Instance.currentState is MovePlanTurnState)
        {
            buttonsUIHolder.SetActive(true);
            moveButton.gameObject.SetActive(true);
            moveUnitText.gameObject.SetActive(true);
           moveText.gameObject.SetActive(true);
            attackText.gameObject.SetActive(false);
            hasActive();
            print("Movestate");

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
            attackUnitText.gameObject.SetActive(true);
            attackText.gameObject.SetActive(true);
            moveText.gameObject.SetActive(false);
            print("AttackState");
            hasActive();

            if (!TurnStateMachine.Instance.currentTurnInfo.CanAttack() || selectedCharacter.hasAttacked) attackButton.interactable = false;
            else attackButton.interactable = true;

        }
        else if (TurnStateMachine.Instance.currentState is StandbyTurnState)
        {
            HideAll();
            selectUnit.gameObject.SetActive(false);
            endTurnUnitText.gameObject.SetActive(false);
            standbyText.gameObject.SetActive(true);
        }


    }
    public void ShowStandbyText()
    {
        standbyText.gameObject.SetActive(true);

    }
    public void ShowAllPips()
    {
        if(TurnStateMachine.Instance.currentState is AttackPlanTurnState || TurnStateMachine.Instance.currentState is MovePlanTurnState )
        foreach(var pip in pips)pip.SetActive(true);
    }
    void HideAllPips()
    {
        foreach( var pip in pips) pip.SetActive(false);
    }
    public void HidePip()
    {

        if (TurnStateMachine.Instance.currentState is MovePlanTurnState)
        {


            int moveCount = TurnStateMachine.Instance.currentTurnInfo.GetMoveCount();
            int index = pips.Length - moveCount;

            if (index >= 0 && index < pips.Length)
            {
                pips[index].SetActive(false);
            }
            hasActive();


        }
        else if (TurnStateMachine.Instance.currentState is AttackPlanTurnState)
        {
            hideAllText();
            pips[(pips.Length) - TurnStateMachine.Instance.currentTurnInfo.GetAttackCount()].SetActive(false);
            hasActive();

        }



    }

    public bool hasActive()
    {
        foreach (var pip in pips) 
        {
            if (pip.activeSelf) 
            {
                HideEndPhaseText();
                return true;
            }
            
        }
        ShowEndPhaseText();
        return false;
    }

    public void hideAllText() 
    {
        selectUnit.gameObject.SetActive(true);
        attackTileSelect.gameObject.SetActive(false);
        moveUnitText.gameObject.SetActive(false);
        attackUnitText.gameObject.SetActive(false);
        endTurnUnitText.gameObject.SetActive(false);
        tileSelect.SetActive(false);
        if (!hasActive()) 
        {
            endTurnUnitText.gameObject.SetActive(true); 
        }
    }

    public void HideAll()
    {
        hideAllText();
        var currentselection = InputManager.Instance.GetCurrentSelection();
        if(currentselection != null)
        currentselection.RemoveOutline();
        moveButton.gameObject.SetActive(false);
        attackButton.gameObject.SetActive(false);
        classUIHolder.SetActive(false);
        buttonsUIHolder.SetActive(false);
       
    }
    private void showMoveText() 
    {
        selectUnit.SetActive(false);
        moveText.gameObject.SetActive(true);
        attackText.gameObject .SetActive(false);
    }
    private void ShowAttackText() 
    {
        selectUnit.SetActive(false);
        attackText.gameObject.SetActive(true);
        moveText.gameObject.SetActive(false);

    }
    private void hideStandbyText(NetUnitResult[] bruh) {
        standbyText.SetActive(false);
    }
    private void hideSelectUnitText(NetUnitResult[] bruh) 
    {
        selectUnit.SetActive(false);
    }
    private void ShowSelectUnitText()
    {
        selectUnit.SetActive(true);
    }
    private void ShowEndPhaseText()
    {
        endTurnUnitText.SetActive(true);
    }
    private void HideEndPhaseText()
    {
        endTurnUnitText.SetActive(false);

    }
}
