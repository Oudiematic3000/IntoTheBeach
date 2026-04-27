using UnityEngine;

public class PhaseChangeAnimator : MonoBehaviour
{
    public bool movePhase = true;
    [SerializeField] float slideDuration = 0.5f;
    [SerializeField] float lingerDuration = 0.5f;

    private RectTransform rectTransform;
    private Vector2 restPosition;      
    private Vector2 offscreenLeft;
    private Vector2 centerScreen;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        restPosition = rectTransform.anchoredPosition;
        centerScreen = new Vector2(-Screen.width / 2f, -Screen.height / 2f);
        offscreenLeft = new Vector2(-Screen.width, centerScreen.y);

        MovePlanTurnState.OnMovePlanStart += AnimateMovePhase;
        AttackPlanTurnState.OnAttackPlanStart += AnimateAttackPhase;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        MovePlanTurnState.OnMovePlanStart -= AnimateMovePhase;
        AttackPlanTurnState.OnAttackPlanStart -= AnimateAttackPhase;
    }


    void AnimateMovePhase()
    {
        if (!movePhase) return;
        movePhase = false;
        gameObject.SetActive(true);
        PlayAnimation();
    }

    void AnimateAttackPhase()
    {
        if (movePhase) return;
        movePhase = true;
        gameObject.SetActive(true);
        PlayAnimation();
    }

    void PlayAnimation()
    {
        Debug.Log("PlayAnimation");
        rectTransform.anchoredPosition = offscreenLeft;
        LeanTween.move(rectTransform, centerScreen, slideDuration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                LeanTween.delayedCall(lingerDuration, () =>
                {
                    LeanTween.move(rectTransform, restPosition, slideDuration)
                        .setEase(LeanTweenType.easeInOutQuad);
                });
            });
    }
}