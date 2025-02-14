#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

public class LevelMakerManager : MonoBehaviour {
    [SerializeField] private LevelsSO levelsSO;
    [SerializeField] private ShapeBase shape;
    [SerializeField] private Transform origin;
    [SerializeField] private Axis axis;
    [SerializeField] private SelectedTransformations selectedTransformations;
    [SerializeField] private LevelData levelData;
    [SerializeField] private bool isListeningStatic;
    [SerializeField] private List<Vector2> staticpoints = new List<Vector2>();
    [SerializeField] private float guiDotSize, guiLineThickness;

    private void OnDrawGizmos() {
        if (staticpoints == null || staticpoints.Count == 0) return;

        Gizmos.color = Color.red;
        foreach (Vector2 point in staticpoints){
            Gizmos.DrawSphere(new Vector3(point.x, point.y, 0), guiDotSize * 0.01f);
        }
        for (int i = 0; i < staticpoints.Count - 1; i++){
            DrawThickLineGizmos(staticpoints[i], staticpoints[i+1]);
        }
    }

    private void DrawThickLineGizmos(Vector3 start, Vector3 end){
        float distance = Vector3.Distance(start, end);
        int segmentCount = Mathf.CeilToInt(distance / guiLineThickness);

        for (int i = 0; i <= segmentCount; i++){
            Vector3 position = Vector3.Lerp(start, end, i / (float)segmentCount);
            Gizmos.DrawSphere(position, guiLineThickness * 0.5f);
        }
    }

    private void Update(){
        if(isListeningStatic && Input.GetMouseButtonDown(0)){
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 snap = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
            staticpoints.Add(snap);
        }
    }

    [ContextMenu("Add Target & Transformations")]
    private void AddTargetData(){
        PositionData data = new();
        data.worldPos = shape.transform.position;
        data.scale = shape.transform.lossyScale.x;
        data.rot = shape.transform.eulerAngles.z;

        levelData.targetPosData = data;
        levelData.targetShapeType = shape.ShapeType;
        levelData.availableTransformations = new int[5];

        FieldInfo fieldInfo1 = typeof(SelectedTransformations).GetField("selectedTransformations", BindingFlags.NonPublic | BindingFlags.Instance);
        int[] tr = (int[])fieldInfo1.GetValue(selectedTransformations);
        FieldInfo fieldInfo2 = typeof(SelectedTransformations).GetField("selectedTransformationsCount", BindingFlags.NonPublic | BindingFlags.Instance);
        int count = (int)fieldInfo2.GetValue(selectedTransformations);
        for(int i=0; i<count; i++){
            levelData.availableTransformations[tr[i]]++;
        }
    }

    [ContextMenu("Add Static")]
    private void AddStaticObstacle(){
        StaticObstacleData data = new();
        Vector2[] p = new Vector2[staticpoints.Count];
        for(int i=0; i<staticpoints.Count; i++)
            p[i] = staticpoints[i];

        data.points = p;
        levelData.staticObstacles.Add(data);
        staticpoints = new List<Vector2>();
    }
    [ContextMenu("Clear Static")]
    private void ClearStatic(){
        staticpoints = new List<Vector2>();
    }

    [ContextMenu("Add Moving Obstacles")]
    private void AddMovingObstacles(){
        foreach(GameObject o in GameObject.FindGameObjectsWithTag("Respawn")){
            MovingObstacleData data;
            MovingObstacle m = o.GetComponent<MovingObstacle>();

            FieldInfo isMoving = typeof(MovingObstacle).GetField("isMoving", BindingFlags.NonPublic | BindingFlags.Instance);
            data.isMoving = (bool)isMoving.GetValue(m);

            FieldInfo startPos = typeof(MovingObstacle).GetField("startPos", BindingFlags.NonPublic | BindingFlags.Instance);
            data.startPos = (Vector2)startPos.GetValue(m);

            FieldInfo endPos = typeof(MovingObstacle).GetField("endPos", BindingFlags.NonPublic | BindingFlags.Instance);
            data.endPos = (Vector2)endPos.GetValue(m);

            FieldInfo startRotation = typeof(MovingObstacle).GetField("startRotation", BindingFlags.NonPublic | BindingFlags.Instance);
            data.startRotation = (float)startRotation.GetValue(m);

            FieldInfo rotationSpeed = typeof(MovingObstacle).GetField("rotationSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            data.rotationSpeed = (float)rotationSpeed.GetValue(m);

            FieldInfo halfCycleDuration = typeof(MovingObstacle).GetField("halfCycleDuration", BindingFlags.NonPublic | BindingFlags.Instance);
            data.halfCycleDuration = (float)halfCycleDuration.GetValue(m);

            FieldInfo firstHalfEase = typeof(MovingObstacle).GetField("firstHalfEase", BindingFlags.NonPublic | BindingFlags.Instance);
            data.firstHalfEase = (LeanTweenType)firstHalfEase.GetValue(m);

            FieldInfo secondHalfEase = typeof(MovingObstacle).GetField("secondHalfEase", BindingFlags.NonPublic | BindingFlags.Instance);
            data.secondHalfEase = (LeanTweenType)secondHalfEase.GetValue(m);

            FieldInfo isOrbiting = typeof(MovingObstacle).GetField("isOrbiting", BindingFlags.NonPublic | BindingFlags.Instance);
            data.isOrbiting = (bool)isOrbiting.GetValue(m);

            FieldInfo childLocalPos = typeof(MovingObstacle).GetField("childLocalPos", BindingFlags.NonPublic | BindingFlags.Instance);
            data.childLocalPos = (Vector2)childLocalPos.GetValue(m);

            FieldInfo orbitingSpeed = typeof(MovingObstacle).GetField("orbitingSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
            data.orbitingSpeed = (float)orbitingSpeed.GetValue(m);

            FieldInfo isSquare = typeof(MovingObstacle).GetField("isSquare", BindingFlags.NonPublic | BindingFlags.Instance);
            data.isSquare = (bool)isSquare.GetValue(m);
            
            levelData.movingObstacles.Add(data);
        }
    }

    [ContextMenu("Clear")]
    private void ClearLevelData(){
        levelData = new LevelData();
    }

    [ContextMenu("Publish")]
    private void Publish(){
        levelsSO.levels.Add(levelData);
        levelData = new LevelData();

        Reset();

        EditorUtility.SetDirty(levelsSO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void Reset(){
        MethodInfo methodInfo = typeof(SelectedTransformations).GetMethod("Reset", BindingFlags.NonPublic | BindingFlags.Instance);
        methodInfo.Invoke(selectedTransformations, null);

        Vector3 shapePos = shape.transform.position;
        origin.position = Vector3.zero;
        shape.transform.position = shapePos;
        axis.MoveTo(Vector3.zero);
    }
}
#endif