using UnityEngine;
using UnityEngine.EventSystems;


// TO DO

[RequireComponent(typeof(EventTrigger))]
public class AvailableTransformationSelf : MonoBehaviour {
    [SerializeField] private AvailableTransformations parent;

    private void Awake(){
        SetupListeners();
    }

    private void HandleOnStartDrag(){

    }

    private void HandleOnEndDrag(){

    }

    private void HandleOnClick(){

    }

    private void SetupListeners(){
        EventTrigger eventTrigger = GetComponent<EventTrigger>();

        EventTrigger.Entry onStartDrag = new EventTrigger.Entry();
        onStartDrag.eventID = EventTriggerType.BeginDrag;
        onStartDrag.callback.AddListener(_ => HandleOnStartDrag());
        eventTrigger.triggers.Add(onStartDrag);

        EventTrigger.Entry onEndDrag = new EventTrigger.Entry();
        onEndDrag.eventID = EventTriggerType.Drag;
        onEndDrag.callback.AddListener(_ => HandleOnEndDrag());
        eventTrigger.triggers.Add(onEndDrag);

        EventTrigger.Entry onClick = new EventTrigger.Entry();
        onClick.eventID = EventTriggerType.PointerClick;
        onClick.callback.AddListener(_ => HandleOnClick());
        eventTrigger.triggers.Add(onClick);
    }
}
