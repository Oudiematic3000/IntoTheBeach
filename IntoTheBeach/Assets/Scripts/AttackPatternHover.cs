using UnityEngine;

public class AttackPatternHover : Interactable
{
    protected override TooltipContent GetTooltipContent()
    {
        UnitClass unitClass;
        TooltipContent content = new TooltipContent();
        if (TryGetComponent<CharacterVisual>(out CharacterVisual character))
        {
            unitClass = character.unitClass;
        }
        else
        {
            unitClass = InputManager.Instance.GetCurrentSelection().unitClass;
        }
   
        content.sprite = unitClass.attackPatternPreview;
        return content;
    }
}
