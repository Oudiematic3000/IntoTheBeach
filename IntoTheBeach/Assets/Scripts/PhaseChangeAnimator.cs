using UnityEngine;
using UnityEngine.UI;

public class PhaseChangeAnimator : MonoBehaviour
{
    public bool movePhase = true;
    [SerializeField] float slideDuration = 0.5f;
    [SerializeField] float lingerDuration = 0.5f;

    private RectTransform rectTransform;
    private Vector2 restPosition;      
    private Vector2 offscreenLeft;
    private Vector2 centerScreen;

    public Image announceImage;
    public AudioClip announceSound;

    public Sprite[] animationFrames;
    [SerializeField] float speed;
    int currentFrame = 0;

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
        //movePhase = false;
        gameObject.SetActive(true);
        PlayAnimationAnnounce();
        AudioManager.instance.PlaySFX(announceSound);

    }

    void AnimateAttackPhase()
    {
        if (movePhase) return;
        //movePhase = true;
        gameObject.SetActive(true);
        PlayAnimationAnnounce();
    }
    public void PlayAnimationAnnounce()
    {
        announceImage.gameObject.SetActive(true);
        currentFrame = 0;
        AdvanceFrame();
    }
    private void AdvanceFrame()
    {
        if (animationFrames == null || animationFrames.Length == 0) return;

        announceImage.sprite = animationFrames[currentFrame];
        if(currentFrame==6 &&!movePhase) AudioManager.instance.PlaySFX(announceSound);

        currentFrame++;
        
        if (currentFrame < animationFrames.Length)
            LeanTween.delayedCall(speed, AdvanceFrame);
        else
        {
            announceImage.gameObject.SetActive(false);
            // PlayAnimation();
            rectTransform.anchoredPosition = restPosition;
        }
            

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