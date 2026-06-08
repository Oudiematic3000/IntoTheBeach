using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] Vector3 offsetPosition = new Vector3(2f, 2f, 0);
    [SerializeField] CanvasGroup group;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    [SerializeField] float duration = 1f;

    public void Animate(int damage, Vector3 worldPosition)
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        Camera cam = Camera.main;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam,
            out Vector2 localPoint
        );

        RectTransform rt = GetComponent<RectTransform>();
        rt.localPosition = localPoint + new Vector2(offsetPosition.x, offsetPosition.y);

        textMeshProUGUI.text = damage.ToString();

        Vector3 targetPos = rt.localPosition + new Vector3(0, 150f, 0);
        rt.LeanMoveLocal(targetPos, duration);
        group.LeanAlpha(0, duration);
        LeanTween.delayedCall(duration, () => Destroy(gameObject));
    }
}