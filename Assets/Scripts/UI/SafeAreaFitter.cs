using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour {
    private void Awake() {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = anchorMin + safeArea.size;

        anchorMin.y /= Screen.height;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = new Vector2(0, Mathf.Max(anchorMin.y, 0.02f));
        rectTransform.anchorMax = new Vector2(1f, Mathf.Min(anchorMax.y, 0.98f));
    }
}