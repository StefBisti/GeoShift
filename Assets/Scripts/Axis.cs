using UnityEngine;

public class Axis : MonoBehaviour {
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private float moveDuration;
    [SerializeField] private LeanTweenType moveEase;

    private void OnEnable(){
        levelManager.OnLevelChanged += HandleOnLevelChanged;
    }
    private void OnDisable(){
        if(levelManager != null){
            levelManager.OnLevelChanged -= HandleOnLevelChanged;
        }
    }

    private void HandleOnLevelChanged(int _) => MoveTo(Vector2.zero);

    private void MoveTo(Vector2 worldPos){
        transform.LeanMove(worldPos, moveDuration).setEase(moveEase);
    }
}
