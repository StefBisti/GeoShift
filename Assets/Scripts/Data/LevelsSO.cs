using UnityEngine;

[CreateAssetMenu(fileName = "LevelsSO", menuName = "Data/LevelsSO")]
public class LevelsSO : ScriptableObject {
    public LevelData[] levels;
}

[System.Serializable]
public struct LevelData {
    public ShapeType targetShapeType;
    public Vector2 targetPos;
    public float targetScale, targetRotation;
    public int[] availableTransformations;
}
