using UnityEngine;

public class Tutorial : MonoBehaviour {

    [SerializeField] private CanvasGroup[] tutorials;
    [SerializeField] private CanvasGroup tutorialButton;
    [SerializeField] private int lastLevelIndexWithTutorial;
    
    private void Awake(){
        LevelManager.Instance.OnLevelChanged += SetTutorial;
    }
    private void OnDestroy(){
        if(LevelManager.Instance != null){
            LevelManager.Instance.OnLevelChanged -= SetTutorial;
        }
    }

    private void Start(){
        SetTutorial(LevelManager.Instance.Level);
    }

    private void SetTutorial(int level){
        RemoveTutorials();
        if(level < tutorials.Length)
            tutorials[level].alpha = 1f;

        if(level > lastLevelIndexWithTutorial)
            tutorialButton.SetCG(false);
    }

    public void RemoveTutorials(){
        foreach(CanvasGroup cg in tutorials) cg.alpha = 0f;
    }
}
