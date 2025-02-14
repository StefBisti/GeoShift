using System.Collections;
using UnityEngine;

public class Gift : MonoBehaviour {
    [SerializeField] private CanvasGroup canvasGroup, backgroundCG;
    [SerializeField] private RectTransform giftRT;
    [SerializeField] private float minScale, moveDuration, waitBeforeStartFade, fadeDuration;
    [SerializeField] private LeanTweenType scaleEase, moveEase, fadeEase;

    [Header("Jump")]
    [SerializeField] private int maxJumps;
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float jumpAnimationDuration;
    [SerializeField] private RectTransform giftTop, giftBottom, giftShadow;
    [SerializeField] private CanvasGroup giftTopCG, giftBottomCG, giftShadowCG;
    [SerializeField] private float topJumpY, topJumpX, topjumpRotate;
    [SerializeField] private float bottomJumpY, bottomJumpRotate;
    [SerializeField] private Vector2 shadowMovePos;
    [SerializeField] private float endTopY, endFadeDuration;
    [SerializeField] private AnimationCurve endEase;

    [Header("Rewards")]
    [SerializeField] private float timeBetweenDiamonds;
    [SerializeField] private int minDiamonds, maxDiamonds;


    private Vector2 bottomStartPos, topStartPos, shadowStartPos;
    private int currentJump = 0;

    
    private void Awake(){
        Diamonds.Instance.OnGiftGot += HandleOnGiftGot;
    }
    private void OnDestroy(){
        if(Diamonds.Instance != null){
            Diamonds.Instance.OnGiftGot -= HandleOnGiftGot;
        }
    }

    private void Start(){
        bottomStartPos = giftBottom.anchoredPosition;
        topStartPos = giftTop.anchoredPosition;
        shadowStartPos = giftTop.anchoredPosition;
    }

    private void HandleOnGiftGot(Vector2 pos){
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
        canvasGroup.SetCG(true);
        giftTopCG.alpha = giftBottomCG.alpha = giftShadowCG.alpha = 1f;
        giftRT.position = screenPos;
        giftRT.localScale = Vector3.one * minScale;
        giftRT.LeanScale(Vector3.one, moveDuration).setEase(scaleEase);
        giftRT.LeanMove(Vector3.zero, moveDuration).setEase(moveEase);

        backgroundCG.LeanAlpha(1f, fadeDuration).setEase(fadeEase).setDelay(waitBeforeStartFade).setOnComplete(() => backgroundCG.interactable = backgroundCG.blocksRaycasts = true);
    }

    public void OnClick(){
        if(currentJump < maxJumps){
            currentJump++;
            AnimateTop();
            AnimateBottom();
            AnimateShadow();
            Audio.Instance.PlaySfx(AudioFx.BoxOpening);
        }
    }

    private void AnimateTop(){
        Vector2 maxPos = topStartPos + new Vector2(topJumpX * (currentJump % 2 * 2 - 1), currentJump == maxJumps ? endTopY : topJumpY);
        float currentRot = giftTop.localEulerAngles.z;
        float maxRot = currentRot + (currentJump % 2 * 2 - 1) * topjumpRotate;
        LeanTween.value(giftTop.gameObject, v => UpdateRT(giftTop, v), topStartPos, maxPos, jumpAnimationDuration).setEase(currentJump == maxJumps ? endEase : jumpCurve);
        LeanTween.value(giftTop.gameObject, v => UpdateRot(giftTop, v), currentRot, maxRot, jumpAnimationDuration).setEase(currentJump == maxJumps ? endEase : jumpCurve);

        if(currentJump == maxJumps){
            giftTopCG.LeanAlpha(0f, endFadeDuration).setEase(endEase);
        }
    }

    private void AnimateBottom(){
        Vector2 maxPos = bottomStartPos + new Vector2(0, bottomJumpY);
        LeanTween.value(giftBottom.gameObject, v => UpdateRT(giftBottom, v), bottomStartPos, maxPos, jumpAnimationDuration).setEase(jumpCurve);

        if(currentJump == maxJumps){
            StartCoroutine(GiveDiamonds());
            giftBottomCG.LeanAlpha(0f, endFadeDuration).setEase(endEase);
        }
    }

    private void AnimateShadow(){
        Vector2 maxPos = shadowStartPos + shadowMovePos;
        LeanTween.value(giftShadow.gameObject, v => UpdateRT(giftShadow, v), shadowStartPos, maxPos, jumpAnimationDuration).setEase(jumpCurve);

        if(currentJump == maxJumps){
            giftShadowCG.LeanAlpha(0f, endFadeDuration).setEase(endEase);
        }
    }

    private void UpdateRT(RectTransform rt, Vector2 pos) => rt.anchoredPosition = pos;
    private void UpdateRot(RectTransform rt, float rot) => rt.localEulerAngles = Vector3.forward * rot;


    private IEnumerator GiveDiamonds(){
        backgroundCG.LeanAlpha(0f, fadeDuration).setEase(fadeEase).setOnComplete(() => Reset());
        int diamonds = Random.Range(minDiamonds, maxDiamonds);
        for(int i=0; i<diamonds; i++){
            Diamonds.Instance.AddDiamond(Vector2.zero);
            Audio.Instance.PlaySfx(AudioFx.Collect);
            yield return new WaitForSeconds(timeBetweenDiamonds);
        }
    }

    private void Reset(){
        backgroundCG.SetCG(false);
        canvasGroup.SetCG(false);
        giftTop.anchoredPosition = topStartPos;
        giftBottom.anchoredPosition = bottomStartPos;
        giftShadow.anchoredPosition = shadowStartPos;
        giftTop.localEulerAngles = giftBottom.localEulerAngles = Vector3.zero;
        currentJump = 0;
        Diamonds.Instance.EndGift();
    }
}
