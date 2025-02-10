using UnityEngine;

public class MeasuringTool : MonoBehaviour {

    [SerializeField] private float snapThreshold;
    [SerializeField] private int frontLayer, behindLayer;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Vector2 offset;
    private bool isSelected = false, isRotating = false;
    private bool isActive = false;

    private void Update() {
        if(isActive == false) return;

        if (Input.touchCount > 0) {
            Touch firstTouch = Input.GetTouch(0);
            if (firstTouch.phase == TouchPhase.Began){
                RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(firstTouch.position));
                if (hit.transform != null) {
                    if (hit.transform == transform){
                        isSelected = true;
                        spriteRenderer.sortingOrder = frontLayer;
                        Vector2 touchWorldPos = GetWorldPosition(firstTouch.position);
                        offset = (Vector2)transform.position - touchWorldPos;
                    }
                    else {
                        isSelected = false;
                        spriteRenderer.sortingOrder = behindLayer;
                    }
                }
            }

            if (isSelected){
                if (Input.touchCount == 2){
                    isRotating = true;
                    Touch touch0 = Input.GetTouch(0);
                    Touch touch1 = Input.GetTouch(1);
                    HandleRotation(touch0, touch1);
                }
                else if (Input.touchCount == 1){
                    if (isRotating){
                        SnapRotation();
                        isRotating = false;
                    }
                    HandleDrag(firstTouch);
                }
            }
        }
        else {
            if (isRotating){
                SnapRotation();
                isRotating = false;
            }
            isSelected = false;
            spriteRenderer.sortingOrder = behindLayer;
        }
    }

    private void HandleDrag(Touch touch){
        if (touch.phase == TouchPhase.Moved){
            Vector2 touchWorldPos = GetWorldPosition(touch.position);
            transform.position = (Vector3)(touchWorldPos + offset);
        }
    }

    private void HandleRotation(Touch touch0, Touch touch1){
        Vector2 prevPos0 = touch0.position - touch0.deltaPosition;
        Vector2 prevPos1 = touch1.position - touch1.deltaPosition;

        float prevAngle = Mathf.Atan2(prevPos1.y - prevPos0.y, prevPos1.x - prevPos0.x) * Mathf.Rad2Deg;
        float currentAngle = Mathf.Atan2(touch1.position.y - touch0.position.y, touch1.position.x - touch0.position.x) * Mathf.Rad2Deg;
        float deltaAngle = currentAngle - prevAngle;

        transform.Rotate(0f, 0f, deltaAngle);
    }

    private void SnapRotation(){
        float currentZ = transform.eulerAngles.z;

        float closest = Mathf.Round(currentZ / 90f) * 90f;
        
        float diff = Mathf.Abs(Mathf.DeltaAngle(currentZ, closest));
        print((closest, diff));

        if (diff <= snapThreshold){
            Vector3 newEuler = transform.eulerAngles;
            newEuler.z = closest;
            transform.eulerAngles = newEuler;
        }
    }

    private Vector2 GetWorldPosition(Vector2 screenPosition){
        return (Vector2)Camera.main.ScreenToWorldPoint(screenPosition);
    }


    public void Toggle(){
        isActive = !isActive;
        spriteRenderer.enabled = isActive;
    }
}
