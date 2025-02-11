using UnityEngine;

public static class TransformationsUtils {
    public delegate void ApplyTransformation(float k, Transform shape, Transform origin, TransformationStepData initData);
    public static ApplyTransformation[] applyTransformation = new ApplyTransformation[] { MoveX, MoveY, Rotate, Scale, CenterOrigin };

    private static void MoveX(float k, Transform shape, Transform origin, TransformationStepData initData){
        shape.position = initData.worldPos + Vector2.right * k;
    }
    private static void MoveY(float k, Transform shape, Transform origin, TransformationStepData initData){
        shape.position = initData.worldPos + Vector2.up * k;
    }
    private static void Rotate(float k, Transform shape, Transform origin, TransformationStepData initData){
        origin.localEulerAngles = Vector3.forward * (initData.rot - k);
    }
    private static void Scale(float k, Transform shape, Transform origin, TransformationStepData initData){
        origin.localScale = Vector3.one * (initData.scale * k);
    }
    private static void CenterOrigin(float k, Transform shape, Transform origin, TransformationStepData initData){
        origin.position = shape.position;
        shape.localPosition = Vector3.zero;
        //shape.localEulerAngles += Vector3.forward * origin.localEulerAngles.z;
        //origin.localEulerAngles = Vector3.zero;
    }
}