using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField]Vector3 offsetPosition = new Vector3(1f, 1f,0);
    [SerializeField] float distance = 2f;
    [SerializeField] CanvasGroup group;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    [SerializeField] float duration = 1f;
    void Start()
    {
        
    }
    public void Animate(int damage)
    {
        textMeshProUGUI.text=damage.ToString();
        transform.position = transform.position + offsetPosition;
        transform.LeanMove(transform.position + offsetPosition + Vector3.up * 2f, duration);
        group.LeanAlpha(0, duration);
        LeanTween.delayedCall(duration, () => {Destroy(gameObject); });
    }
}
