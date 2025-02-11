using System.Collections.Generic;
using UnityEngine;

public class ObstaclesParent : MonoBehaviour {
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private StaticObstacle staticObstacelPrefab;
    [SerializeField] private MovingObstacle movingObstacle;
    private List<GameObject> obstacles = new List<GameObject>();

    private void OnEnable(){
        levelManager.OnLevelChanged += SetObstacles;
    }

    private void OnDisable(){
        if(levelManager != null)
            levelManager.OnLevelChanged -= SetObstacles;
    }

    private void Awake(){
        SetObstacles(levelManager.Level);
    }

    private void SetObstacles(int level){
        foreach(GameObject go in obstacles){
            Destroy(go);
        }
        obstacles = new List<GameObject>();

        LevelData levelData = levelManager.GetLevelData(level);
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
