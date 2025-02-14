using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomSlider : MonoBehaviour {
    [SerializeField] private EventTrigger movementEventTrigger;
    [SerializeField] private RectTransform sliderRT, clampRT, knobRT, leftRT, rightRT;
    [SerializeField] private float padding;
    [SerializeField] private float minValue, maxValue, snapIntervals, snapThreshold;
    [SerializeField] private int decimals;
    [SerializeField] private string format;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private float hideY, shownY, hideAnimationDuration;
    [SerializeField] private LeanTweenType hideEase, showEase;
    [SerializeField] private CanvasGroup sliderCanvasGroup;
    [SerializeField] private ShapeTransformations shapeTransformations;
    [SerializeField] private TargetShape targetShape;
    [SerializeField] private MainShape mainShape;
    private float knobY, knobClamp;
    private float maxWidth, sizeY, p;
    private float initX;
    private bool on = false;

    private void Awake(){
        LevelManager.Instance.OnLevelChanged += HandleOnLevelChanged;
        LevelManager.Instance.OnRequestedExit += HandleOnExit;
        Diamonds.Instance.OnGiftGot += LoseFocus;
        Diamonds.Instance.OnGiftEnded += RegainFocus;
        mainShape.OnDeath += Hide;
        p = Mathf.Pow(10f, decimals);
        SetupEvents();
    }
    private void OnDestroy(){
        if(LevelManager.Instance != null) {
            LevelManager.Instance.OnLevelChanged -= HandleOnLevelChanged;
            LevelManager.Instance.OnRequestedExit -= HandleOnExit;
        }
        if(Diamonds.Instance != null){
            Diamonds.Instance.OnGiftGot -= LoseFocus;
            Diamonds.Instance.OnGiftEnded -= RegainFocus;
        }

        if(mainShape != null){
            mainShape.OnDeath -= Hide;
        }
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
        float rounded = Mathf.Round(value / snapIntervals) * snapIntervals;
        if(Mathf.Abs(value - rounded) < snapThreshold) value = rounded;

        valueText.text = string.Format(format, Mathf.Round(value * p) / p);
    }

    public void Set(TransformationData data){
        minValue = data.minRange;
        maxValue = data.maxRange;
        snapIntervals = data.snapIntervals;
        snapThreshold = data.snapThreshold;
        format = data.format;
        decimals = data.decimals;
        p = Mathf.Pow(10f, decimals);
        HardSet(Mathf.InverseLerp(data.minRange, data.maxRange, data.initialValue));

        if(data.needsSlider != on) {
            on = data.needsSlider;
            sliderCanvasGroup.blocksRaycasts = sliderCanvasGroup.interactable = on;
            LeanTween.value(sliderRT.gameObject, MoveToY, on ? hideY : shownY, on ? shownY : hideY, hideAnimationDuration);
        }
    }

    public void Hide(){
        if(on == false) return;
        on = false;
        sliderCanvasGroup.blocksRaycasts = sliderCanvasGroup.interactable = on;

        LeanTween.value(sliderRT.gameObject, MoveToY, shownY, hideY, hideAnimationDuration);
    }

    private void LoseFocus(Vector2 _){
        on = false;
    }
    private void RegainFocus(){
        on = true;
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
        float rounded = Mathf.Round(value / snapIntervals) * snapIntervals;
        if(Mathf.Abs(value - rounded) < snapThreshold) value = rounded;
        valueText.text = string.Format(format, Mathf.Round(value * p) / p);
    }

    public void HardSetValue(float value){
        float t = Mathf.InverseLerp(minValue, maxValue, value);
        HardSet(t);
    }

    private void HandleDrag(PointerEventData eventData){
        if(on == false) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(clampRT, eventData.position, null, out Vector2 localPoint);
        float clampedX = Mathf.Clamp(localPoint.x, -knobClamp, knobClamp);
        knobRT.anchoredPosition = new Vector2(clampedX, knobY);

        float t = Mathf.InverseLerp(-maxWidth / 2, maxWidth / 2, clampedX);
        leftRT.sizeDelta = new Vector2(maxWidth * t - padding / 2, sizeY);
        rightRT.sizeDelta = new Vector2(maxWidth * (1 - t) - padding / 2, sizeY);

        float vt = Mathf.InverseLerp(-knobClamp, knobClamp, clampedX);
        float value = Mathf.Lerp(minValue, maxValue, vt);
        float rounded = Mathf.Round(value / snapIntervals) * snapIntervals;
        if(Mathf.Abs(value - rounded) < snapThreshold) value = rounded;
        valueText.text = string.Format(format, Mathf.Round(value * p) / p);

        shapeTransformations.TweakTransformation(value);
    }

    private void OnEndDrag(){
        if(on == false) return;
        targetShape.CheckCompletion();
    }

    private void HandleOnLevelChanged(int _) => Hide();
    private void HandleOnExit(float _) => Hide();

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
