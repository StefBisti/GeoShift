using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DeathScreen : MonoBehaviour {
    [SerializeField] private CanvasGroup canvasGroup, childCanvasGroup;
    [SerializeField] private Transform shadow, holder;
    [SerializeField] private LeanTweenType fadeEase, scaleEase;
    [SerializeField] private float fadeDuration, hideFadeDuration, scaleDuration, waitBeforeMoveToMenu;
    [SerializeField] private TextMeshProUGUI nextInText;
    [SerializeField] private string nextInFormat;
    private bool fullyOn = false;

    private void Awake(){
        Hearts.Instance.OnHeartRemoved += HandleHeartRemoved;
        LevelManager.Instance.OnEnteringLevelFailed += HandleEnterLevelFail;
    }
    private void OnDestroy(){
        if(Hearts.Instance != null){
            Hearts.Instance.OnHeartRemoved -= HandleHeartRemoved;
        }
        if(LevelManager.Instance != null){
            LevelManager.Instance.OnEnteringLevelFailed -= HandleEnterLevelFail;
        }
    }

    private void Start(){
        if(LevelManager.Instance.IsMain && Hearts.Instance.HeartsCount == 0){
            StartCoroutine(ShowDeathScreen());
        }
    }

    private void HandleHeartRemoved(int cnt){
        if(cnt == 0) {
            StartCoroutine(ShowDeathScreen());
        }
    }

    private void HandleEnterLevelFail(){
        StartCoroutine(ShowDeathScreen());
    }

    private IEnumerator ShowDeathScreen(){
        nextInText.text = string.Format(nextInFormat, GetNextInTime());
        canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
        childCanvasGroup.interactable = childCanvasGroup.blocksRaycasts = true;
        canvasGroup.LeanAlpha(1f, fadeDuration).setEase(fadeEase);
        yield return new WaitForSeconds(fadeDuration);
        shadow.LeanScale(Vector3.one, scaleDuration).setEase(scaleEase);
        holder.LeanScale(Vector3.one, scaleDuration).setEase(scaleEase).setOnComplete(() => fullyOn = true);
    }

    private void HideDeathScreen(){
        fullyOn = false;
        canvasGroup.LeanAlpha(0f, hideFadeDuration).setEase(fadeEase).setOnComplete(() => canvasGroup.SetCG(false));
        shadow.LeanScale(Vector3.zero, scaleDuration).setEase(scaleEase);
        holder.LeanScale(Vector3.zero, scaleDuration).setEase(scaleEase);
    }

    public void Refill(){
        Hearts.Instance.Refill();
        childCanvasGroup.interactable = childCanvasGroup.blocksRaycasts = false;
        HideDeathScreen();
    }

    public void OnPressOutside(){
        if(fullyOn == false) return;

        childCanvasGroup.interactable = childCanvasGroup.blocksRaycasts = false;
        fullyOn = false;
        
        shadow.LeanScale(Vector3.zero, scaleDuration).setEase(scaleEase);
        holder.LeanScale(Vector3.zero, scaleDuration).setEase(scaleEase);

        if(LevelManager.Instance.IsMain) {
            canvasGroup.LeanAlpha(0f, hideFadeDuration).setEase(fadeEase);
            this.DoAfterSeconds(waitBeforeMoveToMenu, () => LevelManager.Instance.GoToMenu());
        }
        else{
            canvasGroup.LeanAlpha(0f, hideFadeDuration).setEase(fadeEase).setOnComplete(() => canvasGroup.interactable = canvasGroup.blocksRaycasts = false);
        }
    }

    private string GetNextInTime(){
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
