using System;
using TMPro;
using UnityEngine;

public class AvailableTransformations : MonoBehaviour {
    public event Action<int> OnSelected;
    [SerializeField] private MainShape mainShape;
    [SerializeField] private ShapeTransformations shapeTransformations;
    [SerializeField] private SelectedTransformations selectedTransformations;
    [SerializeField] private int[] availableTrCounts;
    [SerializeField] private RectTransform[] availableTrRTs, extraTrRTs;
    [SerializeField] private CanvasGroup[] availableTrCGs, extraTrCGS, plusesCGs;
    [SerializeField] private TextMeshProUGUI[] availableTrTexts, extraTrTexts;
    [SerializeField] private Vector2 spaceBetween;
    [SerializeField] private float animationTime;
    [SerializeField] private LeanTweenType ease;
    [SerializeField] private float extraTrAlpha;
    private bool canChoose = true;

    private void Awake(){
        selectedTransformations.OnAddBack += AddBack;
        LevelManager.Instance.OnLevelChanged += HandleOnLevelChanged;
        ExtraTransformations.Instance.OnExtraTransformationAdded += HandleExtrasChanged;
        mainShape.OnDeath += HandleShapeDeath;
        mainShape.OnReset += HandleShapeReset;
    }

    private void OnDestroy(){
        if(selectedTransformations != null){
            selectedTransformations.OnAddBack -= AddBack;
        }
        if(LevelManager.Instance != null){
            LevelManager.Instance.OnLevelChanged -= HandleOnLevelChanged;
        }
        if(ExtraTransformations.Instance != null){
            ExtraTransformations.Instance.OnExtraTransformationAdded -= HandleExtrasChanged;
        }
        if(mainShape != null){
            mainShape.OnDeath -= HandleShapeDeath;
            mainShape.OnReset -= HandleShapeReset;
        }
    }

    private void Start(){
        HandleOnLevelChanged(LevelManager.Instance.Level);
    }

    private void Setup(){
        int index = 0;
        for(int i=0; i<availableTrCounts.Length; i++){
            availableTrTexts[i].text = availableTrCounts[i].ToString();
            if(availableTrCounts[i] == 0){
                availableTrCGs[i].SetCG(false);
            }
            else {
                availableTrCGs[i].SetCG(true);
                availableTrRTs[i].anchoredPosition = index * spaceBetween;
                index++;
            }
        }
        int[] extraTr = ExtraTransformations.Instance.ExtraTransformationsCounts;
        for(int i=0; i<extraTr.Length; i++){
            extraTrTexts[i].text = extraTr[i].ToString();
            if(extraTr[i] > 0 && availableTrCounts[i] == 0){
                extraTrCGS[i].SetCG(true);
                plusesCGs[i].alpha = 1f;
                extraTrCGS[i].alpha = extraTrAlpha;
                extraTrRTs[i].anchoredPosition = index * spaceBetween;
                index++;
            }
            else{
                extraTrCGS[i].SetCG(false);
                plusesCGs[i].alpha = 0f;
            }
        }
    }

    public void Select(int index){
        if(canChoose == false) return;
        int[] extraTr = ExtraTransformations.Instance.ExtraTransformationsCounts;

        if(availableTrCounts[index] != 0){
            availableTrCounts[index]--;
            availableTrTexts[index].text = availableTrCounts[index].ToString();
            if(availableTrCounts[index] == 0){
                availableTrCGs[index].SetCG(false);
                for(int i=index+1; i<availableTrCounts.Length; i++){
                    if(availableTrCounts[i] > 0){
                        Vector2 endPos = availableTrRTs[i].anchoredPosition - spaceBetween;
                        availableTrRTs[i].LeanMoveLocal(endPos, animationTime).setEase(ease);
                    }
                }
                
                for(int i=0; i<extraTrRTs.Length; i++){
                    if(i != index && extraTr[i] > 0 && availableTrCounts[i] == 0){
                        Vector2 endPos = extraTrRTs[i].anchoredPosition - spaceBetween;
                        extraTrRTs[i].LeanMoveLocal(endPos, animationTime).setEase(ease);
                    }
                }
            }
            this.DoAfterSeconds(animationTime, () => ShowExtra(index));
            shapeTransformations.SelectTransformation(index);
            OnSelected?.Invoke(index);
        }
        else if(extraTr[index] > 0){
            ExtraTransformations.Instance.TryUseExtraTransformation(index);
            extraTrTexts[index].text = extraTr[index].ToString();
            if(extraTr[index] == 0){
                extraTrCGS[index].SetCG(false);
                plusesCGs[index].SetCG(false);
                for(int i=index+1; i<extraTr.Length; i++){
                    if(extraTr[i] > 0 && availableTrCounts[i] == 0){
                        Vector2 endPos = extraTrRTs[i].anchoredPosition - spaceBetween;
                        extraTrRTs[i].LeanMoveLocal(endPos, animationTime).setEase(ease);
                    }
                }
            }
            shapeTransformations.SelectTransformation(index);
            OnSelected?.Invoke(index);
        }
        
    }

    private void ShowExtra(int index){
        int[] extraTr = ExtraTransformations.Instance.ExtraTransformationsCounts;
        if(availableTrCounts[index] != 0 || extraTr[index] == 0) return;

        extraTrTexts[index].text = extraTr[index].ToString();
        extraTrCGS[index].SetCG(true);
        extraTrCGS[index].alpha = extraTrAlpha;
        plusesCGs[index].alpha = 1f;

        int availableBefore = 0;
        for(int i=0; i<availableTrCounts.Length; i++){
            if(availableTrCounts[i] > 0) availableBefore++;
        }
        for(int i=0; i<index; i++){
            if(extraTr[i] > 0 && availableTrCounts[i] == 0) availableBefore++;
        }
        extraTrRTs[index].anchoredPosition = availableBefore * spaceBetween;
        for(int i=index+1; i<extraTr.Length; i++){
            if(extraTr[i] > 0 && availableTrCounts[i] == 0){
                Vector2 endPos = extraTrRTs[i].anchoredPosition + spaceBetween;
                extraTrRTs[i].LeanMoveLocal(endPos, animationTime).setEase(ease);
            }
        }
    }

    private void AddBack(int index){
        availableTrCounts[index]++;
        availableTrTexts[index].text = availableTrCounts[index].ToString();
        if(availableTrCounts[index] == 1){
            availableTrCGs[index].SetCG(true);
            int availableBefore = 0;
            for(int i=0; i<index; i++){
                if(availableTrCounts[i] > 0) availableBefore++;
            }
            availableTrRTs[index].anchoredPosition = availableBefore * spaceBetween;

            for(int i=index+1; i<availableTrCounts.Length; i++){
                if(availableTrCounts[i] > 0){
                    Vector2 endPos = availableTrRTs[i].anchoredPosition + spaceBetween;
                    availableTrRTs[i].LeanMoveLocal(endPos, animationTime).setEase(ease);
                }
            }

            extraTrCGS[index].SetCG(false);
            plusesCGs[index].alpha = 0f;

            int[] extraTr = ExtraTransformations.Instance.ExtraTransformationsCounts;
            for(int i=0; i<index; i++){
                if(extraTr[i] > 0 && availableTrCounts[i] == 0){
                    Vector2 endPos = extraTrRTs[i].anchoredPosition + spaceBetween;
                    extraTrRTs[i].LeanMoveLocal(endPos, animationTime).setEase(ease);
                }
            }
        }
    }

    private void HandleExtrasChanged(int[] extraTr) => Setup();

    public Vector2 GetAddBackWorldPos(int index){
        int availableBefore = 0;
        for(int i=0; i<index; i++){
            if(availableTrCounts[i] > 0) availableBefore++;
        }

        Vector2 anchPos = availableBefore * spaceBetween;
        return transform.GetComponent<RectTransform>().TransformPoint(anchPos);
    }

    private void HandleOnLevelChanged(int level){
        Array.Copy(LevelManager.Instance.GetLevelData(level).availableTransformations, availableTrCounts, availableTrCounts.Length);
        int[] alreadyUsedExtras = ExtraTransformations.Instance.GetUsedExtraTransformations(LevelManager.Instance.Level);
        for(int i=0; i<alreadyUsedExtras.Length; i++){
            availableTrCounts[i] += alreadyUsedExtras[i];
        }
        Setup();
    }

    private void HandleShapeDeath(){
        canChoose = false;
    }

    private void HandleShapeReset(){
        Array.Copy(LevelManager.Instance.GetCurrentLevelData().availableTransformations, availableTrCounts, availableTrCounts.Length);
        int[] alreadyUsedExtras = ExtraTransformations.Instance.GetUsedExtraTransformations(LevelManager.Instance.Level);
        for(int i=0; i<alreadyUsedExtras.Length; i++){
            availableTrCounts[i] += alreadyUsedExtras[i];
        }
        Setup();
        canChoose = true;
    }
}