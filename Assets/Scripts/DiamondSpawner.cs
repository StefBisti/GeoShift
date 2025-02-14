using System.Collections.Generic;
using UnityEngine;

public class DiamondSpawner : MonoBehaviour {
    [SerializeField] private Transform diamondPrefab, giftPrefab;
    [SerializeField] private bool randomRotation;
    [SerializeField] private float randomRotationSteps, diamondsScale, giftsScale, diamondScaleDuration;
    [SerializeField] private LeanTweenType diamondScaleEase;
    private List<Transform> currentDiamonds = new List<Transform>();

    private void Awake(){
        LevelManager.Instance.OnLevelChanged += HandleOnLevelChanged;
    }
    private void OnDestroy(){
        if(LevelManager.Instance != null){
            LevelManager.Instance.OnLevelChanged -= HandleOnLevelChanged;
        }
    }

    private void Start(){
        LevelData levelData = LevelManager.Instance.GetCurrentLevelData();
        Spawn(levelData.diamondPositions, levelData.lastIsGift);
    }
    
    private void RemoveCurrent(){
        foreach(Transform t in currentDiamonds){
            if(t != null) Destroy(t.gameObject);
        }
        currentDiamonds = new List<Transform>();
    }

    private void Spawn(List<Vector2> positions, bool lastIsGift){
        for(int i=0; i<positions.Count; i++){
            if(Diamonds.Instance.IsDiamondCollected(i)) continue;

            Transform reward = Instantiate((lastIsGift && i == positions.Count - 1) ? giftPrefab : diamondPrefab,
                positions[i], Quaternion.Euler(0f, 0f, (randomRotation && !(lastIsGift && i == positions.Count - 1)) ? Random.Range(-randomRotationSteps, randomRotationSteps) : 0f), transform);
            reward.name = "Diamond_" + i.ToString();
            reward.localScale = Vector3.zero;
            reward.LeanScale(Vector3.one * ((lastIsGift && i == positions.Count - 1) ? giftsScale : diamondsScale), diamondScaleDuration).setEase(diamondScaleEase);
            currentDiamonds.Add(reward);
        }
    }

    private void HandleOnLevelChanged(int level){
        LevelData levelData = LevelManager.Instance.GetLevelData(level);
        RemoveCurrent();
        Spawn(levelData.diamondPositions, levelData.lastIsGift);
    }
    
}
