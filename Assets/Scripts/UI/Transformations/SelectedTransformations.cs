using UnityEngine;
using UnityEngine.UI;
using System;

public class SelectedTransformations : MonoBehaviour {
    public event Action<int> OnAddBack;
    [SerializeField] private MainShape mainShape;
    [SerializeField] private ShapeTransformations shapeTransformations;
    [SerializeField] private AvailableTransformations availableTransformations;
    [SerializeField] private Vector2 spaceBetween, pointerOffset;
    [SerializeField] private int wrapCount;
    [SerializeField] private Sprite[] transformationSprites;
    [SerializeField] private float disabledAlpha;
    [SerializeField] private RectTransform pointerRT;
    [SerializeField] private CanvasGroup pointerCG, selectedTransformationsCG;
    [SerializeField] private CustomSlider slider;
    [SerializeField] private TransformationsSO transformationsSO;
    private int[] selectedTransformations = new int[20];
    private int selectedTransformationsCount = 0;
    private RectTransform prefab;

    private void Awake(){
        availableTransformations.OnSelected += HandleOnSelected;
        LevelManager.Instance.OnLevelChanged += HandleOnLevelChanged;
        mainShape.OnReset += Reset;
        mainShape.OnDeath += HandleOnShapeDeath;
    }

    private void OnDestroy(){
        if(availableTransformations != null){
            availableTransformations.OnSelected -= HandleOnSelected;
        }
        if(LevelManager.Instance != null){
            LevelManager.Instance.OnLevelChanged -= HandleOnLevelChanged;
        }
        if(mainShape != null){
            mainShape.OnReset -= Reset;
            mainShape.OnDeath -= HandleOnShapeDeath;
        }
    }

    private void Start(){
        pointerCG.SetCG(false);

        prefab = transform.GetChild(1).GetComponent<RectTransform>();
        prefab.GetComponent<CanvasGroup>().SetCG(false);
    }

    private void HandleOnSelected(int index){
        RectTransform newTr = Instantiate(prefab, transform);
        newTr.name = "Transformation";
        int s = selectedTransformationsCount;
        Vector2 pos = new Vector2(spaceBetween.x * (s % wrapCount), spaceBetween.y * (s / wrapCount));
        newTr.anchoredPosition = pos;
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
        pointerRT.anchoredPosition = pos + pointerOffset;

        selectedTransformations[selectedTransformationsCount-1] = index;
        slider.Set(transformationsSO.datas[index]);
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
            pointerRT.anchoredPosition = transform.GetChild(selectedTransformationsCount + 1).GetComponent<RectTransform>().anchoredPosition + pointerOffset;
            slider.Set(transformationsSO.datas[selectedTransformations[selectedTransformationsCount-1]]);
        }
        
        shapeTransformations.UndoToPreviousTransformation();
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


    private void Reset(){
        for(int i=2; i<transform.childCount; i++){
            Destroy(transform.GetChild(i).gameObject);
        }
        pointerCG.SetCG(false);
        selectedTransformations = new int[20];
        selectedTransformationsCount = 0;
        selectedTransformationsCG.blocksRaycasts = selectedTransformationsCG.interactable = true;
    }

    private void HandleOnLevelChanged(int _) => Reset();

    private void HandleOnShapeDeath(){
        pointerCG.SetCG(false);
        selectedTransformationsCG.blocksRaycasts = selectedTransformationsCG.interactable = false;
    }
}
