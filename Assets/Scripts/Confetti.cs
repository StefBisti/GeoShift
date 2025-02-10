using UnityEngine;

public class ConfettiHandler : MonoBehaviour {
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Camera confettiCam;
    [SerializeField] private ParticleSystem confetti1, confetti2;
    [SerializeField] private CanvasGroup confettiTexture;
    [SerializeField] private float onTime;
    private float timer = 0f;
    private bool active = false;

    private void OnEnable(){
        levelManager.OnLevelChanged += HandleOnLevelChanged;
    }

    private void OnDisable(){
        if(levelManager != null)
            levelManager.OnLevelChanged -= HandleOnLevelChanged;
    }

    private void Update(){
        if(active){
            timer += Time.deltaTime;
            if(timer > onTime){
                timer = 0f;
                active = false;
                confettiCam.enabled = false;
                confettiTexture.alpha = 0f;
            }
        }
    }

    private void HandleOnLevelChanged(int _) => Burst();

    private void Burst(){
        active = true;
        confettiCam.enabled = true;
        confettiTexture.alpha = 1f;
        confetti1.Play();
        confetti2.Play();
    }
}