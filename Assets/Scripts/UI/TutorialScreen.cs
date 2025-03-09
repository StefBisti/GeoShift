using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialScreen : MonoBehaviour {
    [SerializeField, TextArea] private string[] tutorialTexts;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform shadow, holder;
    [SerializeField] private LeanTweenType fadeEase, scaleEase;
    [SerializeField] private float fadeDuration, hideFadeDuration, scaleDuration;
    private bool fullyOn = false;


    public void Show(){
        StartCoroutine(ShowTutorialScreen());
    }

    private IEnumerator ShowTutorialScreen(){
        tutorialText.text = tutorialTexts[LevelManager.Instance.Level];
        canvasGroup.LeanAlpha(1f, fadeDuration).setEase(fadeEase);
        yield return new WaitForSeconds(fadeDuration);
        shadow.LeanScale(Vector3.one, scaleDuration).setEase(scaleEase);
        holder.LeanScale(Vector3.one, scaleDuration).setEase(scaleEase).setOnComplete(() => {
            fullyOn = true;
            canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
        });
    }

    private void HideTutorialScreen(){
        fullyOn = false;
        canvasGroup.LeanAlpha(0f, hideFadeDuration).setEase(fadeEase).setOnComplete(() => canvasGroup.SetCG(false));
        shadow.LeanScale(Vector3.zero, scaleDuration).setEase(scaleEase);
        holder.LeanScale(Vector3.zero, scaleDuration).setEase(scaleEase);
    }

    public void OnPressOutside(){
        if(fullyOn == false) return;

        HideTutorialScreen();
    }
}
