using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using TMPro;
using Unity.Burst.Intrinsics;
using System;

public class HeartsUI : MonoBehaviour {
    [SerializeField] private Image[] images;
    [SerializeField] private Sprite emptySprite, fullSprite;
    [SerializeField] private float shakeDuration, shakeMagnitude;
    [SerializeField] private RectTransform feedbackRT;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private CanvasGroup feedbackCG;
    [SerializeField] private float feedbackAnimDuration, feedbackStayOn, feedbackUpY, feedbackDownY;
    [SerializeField] private LeanTweenType feedbackFadeEase, feedbackMoveEase;
    private RectTransform rt;
    private Vector2 startPos;
    private float feedBackX, feedbackTimer;
    private bool feedbackActive = false;


    private void Awake(){
        Hearts.Instance.OnHeartRemoved += HandleOnHeartRemoved;
        Hearts.Instance.OnHeartsCountChanged += ChangeHeartsCount;
        rt = GetComponent<RectTransform>();
        startPos = rt.anchoredPosition;
        feedBackX = feedbackRT.anchoredPosition.x;
    }
    private void OnDestroy(){
        if(Hearts.Instance != null) {
            Hearts.Instance.OnHeartRemoved -= HandleOnHeartRemoved;
            Hearts.Instance.OnHeartsCountChanged -= ChangeHeartsCount;
        }
    }

    private void Start(){
        ChangeHeartsCount(Hearts.Instance.HeartsCount);
    }

    private void Update(){
        if(feedbackTimer > 0f){
            feedbackTimer -= Time.deltaTime;
            if(feedbackTimer <= 0f){
                HideFeedback();
            }
        }
    }

    private void HandleOnHeartRemoved(int count){
        StartCoroutine(ShakeHearts());
        ChangeHeartsCount(count);
    }

    private void ChangeHeartsCount(int count){
        for(int i=0; i<images.Length; i++){
            images[i].sprite = i < count ? fullSprite : emptySprite;
        }
    }

    private IEnumerator ShakeHearts(){
        float t = shakeDuration;
        while(t > 0){
            t -= Time.deltaTime;
            Vector2 offset = UnityEngine.Random.insideUnitCircle.normalized * shakeMagnitude;
            rt.anchoredPosition = startPos + offset;
            yield return null;
        }
        rt.anchoredPosition = startPos;
    }

    public void ShowFeedback(){
        if(feedbackActive){
            HideFeedback();
            return;
        }
        feedbackActive = true;
        feedbackText.text = GetFeedbackText();
        feedbackCG.LeanAlpha(1f, feedbackAnimDuration).setEase(feedbackFadeEase);
        LeanTween.value(gameObject, ChangeFeedbackPos, feedbackUpY, feedbackDownY, feedbackAnimDuration).setEase(feedbackMoveEase);
        feedbackTimer = feedbackStayOn;
    }
    private void HideFeedback(){
        if(feedbackActive == false) return;
        feedbackActive = false;
        
        feedbackCG.LeanAlpha(0f, feedbackAnimDuration).setEase(feedbackFadeEase);
        LeanTween.value(gameObject, ChangeFeedbackPos, feedbackDownY, feedbackUpY, feedbackAnimDuration).setEase(feedbackMoveEase);
    }

    private void ChangeFeedbackPos(float y) => feedbackRT.anchoredPosition = new Vector2(feedBackX, y);


    private string GetFeedbackText(){
        if (Hearts.Instance.IsFull)
            return "Full";
    
        string lastHeartLostTimeString = PlayerPrefs.GetString("LastHeartLostTime", DateTime.UtcNow.ToString());
        DateTime lastHeartLostTime = DateTime.Parse(lastHeartLostTimeString);

        DateTime nextRefillTime = lastHeartLostTime.AddHours(Hearts.Instance.HoursToRefill);
        TimeSpan timeRemaining = nextRefillTime - DateTime.UtcNow;

        if (timeRemaining.TotalSeconds < 0)
            timeRemaining = TimeSpan.Zero;

        string timeRemainingString = string.Format("{0:D2}:{1:D2}", timeRemaining.Hours, timeRemaining.Minutes);
        return timeRemainingString;
    }
}
