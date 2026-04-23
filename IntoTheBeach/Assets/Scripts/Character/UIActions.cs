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
    public GameObject selectUnit;
    public GameObject moveUnitText;
    public GameObject attackUnitText;
    public GameObject endTurnUnitText;
    public GameObject tileSelect;
    public GameObject attackTileSelect;
    private void OnEnable()
    {
       
        CharacterVisual.OnClick += SetSelectedCharacter;
        CharacterVisual.OnMoveSelected += showMoveText;
        CharacterVisual.OnAttackSelected += ShowAttackText;
        InputManager.OnClickNothing += HideAll;
        GridVisual.OnGridClick += HideAll;
        GridVisual.OnResetPip += ShowAllPips;
        GridVisual.OnUnitMoved += ShowUnitInfo;
        GridVisual.OnUnitMoved += HidePip;
        GridVisual.OnUnitAttacked += HidePip;
        GridVisual.onMoveText += hideAllText;
    }
    private void OnDisable()
    {
        CharacterVisual.OnClick -= SetSelectedCharacter;
        CharacterVisual.OnMoveSelected -= showMoveText;
        CharacterVisual.OnAttackSelected -= ShowAttackText;
        GridVisual.OnGridClick -= HideAll;
        GridVisual.OnResetPip -= ShowAllPips;
        InputManager.OnClickNothing -= HideAll;
        GridVisual.OnUnitMoved -= ShowUnitInfo;
        GridVisual.OnUnitMoved -= HidePip;
        GridVisual.onMoveText -= hideAllText;
        GridVisual.OnUnitAttacked -= HidePip;


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

    }

    public void MoveButtonPressed() 
    {
        hideAllText();
        moveUnitText.SetActive(false);
        tileSelect.SetActive(true);
        OnMovement?.Invoke();
    }

    public void AttackButtonPressed() 
    {
        hideAllText();
        attackTileSelect.SetActive(true);
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
            pips[(pips.Length)-TurnStateMachine.Instance.currentTurnInfo.GetMoveCount()].SetActive(false);
        }else if(TurnStateMachine.Instance.currentState is AttackPlanTurnState)
        {
            pips[(pips.Length - 1) - TurnStateMachine.Instance.currentTurnInfo.GetAttackCount()].SetActive(false);
        }

    }
    public void hideAllText() 
    {
        selectUnit.gameObject.SetActive(true);
        moveUnitText.gameObject.SetActive(false);   
        attackTileSelect.gameObject.SetActive(false);
        attackUnitText.gameObject.SetActive(false);
        endTurnUnitText.gameObject.SetActive(false);
        tileSelect.SetActive(false);
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
        moveUnitText.SetActive(true);
    }
    private void ShowAttackText() 
    {
        selectUnit.SetActive(false);
        attackUnitText.SetActive(true);
    }
}
