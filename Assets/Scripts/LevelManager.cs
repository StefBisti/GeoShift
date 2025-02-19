using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager> {
    public event Action<int> OnLevelChanged;
    public event Action<float> OnRequestedExit, OnEntered;
    public event Action OnEnteringLevelFailed;
    [SerializeField] private LevelsSO levelsData;
    [SerializeField] private float exitAndEnterAnimationDuration;
    [SerializeField] private bool loadLevel;
    private int level = 0, maxLevel = 0;
    private bool isMain = true;

    public int Level { get => level; }
    public int MaxLevel { get => maxLevel; }
    public bool IsMain { get => isMain; }

    
    protected override void SingletonInit(){
        Application.targetFrameRate = Application.isEditor ? -1 : 60;
        LeanTween.reset();

        if(loadLevel)
            level = maxLevel = PlayerPrefs.GetInt("Level", 0);
    }

    private void Start(){
        OnEntered?.Invoke(exitAndEnterAnimationDuration);
        SceneManager.sceneLoaded += HandleSceneLoad;
    }

    private void OnDestroy(){
        SceneManager.sceneLoaded -= HandleSceneLoad;
    }

    public void TriggerCompleteLevel(){
        level++;
        PlayerPrefs.SetInt("Level", level);
        maxLevel = Mathf.Max(maxLevel, level);
        OnLevelChanged?.Invoke(level);
    }

    public LevelData GetLevelData(int index) => levelsData.levels[index];
    public LevelData GetCurrentLevelData() => GetLevelData(level);

    public void GoToMenu() {
        Audio.Instance.PlaySfx(AudioFx.ButtonPress);
        OnRequestedExit?.Invoke(exitAndEnterAnimationDuration);
        this.DoAfterSeconds(exitAndEnterAnimationDuration, () => SceneManager.LoadSceneAsync(1));
        isMain = false;
        Audio.Instance.PlaySfx(AudioFx.Swoosh);
    }
    public void GoToLevel(int level){
        Audio.Instance.PlaySfx(AudioFx.ButtonPress);
        if(Hearts.Instance.HeartsCount == 0){
            OnEnteringLevelFailed?.Invoke();
            return;
        }
        this.level = level;
        OnRequestedExit?.Invoke(exitAndEnterAnimationDuration);
        this.DoAfterSeconds(exitAndEnterAnimationDuration, () => SceneManager.LoadSceneAsync(0));
        isMain = true;
        Audio.Instance.PlaySfx(AudioFx.Swoosh);
    }

    private void HandleSceneLoad(Scene scene, LoadSceneMode mode) => OnEntered?.Invoke(exitAndEnterAnimationDuration);
}
