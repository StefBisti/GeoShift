using System;
using System.Collections.Generic;
using UnityEngine;

public class MainShape : ShapeBase {
    public event Action OnDeath, OnReset;
    [SerializeField] private Transform origin;
    [SerializeField] private Axis axis;
    [SerializeField] private Transform[] points;
    [SerializeField] private float colliderScaleFactor;
    [SerializeField] private ShapeDeathAnimation[] shapeDeathAnimations;
    [SerializeField] private float waitBeforeReset;
    private PolygonCollider2D polygonCollider2D;
    private List<Collider2D> obstacleColliders, diamondsColliders, giftsColliders;
    private bool playing = true;

    private void Awake(){
        LevelManager.Instance.OnLevelChanged += HandleOnLevelChanged;
        LevelManager.Instance.OnRequestedExit += HandleOnExit;
        Hearts.Instance.OnHeartsRefilled += HandleHeartsRefilled;
        SetShapeOnStart();

        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }
    private void OnDestroy(){
        if(LevelManager.Instance != null) {
            LevelManager.Instance.OnLevelChanged -= HandleOnLevelChanged;
            LevelManager.Instance.OnRequestedExit += HandleOnExit;
        }
        if(Hearts.Instance != null){
            Hearts.Instance.OnHeartsRefilled -= HandleHeartsRefilled;
        }
    }

    private void Start(){
        FindObstacleColliders();
        FindDiamondsColliders();
        FindGiftsColliders();
    }

    private void Update(){
        if(playing == false) return;

        SetCollider();
        if(CheckObstacleCollision()){
            playing = false;
            ShapeDeathAnimation death = Instantiate(shapeDeathAnimations[(int)shapeType], Vector3.zero, Quaternion.identity, null);
            death.Initialize(transform);
            this.DoAfterSeconds(waitBeforeReset, () => Reset());
            Hearts.Instance.RemoveHeart();
            OnDeath?.Invoke();
            Audio.Instance.PlaySfx(AudioFx.Explosion);
        }
        if(CheckDiamondCollision(out GameObject diamond)){
            Diamonds.Instance.AddDiamond(diamond.transform.position);
            int index = Int32.Parse(diamond.name.Split("_")[1]);
            Diamonds.Instance.CollectDiamond(index);
            Destroy(diamond);
            Audio.Instance.PlaySfx(AudioFx.Collect);
        }
        if(CheckGiftCollision(out GameObject gift)){
            Diamonds.Instance.GetGift(gift.transform.position);
            int index = Int32.Parse(gift.name.Split("_")[1]);
            Diamonds.Instance.CollectDiamond(index);
            Destroy(gift);
            Audio.Instance.PlaySfx(AudioFx.Collect);
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
        obstacleColliders = new List<Collider2D>();
        for(int i=0; i<obstacles.Length; i++){
            obstacleColliders.Add(obstacles[i].GetComponent<Collider2D>());
        }
    }

    private void FindDiamondsColliders(){
        GameObject[] diamonds = GameObject.FindGameObjectsWithTag("Diamond");
        diamondsColliders = new List<Collider2D>();
        for(int i=0; i<diamonds.Length; i++){
            diamondsColliders.Add(diamonds[i].GetComponent<Collider2D>());
        }
    }

    private void FindGiftsColliders(){
        GameObject[] gifts = GameObject.FindGameObjectsWithTag("Gift");
        giftsColliders = new List<Collider2D>();
        for(int i=0; i<gifts.Length; i++){
            giftsColliders.Add(gifts[i].GetComponent<Collider2D>());
        }
    }

    private bool CheckObstacleCollision(){
        foreach(Collider2D col in obstacleColliders){
            if(col != null && col.Distance(polygonCollider2D).isOverlapped) return true;
        }
        return false;
    }

    private bool CheckDiamondCollision(out GameObject diamond){
        foreach(Collider2D col in diamondsColliders){
            if(col != null && col.Distance(polygonCollider2D).isOverlapped){
                diamond = col.gameObject;
                diamondsColliders.Remove(col);
                return true;
            }
        }
        diamond = null;
        return false;
    }

    private bool CheckGiftCollision(out GameObject gift){
        foreach(Collider2D col in giftsColliders){
            if(col != null && col.Distance(polygonCollider2D).isOverlapped){
                gift = col.gameObject;
                giftsColliders.Remove(col);
                return true;
            }
        }
        gift = null;
        return false;
    }

    public void Reset() {
        if(Hearts.Instance.HeartsCount <= 0) return;

        playing = true;
        LevelData levelData = LevelManager.Instance.GetLevelData(Mathf.Max(LevelManager.Instance.Level - 1, 0));
        origin.transform.position = Vector3.zero;
        origin.transform.localEulerAngles = Vector3.forward * levelData.targetPosData.rot;
        origin.transform.localScale = Vector3.one * levelData.targetPosData.scale;
        transform.position = levelData.targetPosData.worldPos;
        axis.MoveTo(Vector3.zero);

        OnReset?.Invoke();
    }

    private void HandleHeartsRefilled(){
        if(playing == false)
            Reset();
    }

    private void SetShapeOnStart(){
        if(LevelManager.Instance.Level == 0) return;
            
        LevelData previousData = LevelManager.Instance.GetLevelData(LevelManager.Instance.Level - 1);
        origin.transform.localEulerAngles = Vector3.forward * previousData.targetPosData.rot;
        origin.transform.localScale = Vector3.one * previousData.targetPosData.scale;
        transform.position = previousData.targetPosData.worldPos;
        shapeType = previousData.targetShapeType;
    }

    private void HandleOnLevelChanged(int _){
        FindObstacleColliders();
        FindDiamondsColliders();
        FindGiftsColliders();
    }

    private void HandleOnExit(float _) => playing = false;
}
