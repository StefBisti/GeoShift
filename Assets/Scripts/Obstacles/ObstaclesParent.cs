using System.Collections.Generic;
using UnityEngine;

public class ObstaclesParent : MonoBehaviour {
    [SerializeField] private StaticObstacle staticObstacelPrefab;
    [SerializeField] private MovingObstacle movingObstacle;
    private List<GameObject> obstacles = new List<GameObject>();

    private void Awake(){
        LevelManager.Instance.OnLevelChanged += SetObstacles;
        SetObstacles(LevelManager.Instance.Level);
    }

    private void OnDestroy(){
        if(LevelManager.Instance != null)
            LevelManager.Instance.OnLevelChanged -= SetObstacles;
    }

    private void SetObstacles(int level){
        foreach(GameObject go in obstacles){
            Destroy(go);
        }
        obstacles = new List<GameObject>();

        LevelData levelData = LevelManager.Instance.GetLevelData(level);
        if(levelData.staticObstacles != null){
            foreach(StaticObstacleData data in levelData.staticObstacles){
                StaticObstacle obstacle = Instantiate(staticObstacelPrefab, Vector3.zero, Quaternion.identity, transform);
                obstacle.Initialize(data);
                obstacles.Add(obstacle.gameObject);
            }
        }
        if(levelData.movingObstacles != null){
            foreach(MovingObstacleData data in levelData.movingObstacles){
                MovingObstacle obstacle = Instantiate(movingObstacle, Vector3.zero, Quaternion.identity, transform);
                obstacle.Initialize(data);
                obstacles.Add(obstacle.gameObject);
            }
        }
    }
}
