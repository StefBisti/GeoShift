using System;
using TMPro;
using UnityEngine;

public class AvailableTransformations : MonoBehaviour {
    public event Action<int> OnSelected;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private SelectedTransformations selectedTransformations;
    [SerializeField] private int[] availableTrCounts;
    [SerializeField] private RectTransform[] availableTrRTs;
    [SerializeField] private CanvasGroup[] availableTrCGs;
    [SerializeField] private TextMeshProUGUI[] availableTrTexts;
    [SerializeField] private Vector2 spaceBetween;
    [SerializeField] private float animationTime;
    [SerializeField] private LeanTweenType ease;

    private void OnEnable(){
        selectedTransformations.OnAddBack += AddBack;
        levelManager.OnLevelChanged += HandleOnLevelChanged;
    }

    private void OnDisable(){
        if(selectedTransformations != null){
            selectedTransformations.OnAddBack -= AddBack;
        }
        if(levelManager != null){
            levelManager.OnLevelChanged -= HandleOnLevelChanged;
        }
    }

    private void Start(){
        HandleOnLevelChanged(0);
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
    }

    public void Select(int index){
        if(availableTrCounts[index] == 0) return;
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
        }
        
        OnSelected?.Invoke(index);
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
        }
    }

    public Vector2 GetAddBackWorldPos(int index){
        int availableBefore = 0;
        for(int i=0; i<index; i++){
            if(availableTrCounts[i] > 0) availableBefore++;
        }

        Vector2 anchPos = availableBefore * spaceBetween;
        return transform.GetComponent<RectTransform>().TransformPoint(anchPos);
    }

    private void HandleOnLevelChanged(int level){
        Array.Copy(levelManager.GetLevelData(level).availableTransformations, availableTrCounts, availableTrCounts.Length);
        Setup();
    }
}