using UnityEngine;

[CreateAssetMenu(fileName = "LevelsSO", menuName = "Data/LevelsSO")]
public class LevelsSO : ScriptableObject {
    public LevelData[] levels;
}

[System.Serializable]
public struct LevelData {
    public PositionData targetPosData;
    public ShapeType targetShapeType;
    public int[] availableTransformations;
    public StaticObstacleData[] staticObstacles;
    public MovingObstacleData[] movingObstacles;
}

