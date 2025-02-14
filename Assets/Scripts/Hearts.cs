using System;
using System.Collections;
using UnityEngine;

public class Hearts : Singleton<Hearts> {
    public event Action<int> OnHeartsCountChanged, OnHeartRemoved;
    public event Action OnHeartsRefilled;
    [SerializeField] private int maxHearts = 3, hoursToRefill = 4;
    private int heartsCount;

    public int HeartsCount { get => heartsCount; }
    public bool IsFull { get => heartsCount == maxHearts; }
    public int HoursToRefill { get => hoursToRefill; }

    protected override void SingletonInit() {
        heartsCount = PlayerPrefs.GetInt("CurrentHearts", maxHearts);

        string lastHeartLostTimeString = PlayerPrefs.GetString("LastHeartLostTime", DateTime.UtcNow.ToString());
        DateTime lastHeartLostTime = DateTime.Parse(lastHeartLostTimeString);
        TimeSpan timeSinceLastHeartLost = DateTime.UtcNow - lastHeartLostTime;

        int heartsToRefill = Mathf.FloorToInt((float)(timeSinceLastHeartLost.TotalHours / hoursToRefill));
        heartsCount = Mathf.Min(heartsCount + heartsToRefill, maxHearts);

        if (heartsToRefill > 0){
            PlayerPrefs.SetString("LastHeartLostTime", DateTime.UtcNow.ToString());
        }

        PlayerPrefs.SetInt("CurrentHearts", heartsCount);
        PlayerPrefs.Save();

        StartCoroutine(RefillHeartsOverTime());
    }

    public void RemoveHeart(){
        if(heartsCount <= 0) return;

        heartsCount--;
        PlayerPrefs.SetInt("CurrentHearts", heartsCount);
        if(heartsCount == maxHearts - 1)
            PlayerPrefs.SetString("LastHeartLostTime", DateTime.UtcNow.ToString());
        PlayerPrefs.Save();
        OnHeartRemoved?.Invoke(heartsCount);
    }

    public void Refill(){
        heartsCount = maxHearts;
        PlayerPrefs.SetInt("CurrentHearts", heartsCount);
        PlayerPrefs.Save();
        OnHeartsRefilled?.Invoke();
        OnHeartsCountChanged?.Invoke(heartsCount);
    }

    private IEnumerator RefillHeartsOverTime(){
        while (true){
            if (heartsCount < maxHearts) {
                string lastHeartLostTimeString = PlayerPrefs.GetString("LastHeartLostTime", DateTime.UtcNow.ToString());
                DateTime lastHeartLostTime = DateTime.Parse(lastHeartLostTimeString);
                TimeSpan timeSinceLastHeartLost = DateTime.UtcNow - lastHeartLostTime;
                if (timeSinceLastHeartLost.TotalHours >= hoursToRefill){
                    heartsCount++;
                    PlayerPrefs.SetInt("CurrentHearts", heartsCount);
                    PlayerPrefs.SetString("LastHeartLostTime", DateTime.UtcNow.ToString());
                    PlayerPrefs.Save();
                }
            }

            yield return new WaitForSeconds(60);
        }
    }

    [ContextMenu("Set Hearts")]
    private void SetHearts(){
        PlayerPrefs.SetInt("CurrentHearts", maxHearts);
    }
}
