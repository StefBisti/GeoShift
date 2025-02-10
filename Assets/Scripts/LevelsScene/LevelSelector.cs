using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(EventTrigger))]
public class LevelSelector : MonoBehaviour {
    [SerializeField] private int level, mainSceneIndex;
    [SerializeField] private Sprite unpressedSprite, pressedSprite;
    [SerializeField] private RectTransform canvasContent;
    [SerializeField] private Vector2 contentPosWhenPressed;
    private Vector2 contentInitPos;
    private EventTrigger eventTrigger;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        eventTrigger = GetComponent<EventTrigger>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        contentInitPos = canvasContent.anchoredPosition;
        SetListeners();
    }

    private void HandleOnPointerDown(){
        spriteRenderer.sprite = pressedSprite;
        canvasContent.anchoredPosition = contentPosWhenPressed;
    }

    private void HandleOnPointerUp(){
        spriteRenderer.sprite = unpressedSprite;
        canvasContent.anchoredPosition = contentInitPos;
    }

    private void HandleOnPointerClick(){
        SceneManager.LoadSceneAsync(mainSceneIndex);
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
