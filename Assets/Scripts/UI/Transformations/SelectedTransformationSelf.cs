using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class SelectedTransformationSelf : MonoBehaviour {
    [SerializeField] private float animationTime;
    [SerializeField] private LeanTweenType ease;
    [SerializeField] private Vector2 blReturnRectNormalized, trReturnRectNormalized;
    [SerializeField] private SelectedTransformations parent;
    private RectTransform rt;
    private CanvasGroup cg;
    private Vector2 startPos;

    private void Awake(){
        SetupTriggers();
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
    }

    private void HandleOnStartDrag(PointerEventData data){
        startPos = rt.anchoredPosition;
        rt.position = data.position;
        parent.OnLastStaredDrag();
    }

    private void HandleOnDrag(PointerEventData data){
        rt.position = data.position;
    }

    private void HandleOnEndDrag(PointerEventData data){
        parent.OnLastEndedDrag();
        if(IsInsideReturnRect(rt.position)){
            parent.AddBackLast();
            Vector2 addBackWorldPos = parent.GetAddBackWorldPos();
            Vector2 addBackAnchoredPos = rt.parent.InverseTransformPoint(addBackWorldPos);
            rt.LeanMoveLocal(addBackAnchoredPos, animationTime).setEase(ease).setOnComplete(() => Destroy(gameObject));
        }
        else {
            rt.LeanMoveLocal(startPos, animationTime).setEase(ease);
        }  
    }

    private bool IsInsideReturnRect(Vector2 worldPos){
        float w = Screen.width;
        float h = Screen.height;
        float minx = blReturnRectNormalized.x;
        float maxx = trReturnRectNormalized.x;
        float miny = blReturnRectNormalized.y;
        float maxy = trReturnRectNormalized.y;
        return worldPos.x >= minx * w && worldPos.x <= maxx * w && worldPos.y >= miny * h && worldPos.y <= maxy * h;
    }

    private void SetupTriggers(){
        EventTrigger trigger = GetComponent<EventTrigger>();

        EventTrigger.Entry onStartDrag = new EventTrigger.Entry();
        onStartDrag.eventID = EventTriggerType.BeginDrag;
        onStartDrag.callback.AddListener(ev => HandleOnStartDrag((PointerEventData)ev));
        trigger.triggers.Add(onStartDrag);

        EventTrigger.Entry onDrag = new EventTrigger.Entry();
        onDrag.eventID = EventTriggerType.Drag;
        onDrag.callback.AddListener(ev => HandleOnDrag((PointerEventData)ev));
        trigger.triggers.Add(onDrag);

        EventTrigger.Entry onEndDrag = new EventTrigger.Entry();
        onEndDrag.eventID = EventTriggerType.EndDrag;
        onEndDrag.callback.AddListener(ev => HandleOnEndDrag((PointerEventData)ev));
        trigger.triggers.Add(onEndDrag);
    }
}
