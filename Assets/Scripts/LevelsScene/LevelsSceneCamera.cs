using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelsSceneCamera : MonoBehaviour {
    [SerializeField] private float moveSpeed = 5f, minY = 0f, maxY = 10f;
    private Vector3 touchStart;
    private bool started = false;
    private int uilayer;

    private void Awake(){
        uilayer = LayerMask.NameToLayer("UI");
    }

    private void Update(){
        HandleCameraMovement();
    }

    private void HandleCameraMovement(){
        if (Input.GetMouseButtonDown(0) && IsPointerOverUIObject() == false){
            touchStart = GetWorldPosition(Input.mousePosition);
            started = true;
        }

        if (started && Input.GetMouseButton(0) && IsPointerOverUIObject() == false){
            Vector3 direction = touchStart - GetWorldPosition(Input.mousePosition);
            Vector3 newPosition = transform.position + new Vector3(0, direction.y, 0);

            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            transform.position = Vector3.Lerp(transform.position, newPosition, moveSpeed * Time.deltaTime);
        }

        if(Input.GetMouseButtonUp(0)){
            started = false;
        }
    }

    private Vector3 GetWorldPosition(Vector3 screenPosition){
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0));
        xy.Raycast(ray, out float distance);
        return ray.GetPoint(distance);
    }

    private bool IsPointerOverUIObject() {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach(RaycastResult res in results){
            if(res.gameObject.layer == uilayer) return true;
        }
        return false;
    }
}
