using System;
using Playground.LinearTransformations;
using UnityEngine;

public class TargetShape : ShapeBase {
    public event Action<LevelData> OnTargetChanged;
    [SerializeField] private ShapeBase mainShape;
    [SerializeField] private Transform mainShapeTransform;
    [SerializeField] private float minDist, maxDist, addRotationThreshold, scaleThreshold, completionThreshold, completionTimeThreshold;
    [SerializeField] private float[] shapeSymmetryCounts;
    [SerializeField] private AnimationCurve distVsRotationScore;
    [SerializeField] private float animationDuration;
    [SerializeField] private LeanTweenType moveEase, rotationEase;
    private float timer = 0f;
    private bool playing = true;
    private LevelData currentLevelData;

    public float Score { get; private set;}

    private void Start() {
        currentLevelData = LevelManager.Instance.GetCurrentLevelData();
        OnTargetChanged?.Invoke(currentLevelData);

        SetShape(currentLevelData.targetShapeType);
        transform.position = currentLevelData.targetPosData.worldPos;
        transform.eulerAngles = Vector3.forward * currentLevelData.targetPosData.rot;
        transform.localScale = Vector3.one * currentLevelData.targetPosData.scale;
    }
        
    private void Update() {
        Score = GetScore();

        if(Score > completionThreshold){
            timer += Time.deltaTime;
            if(timer > completionTimeThreshold){
                timer = 0f;
                HandleOnCompletion();
            }
        }
        else timer = 0f;
    }


    public void CheckCompletion(){
        if(Score > completionThreshold)
            HandleOnCompletion();
    }

    private void HandleOnCompletion(){
        if(playing == false) return;
        playing = false;

        LevelManager.Instance.TriggerCompleteLevel();
        currentLevelData = LevelManager.Instance.GetCurrentLevelData(); // new level data

        SetShape(currentLevelData.targetShapeType);
        transform.LeanMove(currentLevelData.targetPosData.worldPos, animationDuration).setEase(moveEase);
        float startAngle = transform.eulerAngles.z;
        float startScale = transform.localScale.x;
        LeanTween.value(gameObject, t => UpdateRotation(startAngle, currentLevelData.targetPosData.rot, t), 0f, 1f, animationDuration).setEase(rotationEase);
        LeanTween.value(gameObject, UpdateScale, startScale, currentLevelData.targetPosData.scale, animationDuration).setEase(moveEase).setOnComplete(() => playing = true);

        OnTargetChanged?.Invoke(currentLevelData);
    }
    private void UpdateRotation(float a, float b, float t) => transform.eulerAngles = Vector3.forward * Mathf.LerpAngle(a, b, t);
    private void UpdateScale(float s) => transform.localScale = Vector3.one * s;

    private float GetScore() {
        if(mainShape.ShapeType != currentLevelData.targetShapeType) return 0;

        float dist = Vector2.Distance(mainShapeTransform.position, currentLevelData.targetPosData.worldPos);
        float distScore = 1 - Mathf.Clamp01(Mathf.InverseLerp(minDist, maxDist, dist));

        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(mainShapeTransform.eulerAngles.z, currentLevelData.targetPosData.rot));
        float symmetryPeriod = 360f / shapeSymmetryCounts[(int)currentLevelData.targetShapeType];
        angleDiff %= symmetryPeriod;
        if (angleDiff > symmetryPeriod / 2f)
            angleDiff = symmetryPeriod - angleDiff;
        float rotScore = Mathf.Clamp01(addRotationThreshold + 1f - (angleDiff / (symmetryPeriod / 2f)));

        float scaleDiff = Mathf.Abs(mainShapeTransform.lossyScale.x - currentLevelData.targetPosData.scale);
        float scaleScore = 1 - Mathf.Clamp01(scaleDiff - scaleThreshold);

        return (distScore + distVsRotationScore.Evaluate(distScore) * (scaleScore + rotScore)) / 3f;
    }
}
