using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomSlider : MonoBehaviour {
    [SerializeField] private EventTrigger movementEventTrigger;
    [SerializeField] private RectTransform sliderRT, clampRT, knobRT, leftRT, rightRT;
    [SerializeField] private float padding;
    [SerializeField] private float minValue, maxValue;
    [SerializeField] private int decimals;
    [SerializeField] private string format;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private float hideY, shownY, hideAnimationDuration;
    [SerializeField] private LeanTweenType hideEase, showEase;
    [SerializeField] private LevelManager levelManager;
    //[SerializeField] private MainShapeBehaviour shapeBehaviour;
    //[SerializeField] private TargetShapeBehaviour targetHandler;
    private float knobY, knobClamp;
    private float maxWidth, sizeY, p;
    private float initX;
    private bool on = false;

    private void OnEnable(){
        levelManager.OnLevelChanged += HandleOnLevelChanged;
    }
    private void OnDisable(){
        if(levelManager != null)
            levelManager.OnLevelChanged -= HandleOnLevelChanged;
    }

    private void Awake(){
        p = Mathf.Pow(10f, decimals);
        SetupEvents();
    }

    private void Start() {
        initX = sliderRT.anchoredPosition.x;
        knobY = knobRT.anchoredPosition.y;
        knobClamp = clampRT.rect.width / 2;
        maxWidth = sliderRT.rect.width;
        sizeY = leftRT.sizeDelta.y;

        knobRT.anchoredPosition = new Vector2(0, knobY);

        leftRT.sizeDelta = new Vector2(maxWidth / 2 - padding / 2, sizeY);
        rightRT.sizeDelta = new Vector2(maxWidth / 2 - padding / 2, sizeY);
        
        float value = Mathf.Lerp(minValue, maxValue, 0.5f);
        valueText.text = string.Format(format, Mathf.Round(value * p) / p);
    }

    public void Set(TransformationData data){
        minValue = data.minRange;
        maxValue = data.maxRange;
        format = data.format;
        decimals = data.decimals;
        p = Mathf.Pow(10f, decimals);
        float value = Mathf.Lerp(minValue, maxValue, 0.5f);
        valueText.text = string.Format(format, Mathf.Round(value * p) / p);
        HardSet(Mathf.InverseLerp(data.minRange, data.maxRange, data.initialValue));

        if(data.needsSlider != on) {
            on = data.needsSlider;
            LeanTween.value(sliderRT.gameObject, MoveToY, on ? hideY : shownY, on ? shownY : hideY, hideAnimationDuration);
        }
    }

    public void Hide(){
        if(on == false) return;
        on = false;
        LeanTween.value(sliderRT.gameObject, MoveToY, shownY, hideY, hideAnimationDuration);
    }

    private void MoveToY(float y){
        sliderRT.anchoredPosition = new Vector2(initX, y);
    }

    private void HardSet(float normalized){
        float clampedX = Mathf.Lerp(-knobClamp, knobClamp, normalized);
        knobRT.anchoredPosition = new Vector2(clampedX, knobY);

        float t = Mathf.InverseLerp(-maxWidth / 2, maxWidth / 2, clampedX);
        leftRT.sizeDelta = new Vector2(maxWidth * t - padding / 2, sizeY);
        rightRT.sizeDelta = new Vector2(maxWidth * (1 - t) - padding / 2, sizeY);

        float vt = Mathf.InverseLerp(-knobClamp, knobClamp, clampedX);
        float value = Mathf.Lerp(minValue, maxValue, vt);
        valueText.text = string.Format(format, Mathf.Round(value * p) / p);
    }

    public void HardSetValue(float value){
        float t = Mathf.InverseLerp(minValue, maxValue, value);
        HardSet(t);
    }

    private void HandleDrag(PointerEventData eventData){
        RectTransformUtility.ScreenPointToLocalPointInRectangle(clampRT, eventData.position, null, out Vector2 localPoint);
        float clampedX = Mathf.Clamp(localPoint.x, -knobClamp, knobClamp);
        knobRT.anchoredPosition = new Vector2(clampedX, knobY);

        float t = Mathf.InverseLerp(-maxWidth / 2, maxWidth / 2, clampedX);
        leftRT.sizeDelta = new Vector2(maxWidth * t - padding / 2, sizeY);
        rightRT.sizeDelta = new Vector2(maxWidth * (1 - t) - padding / 2, sizeY);

        float vt = Mathf.InverseLerp(-knobClamp, knobClamp, clampedX);
        float value = Mathf.Lerp(minValue, maxValue, vt);
        valueText.text = string.Format(format, Mathf.Round(value * p) / p);

        //shapeBehaviour.TweakTransformation(value);
    }

    private void OnEndDrag(){
        //targetHandler.CheckCompletion();
    }

    private void HandleOnLevelChanged(int _) => Hide();

    private void SetupEvents(){
        EventTrigger.Entry onDrag = new EventTrigger.Entry();
        onDrag.eventID = EventTriggerType.Drag;
        onDrag.callback.AddListener(ev => HandleDrag(ev as PointerEventData));
        movementEventTrigger.triggers.Add(onDrag);

        EventTrigger.Entry onEndDrag = new EventTrigger.Entry();
        onEndDrag.eventID = EventTriggerType.EndDrag;
        onEndDrag.callback.AddListener(ev => OnEndDrag());
        movementEventTrigger.triggers.Add(onEndDrag);

        EventTrigger.Entry onClick = new EventTrigger.Entry();
        onClick.eventID = EventTriggerType.PointerClick;
        onClick.callback.AddListener(ev => HandleDrag(ev as PointerEventData));
        movementEventTrigger.triggers.Add(onClick);
    }
}
