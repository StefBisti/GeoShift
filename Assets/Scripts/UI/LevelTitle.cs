using System.Collections;
using TMPro;
using UnityEngine;

public class LevelTitle : MonoBehaviour {
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private float topY, bottomY;
    [SerializeField] private TextMeshProUGUI mainValue, secondaryValue;
    [SerializeField] private RectTransform mainRT, secondaryRT;
    [SerializeField] private CanvasGroup mainCG, secondaryCG;
    [SerializeField] private float moveDuration, alphaDuration;
    [SerializeField] private LeanTweenType moveEase, alphaShowEase, alphaHideEase;
    private float xPos;

    private void OnEnable(){
        levelManager.OnLevelChanged += SetNewLevel;
    }
    private void OnDisable(){
        if(levelManager != null)
            levelManager.OnLevelChanged -= SetNewLevel;
    }

    private void Awake(){
        xPos = mainRT.anchoredPosition.x;
    }

    private void Start(){
        mainValue.text = (levelManager.Level + 1).ToString();
    }

    private void SetNewLevel(int level) => StartCoroutine(HandleAnimation(level+1));

    private IEnumerator HandleAnimation(int level){
        secondaryRT.anchoredPosition = new Vector2(xPos, topY);
        secondaryValue.text = level.ToString();

        bool animating = true;
        secondaryCG.LeanAlpha(1f, alphaDuration).setEase(alphaShowEase);
        mainCG.LeanAlpha(0f, alphaDuration).setEase(alphaHideEase);
        mainRT.LeanMoveLocalY(bottomY, moveDuration).setEase(moveEase);
        secondaryRT.LeanMoveLocalY(0f, moveDuration).setEase(moveEase).setOnComplete(() => animating = false);

        yield return new WaitWhile(() => animating);

        mainRT.anchoredPosition = new Vector2(xPos, 0);
        mainValue.text = level.ToString();
        secondaryCG.alpha = 0f;
        mainCG.alpha = 1f;
    }

}
