using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsSO", menuName = "Data/LevelsSO")]
public class LevelsSO : ScriptableObject {
    public List<LevelData> levels;
}

[System.Serializable]
public struct LevelData {
    public PositionData targetPosData;
    public ShapeType targetShapeType;
    public int[] availableTransformations;
    public List<StaticObstacleData> staticObstacles;
    public List<MovingObstacleData> movingObstacles;
}

