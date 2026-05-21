using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;
    [SerializeField] Tooltip tooltip;
    [SerializeField] Tooltip staticTooltip;

    private void Awake()
    {
        if (instance == null) { instance = this; } else { Destroy(this); }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Show(Vector2 position, TooltipContent content)
    {
        tooltip.gameObject.SetActive(true);
        tooltip.canvasGroup.LeanAlpha(1, 0.15f);
        tooltip.SetContent(content);
    }
    public void ShowStatic(TooltipContent content)
    {
        staticTooltip.gameObject.SetActive(true);
        staticTooltip.canvasGroup.LeanAlpha(1, 0.15f);
        staticTooltip.SetContent(content);
    }
    public void Hide()
    {
        tooltip.canvasGroup.LeanAlpha(0, 0.05f);
        LeanTween.delayedCall(0.05f, () => { tooltip.gameObject.SetActive(false); });
    }
    public void HideStatic()
    {
        staticTooltip.canvasGroup.LeanAlpha(0, 0.05f);
        LeanTween.delayedCall(0.05f, () => { staticTooltip.gameObject.SetActive(false); });
    }
}
