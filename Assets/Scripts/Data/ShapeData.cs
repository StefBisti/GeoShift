using UnityEngine;

public enum ShapeType{
    Circle, Square, Triangle, Star
}

[System.Serializable]
public struct PositionData {
    public Vector2 worldPos;
    public float scale, rot;
}
