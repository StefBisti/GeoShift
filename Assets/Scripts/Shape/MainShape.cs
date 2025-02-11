using System;
using UnityEngine;

public class MainShape : ShapeBase {
    public event Action OnDeath, OnReset;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Transform origin;
    [SerializeField] private Axis axis;
    [SerializeField] private Transform[] points;
    [SerializeField] private float colliderScaleFactor;
    [SerializeField] private ShapeDeathAnimation[] shapeDeathAnimations;
    [SerializeField] private float waitBeforeReset;
    private PolygonCollider2D polygonCollider2D;
    private Collider2D[] obstacleColliders;
    private bool playing = true;

    private void OnEnable(){
        levelManager.OnLevelChanged += HandleOnLevelChanged;
    }
    private void OnDisable(){
        if(levelManager != null)
            levelManager.OnLevelChanged -= HandleOnLevelChanged;
    }

    private void Awake(){
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        FindObstacleColliders();
    }

    private void Update(){
        if(playing == false) return;

        SetCollider();
        if(CheckCollision()){
            playing = false;
            ShapeDeathAnimation death = Instantiate(shapeDeathAnimations[(int)shapeType], Vector3.zero, Quaternion.identity, null);
            death.Initialize(transform);
            this.DoAfterSeconds(waitBeforeReset, () => Reset());
            OnDeath?.Invoke();
        }
    }

    private void SetCollider(){
        Vector2[] p = new Vector2[points.Length];
        for(int i=0; i<points.Length; i++){
            p[i] = points[i].localPosition * colliderScaleFactor;
        }
        polygonCollider2D.points = p;
    }

    private void FindObstacleColliders(){
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Enemy");
        obstacleColliders = new Collider2D[obstacles.Length];
        for(int i=0; i<obstacleColliders.Length; i++){
            obstacleColliders[i] = obstacles[i].GetComponent<Collider2D>();
        }
    }

    private bool CheckCollision(){
        foreach(Collider2D col in obstacleColliders){
            if(col != null && col.Distance(polygonCollider2D).isOverlapped) return true;
        }
        return false;
    }

    public void Reset() {
        playing = true;
        LevelData levelData = levelManager.GetLevelData(Mathf.Max(levelManager.Level - 1, 0));
        origin.transform.position = Vector3.zero;
        origin.transform.localEulerAngles = Vector3.forward * levelData.targetPosData.rot;
        origin.transform.localScale = Vector3.one * levelData.targetPosData.scale;
        transform.position = levelData.targetPosData.worldPos;

        OnReset?.Invoke();
    }

    private void HandleOnLevelChanged(int _) => FindObstacleColliders();
}
