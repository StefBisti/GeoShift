using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Burst.Intrinsics;

public class SelectedTransformations : MonoBehaviour {
    public event Action<int> OnAddBack;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private AvailableTransformations availableTransformations;
    [SerializeField] private Vector2 spaceBetween;
    [SerializeField] private Sprite[] transformationSprites;
    [SerializeField] private float disabledAlpha;
    [SerializeField] private RectTransform pointerRT;
    [SerializeField] private CanvasGroup pointerCG;
    [SerializeField] private CustomSlider slider;
    [SerializeField] private TransformationsSO transformationsSO;
    //[SerializeField] private MainShapeBehaviour mainShape;
    private float pointerY;
    private int[] selectedTransformations = new int[20];
    private int selectedTransformationsCount = 0;
    private RectTransform prefab;

    private void OnEnable(){
        availableTransformations.OnSelected += HandleOnSelected;
        levelManager.OnLevelChanged += HandleOnLevelChanged;
    }

    private void OnDisable(){
        if(availableTransformations != null){
            availableTransformations.OnSelected -= HandleOnSelected;
        }
        if(levelManager != null){
            levelManager.OnLevelChanged -= HandleOnLevelChanged;
        }
    }

    private void Start(){
        pointerY = pointerRT.anchoredPosition.y;
        pointerCG.SetCG(false);

        prefab = transform.GetChild(1).GetComponent<RectTransform>();
        prefab.GetComponent<CanvasGroup>().SetCG(false);
    }

    private void HandleOnSelected(int index){
        RectTransform newTr = Instantiate(prefab, transform);
        newTr.name = "Transformation";
        newTr.anchoredPosition = spaceBetween * selectedTransformationsCount;
        selectedTransformationsCount++;
        newTr.GetChild(0).GetComponent<Image>().sprite = transformationSprites[index];

        newTr.GetComponent<CanvasGroup>().SetCG(true);
        for(int i=2; i<selectedTransformationsCount + 1; i++) {
            CanvasGroup cg = transform.GetChild(i).GetComponent<CanvasGroup>();
            cg.alpha = disabledAlpha;
            cg.interactable = cg.blocksRaycasts = false;
        }

        if(selectedTransformationsCount == 1)
            pointerCG.alpha = 1f;
        pointerRT.anchoredPosition = new Vector2(newTr.anchoredPosition.x, pointerY);

        selectedTransformations[selectedTransformationsCount-1] = index;
        slider.Set(transformationsSO.datas[index]);
        //mainShape.SelectTransformation(index);
    }

    public void AddBackLast(){
        selectedTransformationsCount--;
        if(selectedTransformationsCount > 0)
            transform.GetChild(selectedTransformationsCount + 1).GetComponent<CanvasGroup>().SetCG(true);

        if(selectedTransformationsCount == 0){
            pointerCG.alpha = 0f;
            slider.Hide();
        }
        else {
            pointerRT.anchoredPosition = new Vector2(transform.GetChild(selectedTransformationsCount + 1).GetComponent<RectTransform>().anchoredPosition.x, pointerY);
            slider.Set(transformationsSO.datas[selectedTransformations[selectedTransformationsCount-1]]);
        }
        
        //mainShape.UndoTransformation();
        OnAddBack?.Invoke(selectedTransformations[selectedTransformationsCount]);
    }

    public void OnLastStaredDrag(){
        pointerCG.alpha = 0f;
    }

    public void OnLastEndedDrag(){
        pointerCG.alpha = 1f;
    }

    public Vector2 GetAddBackWorldPos(){
        return availableTransformations.GetAddBackWorldPos(selectedTransformations[selectedTransformationsCount]);
    }

    private void HandleOnLevelChanged(int _){
        for(int i=2; i<transform.childCount; i++){
            Destroy(transform.GetChild(i).gameObject);
        }
        pointerCG.SetCG(false);
        selectedTransformations = new int[20];
        selectedTransformationsCount = 0;
    }
}
