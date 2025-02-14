using UnityEngine;

[RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
public class MoveInOut : MonoBehaviour {
    [SerializeField] private Vector2 outAnchoredPos;
    [SerializeField] private LeanTweenType ease;
    [SerializeField] private bool disappear;
    private Vector2 startPos;
    private RectTransform rt;
    private CanvasGroup cg;

    private void Awake() {
        rt = GetComponent<RectTransform>();
        startPos = rt.anchoredPosition;
        cg = GetComponent<CanvasGroup>();
        LevelManager.Instance.OnRequestedExit += HandleExit;
        LevelManager.Instance.OnEntered += HandleEnter;
    }

    private void OnDestroy(){
        if(LevelManager.Instance != null){
            LevelManager.Instance.OnRequestedExit -= HandleExit;
            LevelManager.Instance.OnEntered -= HandleEnter;
        }
    }

    private void HandleExit(float animTime){
        if(disappear){
            cg.alpha = 0f;
            return;
        }
        cg.blocksRaycasts = cg.interactable = false;
        LeanTween.value(gameObject, SetRtPos, startPos, outAnchoredPos, animTime).setEase(ease).setOnComplete(() => cg.alpha = 0f);
    }

    private void HandleEnter(float animTime){
        if(disappear){
            return;
        }
        LeanTween.value(gameObject, SetRtPos, outAnchoredPos, startPos, animTime).setEase(ease).setOnComplete(() => cg.blocksRaycasts = cg.interactable = true);
    }

    private void SetRtPos(Vector2 anchoredPos) => rt.anchoredPosition = anchoredPos;
}
