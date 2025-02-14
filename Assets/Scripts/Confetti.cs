using UnityEngine;

public class ConfettiHandler : MonoBehaviour {
    [SerializeField] private Camera confettiCam;
    [SerializeField] private ParticleSystem confetti1, confetti2;
    [SerializeField] private CanvasGroup confettiTexture;
    [SerializeField] private float onTime;
    private float timer = 0f;
    private bool active = false;

    private void Awake(){
        LevelManager.Instance.OnLevelChanged += HandleOnLevelChanged;
    }

    private void OnDestroy(){
        if(LevelManager.Instance != null)
            LevelManager.Instance.OnLevelChanged -= HandleOnLevelChanged;
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
        Audio.Instance.PlaySfx(AudioFx.PartyHorn);
        Audio.Instance.PlaySfx(AudioFx.RustlingPaper);
    }
}