using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public event Action<int> OnLevelChanged;
    [SerializeField] private LevelsSO levelsData;
    private int level = 0;

    public int Level { get => level; }

    
    private void Awake(){
        Application.targetFrameRate = Application.isEditor ? -1 : 60;
        LeanTween.reset();

        // retrieve level from save
    }

    public void TriggerCompleteLevel(){
        level++;
        OnLevelChanged?.Invoke(level);
    }

    public LevelData GetLevelData(int index) => levelsData.levels[index];
    public LevelData GetCurrentLevelData() => GetLevelData(level);

    public void GoToMenu() => SceneManager.LoadSceneAsync(1);
}
