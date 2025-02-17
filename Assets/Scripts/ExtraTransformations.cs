using System;
using UnityEngine;

public class ExtraTransformations : Singleton<ExtraTransformations> {
    public event Action<int[]> OnExtraTransformationAdded;
    private int[] extraTransformationsCounts = new int[5];
    private string usedExtraTransformations = "";
    public int[] ExtraTransformationsCounts { get => extraTransformationsCounts; }

    protected override void SingletonInit(){
        extraTransformationsCounts = Decode(PlayerPrefs.GetInt("ExtraTransformations", 0));
        usedExtraTransformations = PlayerPrefs.GetString("UsedExtraTransformations", "");
    }

    public void AddExtraTransformation(int index){
        extraTransformationsCounts[index]++;
        PlayerPrefs.SetInt("ExtraTransformations", Encode(extraTransformationsCounts));
        PlayerPrefs.Save();
        OnExtraTransformationAdded?.Invoke(extraTransformationsCounts);
    }

    public bool TryUseExtraTransformation(int index){
        if(extraTransformationsCounts[index] <= 0) return false;
        extraTransformationsCounts[index]--;
        PlayerPrefs.SetInt("ExtraTransformations", Encode(extraTransformationsCounts));
        PlayerPrefs.Save();
        PublishExtraTransformation(LevelManager.Instance.Level, index);
        return true;
    }

    private void PublishExtraTransformation(int levelIndex, int trIndex){
        while (usedExtraTransformations.Split('|').Length <= levelIndex)
            usedExtraTransformations += "|";
        
        string[] levels = usedExtraTransformations.Split('|');
        int[] alreadyUsedTr = levels[levelIndex] == "" ? new int[5] : Decode(Int32.Parse(levels[levelIndex]));
        alreadyUsedTr[trIndex]++;
        levels[levelIndex] = Encode(alreadyUsedTr).ToString();
        usedExtraTransformations = string.Join("|", levels);
        PlayerPrefs.SetString("UsedExtraTransformations", usedExtraTransformations);
        PlayerPrefs.Save();
    }

    public int[] GetUsedExtraTransformations(int levelIndex){
        string[] levels = usedExtraTransformations.Split('|');
        if (levels.Length <= levelIndex || string.IsNullOrEmpty(levels[levelIndex]))
            return new int[5];

        int[] alreadyUsedTr = Decode(Int32.Parse(levels[levelIndex]));
        return alreadyUsedTr;
    }

    private int Encode(int[] numbers){
        int encoded = 0;
        foreach (int num in numbers){
            encoded = encoded * 100 + num;
        }
        return encoded;
    }

    private int[] Decode(int encoded){
        int[] numbers = new int[5];
        for (int i = 4; i >= 0; i--){
            numbers[i] = encoded % 100;
            encoded /= 100;
        }
        return numbers;
    }

    [ContextMenu("SaveCurrent")]
    private void SaveCurrent() {
        PlayerPrefs.SetInt("ExtraTransformations", Encode(extraTransformationsCounts));
        PlayerPrefs.Save();
    }
}
