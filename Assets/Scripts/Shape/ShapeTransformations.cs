using System.Collections.Generic;
using UnityEngine;

public class ShapeTransformations : MonoBehaviour {
    [SerializeField] private Transform shape, origin;
    [SerializeField] private TransformationsSO transformationsSO;
    [SerializeField] private CustomSlider slider;
    [SerializeField] private Axis axis;
    [SerializeField] private float animationDuration;
    [SerializeField] private LeanTweenType moveEase, rotationEase, scaleEase;
    private int currentTransformation = -1;
    private Stack<Move> moves = new();
    private TransformationStepData currentInitialData;
    private float currentK;


    private void Awake(){
        LevelManager.Instance.OnLevelChanged += HandleOnLevelChanged;
    }
    private void OnDestroy(){
        if(LevelManager.Instance != null)
            LevelManager.Instance.OnLevelChanged -= HandleOnLevelChanged;
    }

    public void SelectTransformation(int index){
        if(currentTransformation != -1){
            moves.Push(new Move{
                initialData = currentInitialData,
                k = currentK,
                trIndex = currentTransformation
            });
        }
        currentInitialData = GetPositionData();
        currentK = transformationsSO.datas[index].initialValue;
        currentTransformation = index;

        if(index == 4) {
            TransformationsUtils.applyTransformation[4](0, shape, origin, currentInitialData);
            axis.MoveTo(shape.position);
        }
    }

    public void UndoToPreviousTransformation(){
        if(moves.Count == 0){
            ResetTransformation(currentTransformation, currentInitialData, currentInitialData);
        }
        else {
            Move m = moves.Pop();
            ResetTransformation(currentTransformation, currentInitialData, m.initialData);
            currentInitialData = m.initialData;
            currentTransformation = m.trIndex;
            currentK = m.k;
            slider.HardSetValue(currentK);
        }
    }

    public void TweakTransformation(float k){
        if(currentTransformation == -1 || currentTransformation == 4) return;
        currentK = k;
        TransformationsUtils.applyTransformation[currentTransformation](k, shape, origin, currentInitialData);
    }

    private void ResetTransformation(int madeTransformation, TransformationStepData currentInitialData, TransformationStepData previousInitialData){
        float start = currentK;
        float end = transformationsSO.datas[madeTransformation].initialValue;

        if(madeTransformation != 4){
            LeanTween.value(gameObject, t => {
                TransformationsUtils.applyTransformation[madeTransformation](t, shape, origin, currentInitialData);
            }, start, end, animationDuration).setEase((madeTransformation == 2 || madeTransformation == 3) ? rotationEase : moveEase);
        }
        else{
            origin.position = previousInitialData.originWorldPos;
            shape.position = currentInitialData.worldPos;
            axis.MoveTo(previousInitialData.originWorldPos);
        }
    }

    private void HandleOnLevelChanged(int _){
        currentTransformation = -1;
        moves = new Stack<Move>();
        Vector3 shapePos = shape.position;
        origin.position = Vector3.zero;
        shape.position = shapePos;
        axis.MoveTo(Vector3.zero);
    }

    private TransformationStepData GetPositionData(){
        return new TransformationStepData {
            worldPos = shape.position,
            originWorldPos = origin.position,
            rot = origin.localEulerAngles.z,
            scale = origin.localScale.x
        };
    }

    private struct Move {
        public TransformationStepData initialData;
        public int trIndex;
        public float k;
    }
}

public struct TransformationStepData {
    public Vector2 worldPos, originWorldPos;
    public float scale, rot;
}
