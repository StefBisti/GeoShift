using System;
using UnityEngine;

public class ShapeBase : MonoBehaviour {
    public event Action<int> OnShapeChanged;
    [SerializeField] protected ShapeType shapeType;

    public ShapeType ShapeType { get => shapeType; }

    public void SetShape(int index){
        if(index == (int)shapeType) return;
        shapeType = (ShapeType)index;
        OnShapeChanged?.Invoke(index);
    }
    public void SetShape(ShapeType shapeType) => SetShape((int)shapeType);
}

