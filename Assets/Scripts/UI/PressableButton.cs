using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger), typeof(Image))]
public class PressableButton : MonoBehaviour {
    [SerializeField] private Sprite buttonUnpressedSprite, buttonPressedSprite;
    [SerializeField] private RectTransform contentRT;
    [SerializeField] private Vector2 contentAnchoredPosWhenPressed;
    private Vector2 contentInitAnchoredPos;
    private EventTrigger eventTrigger;
    private Image image;

    private void Awake() {
        eventTrigger = GetComponent<EventTrigger>();
        image = GetComponent<Image>();
        contentInitAnchoredPos = contentRT.anchoredPosition;
        SetListeners();
    }

    private void HandleOnPointerDown(){
        image.sprite = buttonPressedSprite;
        contentRT.anchoredPosition = contentAnchoredPosWhenPressed;
    }

    private void HandleOnPointerUp(){
        image.sprite = buttonUnpressedSprite;
        contentRT.anchoredPosition = contentInitAnchoredPos;
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
    }
}

