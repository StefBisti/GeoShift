using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {
    [SerializeField] private CanvasGroup selfCanvasGroup;
    [SerializeField] private Transform content, shadow;
    [SerializeField] private float animationDuration;
    [SerializeField] private LeanTweenType scaleEase, fadeEase;
    [SerializeField] private RectTransform titleRT;
    [SerializeField] private string[] sectionsNames;
    [SerializeField] private TextMeshProUGUI titleText, diamondsText;
    [SerializeField] private string diamondsFormat;
    [SerializeField] private CanvasGroup[] sectionCGs;
    [SerializeField] private CanvasGroup leftButtonCG, rightButtonCG;
    [SerializeField] private ShopSection[] sections;
    [SerializeField] private RectTransform diamondsRT;
    [SerializeField] private float diamondsShakeDuration, diamondsShakeMagnitude;
    private Vector2 diamondsStartPos;
    private int sectionIndex = 0;

    private void Start(){
        diamondsStartPos = diamondsRT.anchoredPosition;
    }

    public void Open(){
        Audio.Instance.PlaySfx(AudioFx.ButtonPress);
        diamondsText.text = string.Format(diamondsFormat, Diamonds.Instance.DiamondsCount);
        SetSection(0);
        sectionIndex = 0;

        selfCanvasGroup.LeanAlpha(1f, animationDuration).setEase(fadeEase).setOnComplete(() => selfCanvasGroup.SetCG(true));
        content.LeanScale(Vector3.one, animationDuration).setEase(scaleEase);
        shadow.LeanScale(Vector3.one, animationDuration).setEase(scaleEase);
    }

    public void Close(){
        Audio.Instance.PlaySfx(AudioFx.ButtonPress);
        selfCanvasGroup.blocksRaycasts = selfCanvasGroup.interactable = false;
        selfCanvasGroup.LeanAlpha(0f, animationDuration).setEase(fadeEase);
        content.LeanScale(Vector3.zero, animationDuration).setEase(scaleEase);
        shadow.LeanScale(Vector3.zero, animationDuration).setEase(scaleEase);
    }

    public void ToLeft(){
        Audio.Instance.PlaySfx(AudioFx.ButtonPress);
        if(sectionIndex == 0) return;
        sectionIndex--;
        SetSection(sectionIndex);
    }

    public void ToRight(){
        Audio.Instance.PlaySfx(AudioFx.ButtonPress);
        if(sectionIndex == sectionCGs.Length - 1) return;
        sectionIndex++;
        SetSection(sectionIndex);
    }

    private void SetSection(int section){
        titleText.text = sectionsNames[section];
        LayoutRebuilder.ForceRebuildLayoutImmediate(titleRT);
        for(int i=0; i<sectionCGs.Length; i++){
            sectionCGs[i].SetCG(false);
        }
        sectionCGs[section].SetCG(true);

        leftButtonCG.SetCG(section != 0);
        rightButtonCG.SetCG(section != sectionCGs.Length - 1);
        sections[section].Initialize();
    }

    public void ShakeDiamonds() => StartCoroutine(ShakeDiamondsCoroutine());

    private IEnumerator ShakeDiamondsCoroutine(){
        float t = diamondsShakeDuration;
        while(t > 0){
            t -= Time.deltaTime;
            Vector2 offset = Random.insideUnitCircle.normalized * diamondsShakeMagnitude;
            diamondsRT.anchoredPosition = diamondsStartPos + offset;
            yield return null;
        }
        diamondsRT.anchoredPosition = diamondsStartPos;
    }

    public void UpdateDiamonds() => diamondsText.text = string.Format(diamondsFormat, Diamonds.Instance.DiamondsCount);
}
