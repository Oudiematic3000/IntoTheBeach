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

    private int animationTweenId = -1;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        restPosition = rectTransform.anchoredPosition;
        centerScreen = new Vector2(-Screen.width / 2f, -Screen.height / 2f);
        offscreenLeft = new Vector2(-Screen.width, centerScreen.y);

        MovePlanTurnState.OnMovePlanStart += AnimateMovePhase;
        AttackPlanTurnState.OnAttackPlanStart += AnimateAttackPhase;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        MovePlanTurnState.OnMovePlanStart -= AnimateMovePhase;
        AttackPlanTurnState.OnAttackPlanStart -= AnimateAttackPhase;

        ResetAnimationState();
    }

    void AnimateMovePhase()
    {
        if (!movePhase) return;

        ResetAnimationState();
        gameObject.SetActive(true);
        PlayAnimationAnnounce();

        if (AudioManager.instance != null && announceSound != null)
            AudioManager.instance.PlaySFX(announceSound);
    }

    void AnimateAttackPhase()
    {
        if (movePhase) return;

        ResetAnimationState();
        gameObject.SetActive(true);
        PlayAnimationAnnounce();
    }

    public void PlayAnimationAnnounce()
    {
        announceImage.gameObject.SetActive(true);
        currentFrame = 0;
        AdvanceFrame();
        LeanTween.delayedCall(5f, () => { announceImage.gameObject.SetActive(false); });
    }

    private void AdvanceFrame()
    {
        if (animationFrames == null || animationFrames.Length == 0)
        {
            EndAnimation();
            return;
        }

        announceImage.sprite = animationFrames[currentFrame];

        if (currentFrame == 6 && !movePhase && AudioManager.instance != null && announceSound != null)
        {
            AudioManager.instance.PlaySFX(announceSound);
        }

        currentFrame++;

        if (currentFrame < animationFrames.Length)
        {
            animationTweenId = LeanTween.delayedCall(gameObject, speed, AdvanceFrame).id;
        }
        else
        {
            EndAnimation();
        }
    }

  
    private void ResetAnimationState()
    {
        LeanTween.cancel(gameObject);
        animationTweenId = -1;

        if (announceImage != null)
            announceImage.gameObject.SetActive(false);

        if (rectTransform != null)
            rectTransform.anchoredPosition = restPosition;
    }

    private void EndAnimation()
    {
        ResetAnimationState();
        gameObject.SetActive(false);
    }
}