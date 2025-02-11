using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI), typeof(CanvasGroup))]
public class TargetScore : MonoBehaviour {
    [SerializeField] private TargetShape targetShape;
    [SerializeField] private float scoreMultiplier, minFontSize, maxFontSize;
    [SerializeField] private Color minColor, maxColor;
    [SerializeField] private AnimationCurve fontCurve, colorCurve;
    [SerializeField] private ShapeColorsSO colors;
    [SerializeField] private Vector2 positionOffset;
    [SerializeField] private CanvasGroup cg;
    private TextMeshProUGUI text;
    private RectTransform rt;
    private Camera cam;

    private void OnEnable() {
        targetShape.OnTargetChanged += HandleOnTargetChanged;
    }

    private void OnDisable(){
        if(targetShape != null){
            targetShape.OnTargetChanged -= HandleOnTargetChanged;
        }
    }

    private void Awake() {
        cam = Camera.main;
        text = GetComponent<TextMeshProUGUI>();
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
    }

    private void HandleOnTargetChanged(LevelData data){
        Vector2 offset = new Vector2(positionOffset.x * (data.targetPosData.worldPos.x < 0f ? -1 : 1), positionOffset.y * (data.targetPosData.worldPos.y < 0f ? -1 : 1));
        text.alignment = data.targetPosData.worldPos.x < 0f ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;

        rt.position = cam.WorldToScreenPoint(data.targetPosData.worldPos + offset);
        maxColor = colors.colors[(int)data.targetShapeType];
    }

    private void Update() {
        float normalizedScore = targetShape.Score;
        int score = Mathf.RoundToInt(normalizedScore * scoreMultiplier);
        text.text = score.ToString();
        text.color = Color.Lerp(minColor, maxColor, colorCurve.Evaluate(normalizedScore));
        text.fontSize = Mathf.Lerp(minFontSize, maxFontSize, fontCurve.Evaluate(normalizedScore));

        cg.alpha = score > 0 ? 1f : 0f;
    }
}