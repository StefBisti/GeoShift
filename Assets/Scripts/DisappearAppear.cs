using UnityEngine;

public class DisappearAppear : MonoBehaviour {
    [SerializeField] private LeanTweenType ease;

    private void Awake() {
        LevelManager.Instance.OnRequestedExit += HandleExit;
        LevelManager.Instance.OnEntered += HandleEnter;
    }

    private void OnDestroy(){
        if(LevelManager.Instance != null){
            LevelManager.Instance.OnRequestedExit -= HandleExit;
            LevelManager.Instance.OnEntered -= HandleEnter;
        }
    }

    private void HandleExit(float animTime){
        transform.LeanScale(Vector3.zero, animTime).setEase(ease);
    }

    private void HandleEnter(float animTime){
        Vector3 startScale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.LeanScale(startScale, animTime).setEase(ease);
    }
}
