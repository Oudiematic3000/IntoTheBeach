using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Interactable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected Image image;
    protected RectTransform rectTransform;
    protected Canvas canvas;
    protected CanvasGroup canvasGroup;

    public bool inSlot = false;
    public Transform originalParent;

    [SerializeField] bool showTooltip = true;


    LTDescr delay;
    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        originalParent=transform.parent;
    }

    void Update()
    {
        
    }

    protected virtual TooltipContent GetTooltipContent()
    {
        TooltipContent content = new TooltipContent();

        return content;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (showTooltip)
        {
            delay = LeanTween.delayedCall(0.45f, () =>
             {
                 TooltipManager.instance.Show(eventData.position, GetTooltipContent());
             });
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(delay.uniqueId);
        TooltipManager.instance.Hide();
    }


}
