using UnityEngine;

[CreateAssetMenu(fileName = "TransformationsSO", menuName = "Data/TransformationsSO")]
public class TransformationsSO : ScriptableObject {
    public TransformationData[] datas;
}

[System.Serializable]
public struct TransformationData {
    public float minRange, maxRange, initialValue, snapIntervals, snapThreshold;
    public string format;
    public int decimals;
    public bool needsSlider;
}
