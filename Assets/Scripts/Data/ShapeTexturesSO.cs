using UnityEngine;

[CreateAssetMenu(fileName = "ShapeTexturesSO", menuName = "Data/ShapeTexturesSO")]
public class ShapeTexturesSO : ScriptableObject{
    public ShapeTextureData[] data;
}

[System.Serializable]
public struct ShapeTextureData {
    public Sprite sprite;
}