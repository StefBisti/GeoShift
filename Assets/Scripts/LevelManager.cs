using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public event Action<int> OnLevelChanged;
    [SerializeField] private LevelsSO levelsData;

    
    private void Awake(){
        Application.targetFrameRate = Application.isEditor ? -1 : 60;
        LeanTween.reset();
    }

    public void GoToMenu() => SceneManager.LoadSceneAsync(1);
    public LevelData GetLevelData(int index) => levelsData.levels[index];






    public bool triggerOnSpace;
    public int lvltemp = 1;
    void Update(){
        if(triggerOnSpace && Input.GetKeyDown(KeyCode.Space)){
            TriggerOnLevelChanged();
        }
    }


    [ContextMenu("Trigger OnLevelChanged (Temp)")]
    private void TriggerOnLevelChanged() => OnLevelChanged?.Invoke(lvltemp++);


}
