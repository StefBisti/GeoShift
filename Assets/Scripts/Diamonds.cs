using System;
using UnityEngine;

public class Diamonds : Singleton<Diamonds> {
    public event Action<int> OnDiamondCountChanged;
    public event Action<Vector2> OnDiamondGot, OnGiftGot;
    public event Action OnGiftEnded;
    [SerializeField] private int diamondsCount;
    [SerializeField] private bool loadCollectedDiamonds;
    private string collectedDiamondsString = "";

    public int DiamondsCount { get => diamondsCount; }

    protected override void SingletonInit(){
        diamondsCount = PlayerPrefs.GetInt("Diamonds", 0);
        if(loadCollectedDiamonds)
            collectedDiamondsString = PlayerPrefs.GetString("CollectedDiamonds", "");
    }

    public bool TrySpendDiamonds(int count){
        if(diamondsCount < count) return false;
        diamondsCount -= count;
        PlayerPrefs.SetInt("Diamonds", diamondsCount);
        PlayerPrefs.Save();
        OnDiamondCountChanged?.Invoke(diamondsCount);
        return true;
    }

    public void AddDiamond(Vector2 pos){
        diamondsCount++;
        PlayerPrefs.SetInt("Diamonds", diamondsCount);
        PlayerPrefs.Save();
        OnDiamondGot?.Invoke(pos);
    }

    public void CollectDiamond(int levelIndex, int diamondIndex){
        while (collectedDiamondsString.Split('|').Length <= levelIndex)
            collectedDiamondsString += "|";
        
        string[] levels = collectedDiamondsString.Split('|');
        string[] diamonds = levels[levelIndex].Split(',');

        if (!Array.Exists(diamonds, d => d == diamondIndex.ToString())){
            levels[levelIndex] = levels[levelIndex] == "" ? diamondIndex.ToString() : levels[levelIndex] + "," + diamondIndex.ToString();
            collectedDiamondsString = string.Join("|", levels);
            PlayerPrefs.SetString("CollectedDiamonds", collectedDiamondsString);
            PlayerPrefs.Save();
        }
    }
    public void CollectDiamond(int diamondIndex) => CollectDiamond(LevelManager.Instance.Level, diamondIndex);

    public bool IsDiamondCollected(int levelIndex, int diamondIndex) {
        if (collectedDiamondsString.Split('|').Length <= levelIndex)
            return false;

        string[] diamonds = collectedDiamondsString.Split('|')[levelIndex].Split(',');
        return Array.Exists(diamonds, d => d == diamondIndex.ToString());
    }
    public bool IsDiamondCollected(int diamondIndex) => IsDiamondCollected(LevelManager.Instance.Level, diamondIndex);

    public void GetGift(Vector2 pos){
        OnGiftGot?.Invoke(pos);
    }
    public void EndGift() => OnGiftEnded?.Invoke();

    [ContextMenu("Reset Collected Diamonds Save")]
    private void ResetCollectedDiamondsSave(){
        PlayerPrefs.SetString("CollectedDiamonds", "");
        PlayerPrefs.Save();
    }
}
