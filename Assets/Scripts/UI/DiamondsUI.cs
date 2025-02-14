using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiamondsUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private GameObject icon;
    [SerializeField] private RectTransform iconRT;
    
    [Header("RT hide show animation")]
    [SerializeField] private RectTransform rt;
    [SerializeField] private float hiddenX, rtShowDuration, rtStayOnDuration;
    [SerializeField] private LeanTweenType rtShowEase;
    private Vector2 goToPos;
    private float shownX, timer;
    private bool on = false;

    [Header("Scale Animation")]
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private float maxScale, scaleDuration;
    
    [Header("Move")]
    [SerializeField] private LeanTweenType moveEase;
    [SerializeField] private float moveDuration, waitBeforeFade;
    [SerializeField] private LeanTweenType fadeEase;

    private void Awake(){
        Diamonds.Instance.OnDiamondCountChanged += HandleDiamondCountChanged;
        Diamonds.Instance.OnDiamondGot += HandleOnDiamondGot;
        LevelManager.Instance.OnRequestedExit += HandleGoToMenu;
    }
    private void OnDestroy(){
        if(Diamonds.Instance != null){
            Diamonds.Instance.OnDiamondCountChanged -= HandleDiamondCountChanged;
            Diamonds.Instance.OnDiamondGot -= HandleOnDiamondGot;
        }
        if(LevelManager.Instance != null){
            LevelManager.Instance.OnRequestedExit -= HandleGoToMenu;
        }
    }

    private void Start(){
        valueText.text = Diamonds.Instance.DiamondsCount.ToString();
        shownX = rt.anchoredPosition.x;
        goToPos = iconRT.position;
        rt.anchoredPosition = new Vector2(hiddenX, rt.anchoredPosition.y);
    }

    private void Update(){
        if(timer > 0f){
            timer -= Time.deltaTime;
            if(timer <= 0f){
                ToggleVisual(false);
            }
        }
    }

    private void ToggleVisual(bool on){
        if(on == this.on) return;
        this.on = on;
        LeanTween.value(rt.gameObject, ChangeRTX, on ? hiddenX : shownX, on ? shownX : hiddenX, rtShowDuration).setEase(rtShowEase);
    }
    private void ChangeRTX(float v) => rt.anchoredPosition = new Vector2(v, rt.anchoredPosition.y);

    private void HandleDiamondCountChanged(int diamonds){
        icon.LeanScale(Vector3.one * maxScale, scaleDuration).setEase(scaleCurve).setOnComplete(() => icon.transform.localScale = Vector3.one);
        valueText.text = diamonds.ToString();
    }

    private void HandleOnDiamondGot(Vector2 worldPos){
        RectTransform newDiamond = Instantiate(iconRT, transform);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);
        newDiamond.position = screenPos;
        StartCoroutine(AnimateDiamond(newDiamond));
        ToggleVisual(true);
        timer = rtStayOnDuration;
    }

    private IEnumerator AnimateDiamond(RectTransform diamond){
        float startY = diamond.position.y;
        LeanTween.value(diamond.gameObject, v => Move(v, diamond), (Vector2)diamond.position, goToPos, moveDuration).setEase(moveEase).setOnComplete(() => {
            HandleDiamondCountChanged(Diamonds.Instance.DiamondsCount);
            Destroy(diamond.gameObject);
        });
        yield return new WaitForSeconds(waitBeforeFade);
        diamond.GetComponent<CanvasGroup>().LeanAlpha(0f, moveDuration).setEase(fadeEase);
    }

    private void Move(Vector2 pos, RectTransform diamond) => diamond.position = pos;

    private void HandleGoToMenu(float _){
        ToggleVisual(false);
    }
}
