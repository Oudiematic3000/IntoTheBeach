using UnityEngine;

public class AttackPatternHover : Interactable
{
    protected override TooltipContent GetTooltipContent()
    {
        var unitClass = InputManager.Instance.GetCurrentSelection().unitClass;
        TooltipContent content = new TooltipContent();
        content.sprite = unitClass.attackPatternPreview;
        return content;
    }
}
