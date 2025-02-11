using UnityEngine;

public class Axis : MonoBehaviour {
    [SerializeField] private float moveDuration;
    [SerializeField] private LeanTweenType moveEase;

    public void MoveTo(Vector2 worldPos){
        transform.LeanMove(worldPos, moveDuration).setEase(moveEase);
    }
}
