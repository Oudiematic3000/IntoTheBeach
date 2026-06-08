using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering; 
using UnityEngine.UI;   

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject SelectUnitText, moveRangeText, selectAtileToMoveToText, EndPhaseText, attackRangeText;
    [SerializeField] GameObject selectATileToAttackText, moveAttackUI, HowToCancelText;
    [SerializeField] GameObject moveButton, endPhaseButton, attackButton;
    [SerializeField] GameObject board;
    [SerializeField] GameObject textUnitHover, textPause, textPlan, textPlayerTurn, pip1, pip2;
    private void OnEnable()
    {
        tempSortingGroups.Clear();
        tempCanvases.Clear();
        LobbyMenu.OnClientStart += RunSelectUnit;
        CharacterVisual.OnClick += RunPressMoveRange;
        UIActions.OnMovement += RunSelectTileToMoveTo;
        GridVisual.OnUnitMoved += HideEverythingOnGhostPlacement;
        UIActions.OnNoPipsLeft += EndPhaseHighlight;
        AttackPlanTurnState.OnAttackPlanStart += RunSelectUnitTwo;
        CharacterVisual.OnClick += RunViewAttackRange;
        UIActions.OnAttack += RunLockInAttack;
        GridVisual.OnUnitAttacked += FinalNotes;
        CharacterVisual.OnClick += HideOnUnitHover;
        StandbyTurnState.OnStandbyStart += EndTutorial;
        InputManager.Pause += HideOnPause;

    }
    private void OnDisable()
    {
        LobbyMenu.OnClientStart -= RunSelectUnit;
        CharacterVisual.OnClick -= RunPressMoveRange;
        UIActions.OnMovement -= RunSelectTileToMoveTo;
        GridVisual.OnUnitMoved -= HideEverythingOnGhostPlacement;
        UIActions.OnNoPipsLeft -= EndPhaseHighlight;
        AttackPlanTurnState.OnAttackPlanStart -= RunSelectUnitTwo;
        CharacterVisual.OnClick -= RunViewAttackRange;
        UIActions.OnAttack -= RunLockInAttack;
        GridVisual.OnUnitAttacked -= FinalNotes;
        CharacterVisual.OnClick -= HideOnUnitHover;
        StandbyTurnState.OnStandbyStart -= EndTutorial;
        InputManager.Pause -= HideOnPause;


    }
    private void Start()
    {
        if (PlayerData.Local != null) RunSelectUnit();
    }
    public enum TutorialPhases
    {
        None,
        SelectAUnit,
        ViewMoveRange,
        SelectTileToMoveTo,
        EndPhase,
        SelectAUnit2,
        ViewAttackRange,
        LockInAttack,
        FinalNotes,
    }

    public TutorialPhases currentPhase = TutorialPhases.None;
    [SerializeField] private GameObject blackScreen;

    private List<SortingGroup> tempSortingGroups = new();
    private Dictionary<SortingGroup, (string layer, int order)> originalSortingStates = new();

    private List<Canvas> tempCanvases = new();
    private Dictionary<Canvas, (string layer, int order, bool overrideSort)> originalCanvasStates = new();

     void RunSelectUnit()
    {
        currentPhase = TutorialPhases.SelectAUnit;
        ResetAllHighlights();
        blackScreen.SetActive(true);
        TryRaiseUnits();
        HighlightUI(SelectUnitText);
        endPhaseButton.SetActive(false);
    }

    void RunPressMoveRange(CharacterVisual characterVisual)
    {
        if (currentPhase != TutorialPhases.SelectAUnit) return;
        ResetAllHighlights();
        moveAttackUI.SetActive(true);
        HighlightUI(moveButton);
        HighlightUI(moveRangeText);
        endPhaseButton?.SetActive(false);
        currentPhase= TutorialPhases.ViewMoveRange;
    }
    void RunSelectTileToMoveTo()
    {
        if (currentPhase != TutorialPhases.ViewMoveRange) return;
        ResetAllHighlights();
        HighlightGameObject(InputManager.Instance.GetCurrentSelection().gameObject);
        HighlightGameObject(board);
        HighlightUI(selectAtileToMoveToText);
        HighlightUI(textPlan);
        HighlightUI(pip1);
        HighlightUI(pip2);
        LeanTween.delayedCall(0f, () =>
        {
            HighlightGameObject(InputManager.Instance.GetCurrentSelection().ghost.gameObject);
        });
        currentPhase = TutorialPhases.SelectTileToMoveTo;

    }
    void HideEverythingOnGhostPlacement()
    {
        if (currentPhase != TutorialPhases.SelectTileToMoveTo) return;
        ResetAllHighlights();
        textPlan.SetActive(false);
        HighlightUI(textUnitHover);
        blackScreen?.SetActive(false);
    }
    void EndPhaseHighlight()
    {
        if (currentPhase != TutorialPhases.SelectTileToMoveTo) return;
        ResetAllHighlights();
        textUnitHover.SetActive(false);
        blackScreen?.SetActive(true);
        endPhaseButton.SetActive(true);
        HighlightUI(endPhaseButton);
        HighlightUI(EndPhaseText);
        currentPhase = TutorialPhases.EndPhase;
    }
    void HideOnUnitHover(CharacterVisual character)
    {
        if (currentPhase != TutorialPhases.SelectTileToMoveTo) return;
        textUnitHover.SetActive(false);
    }
    void RunSelectUnitTwo()
    {
        currentPhase = TutorialPhases.EndPhase;
        ResetAllHighlights();
        blackScreen.SetActive(true);
        TryRaiseUnits();
        HighlightUI(SelectUnitText);
        endPhaseButton.SetActive(true);
        currentPhase = TutorialPhases.SelectAUnit2;
    }
    void RunViewAttackRange(CharacterVisual visual)
    {
        if (currentPhase != TutorialPhases.SelectAUnit2) return;
        ResetAllHighlights();
        LeanTween.delayedCall(0f, () =>
        {
            HighlightGameObject(InputManager.Instance.GetCurrentSelection().gameObject);
            HowToCancelText.SetActive(true);
            HighlightUI(HowToCancelText);
            HighlightUI(attackButton);
            HighlightUI(attackRangeText);
        });
    
        currentPhase = TutorialPhases.ViewAttackRange;
    }
    void RunLockInAttack()
    {
        if (currentPhase != TutorialPhases.ViewAttackRange) return;
        ResetAllHighlights();
        HowToCancelText.SetActive(false);
        HighlightGameObject(InputManager.Instance.GetCurrentSelection().gameObject);
        LeanTween.delayedCall(0f, () =>
        {
            if(InputManager.Instance.GetCurrentSelection().ghost.gameObject!=null)
            HighlightGameObject(InputManager.Instance.GetCurrentSelection().ghost.gameObject);
        });
        HighlightGameObject(board);
        HighlightUI(selectATileToAttackText);
        currentPhase = TutorialPhases.LockInAttack;

    }
    void FinalNotes()
    {
        if (currentPhase != TutorialPhases.LockInAttack) return;
        ResetAllHighlights();
        blackScreen.SetActive(false);
        HighlightUI(textPause);
        HighlightUI(textPlayerTurn);
        currentPhase = TutorialPhases.FinalNotes;
    }
    void HideOnPause()
    {
        if(currentPhase!=TutorialPhases.FinalNotes) return;
        textPause.SetActive(false);
        textPlayerTurn.SetActive(false);
    }
    void EndTutorial()
    {
        if (currentPhase != TutorialPhases.FinalNotes) return;
        ResetAllHighlights();
        textPlayerTurn.SetActive(false);
        textPause.SetActive(false);
        Destroy(gameObject);
    }
    void TryRaiseUnits()
    {
        if (PlayerData.Local != null)
        {
            FindAndRaiseUnits();
        }
        else
        {
            LeanTween.delayedCall(0.1f, TryRaiseUnits);
        }
    }
    private void FindAndRaiseUnits()
    {
        
        var yourUnits = FindObjectsByType<CharacterVisual>(FindObjectsSortMode.None)
            .Where(c => c.teamIndex == PlayerData.Local.TeamIndex.Value)
            .ToList();

        foreach (var unit in yourUnits)
        {
            SortingGroup sg = unit.GetComponent<SortingGroup>();

            if (sg == null)
            {
                sg = unit.gameObject.AddComponent<SortingGroup>();
                tempSortingGroups.Add(sg);
            }
            else
            {
                originalSortingStates[sg] = (sg.sortingLayerName, sg.sortingOrder);
            }

            sg.sortingLayerName = "Tutorial";
            sg.sortingOrder = 1; 
        }
    }
    void HighlightGameObject(GameObject go)
    {
        LeanTween.delayedCall(0f, () => {

            SortingGroup sg = go.GetComponent<SortingGroup>();

            if (sg == null)
            {
                sg = go.gameObject.AddComponent<SortingGroup>();
                tempSortingGroups.Add(sg);
            }
            else
            {
                originalSortingStates[sg] = (sg.sortingLayerName, sg.sortingOrder);
            }

            sg.sortingLayerName = "Tutorial";
            sg.sortingOrder = 1;
        });
            
        
    }
    public void HighlightUI(GameObject uiElement)
    {
        if(uiElement!=pip1 || uiElement!=pip2)
        uiElement.SetActive(true);
        LeanTween.delayedCall(0f, () => {

            Canvas canvas = uiElement.GetComponent<Canvas>();

            if (canvas == null)
            {
                canvas = uiElement.AddComponent<Canvas>();
                uiElement.AddComponent<GraphicRaycaster>();
                tempCanvases.Add(canvas);
            }
            else
            {
                originalCanvasStates[canvas] = (canvas.sortingLayerName, canvas.sortingOrder, canvas.overrideSorting);
            }

            canvas.overrideSorting = true;
            canvas.sortingLayerName = "Tutorial";
            canvas.sortingOrder = 1;
        });
        
    }

    private void ResetAllHighlights()
    {
        foreach (var sg in tempSortingGroups)
        {
            if (sg != null) Destroy(sg);
        }
        tempSortingGroups.Clear();

        foreach (var kvp in originalSortingStates)
        {
            if (kvp.Key != null)
            {
                kvp.Key.sortingLayerName = kvp.Value.layer;
                kvp.Key.sortingOrder = kvp.Value.order;
            }
        }
        originalSortingStates.Clear();

        foreach (var canvas in tempCanvases)
        {
            if (canvas != null)
            {
                Destroy(canvas.GetComponent<GraphicRaycaster>());
                Destroy(canvas);
            }
        }
        tempCanvases.Clear();

        foreach (var kvp in originalCanvasStates)
        {
            if (kvp.Key != null)
            {
                kvp.Key.sortingLayerName = kvp.Value.layer;
                kvp.Key.sortingOrder = kvp.Value.order;
                kvp.Key.overrideSorting = kvp.Value.overrideSort;
            }
        }
        originalCanvasStates.Clear();
    }
}