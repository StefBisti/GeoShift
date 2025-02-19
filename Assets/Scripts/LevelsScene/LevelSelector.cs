using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(EventTrigger))]
public class LevelSelector : MonoBehaviour {
    [SerializeField] private int level, mainSceneIndex;
    [SerializeField] private Sprite unpressedSprite, pressedSprite;
    [SerializeField] private RectTransform canvasContent;
    [SerializeField] private Vector2 contentPosWhenPressed;
    [SerializeField] private Color disabledColor, enabledColor;
    [SerializeField] private float disabledAlpha;
    [SerializeField] private CanvasGroup cg;
    private Vector2 contentInitPos;
    private EventTrigger eventTrigger;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        eventTrigger = GetComponent<EventTrigger>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        contentInitPos = canvasContent.anchoredPosition;
        SetListeners();
    }

    private void Start(){
        if(level > LevelManager.Instance.MaxLevel){
            cg.alpha = disabledAlpha;
            spriteRenderer.color = disabledColor;
        }
        else{
            cg.alpha = 1f;
            spriteRenderer.color = enabledColor;
        }
    }

    private void HandleOnPointerDown(){
        if(level > LevelManager.Instance.MaxLevel) return;
        spriteRenderer.sprite = pressedSprite;
        canvasContent.anchoredPosition = contentPosWhenPressed;
    }

    private void HandleOnPointerUp(){
        if(level > LevelManager.Instance.MaxLevel) return;
        spriteRenderer.sprite = unpressedSprite;
        canvasContent.anchoredPosition = contentInitPos;
    }

    private void HandleOnPointerClick(){
        if(level > LevelManager.Instance.MaxLevel) return;
        LevelManager.Instance.GoToLevel(level);
    }

    private void SetListeners(){
        EventTrigger.Entry onPointerDown = new EventTrigger.Entry();
        onPointerDown.eventID = EventTriggerType.PointerDown;
        onPointerDown.callback.AddListener(_ => HandleOnPointerDown());
        eventTrigger.triggers.Add(onPointerDown);

        EventTrigger.Entry onPointerUp = new EventTrigger.Entry();
        onPointerUp.eventID = EventTriggerType.PointerUp;
        onPointerUp.callback.AddListener(_ => HandleOnPointerUp());
        eventTrigger.triggers.Add(onPointerUp);

        EventTrigger.Entry onClick = new EventTrigger.Entry();
        onClick.eventID = EventTriggerType.PointerClick;
        onClick.callback.AddListener(_ => HandleOnPointerClick());
        eventTrigger.triggers.Add(onClick);
    }
}
