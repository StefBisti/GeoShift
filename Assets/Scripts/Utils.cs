using System;
using System.Collections;
using UnityEngine;

public static class Utils {
    public static void SetCG(this CanvasGroup cg, bool value){
        cg.alpha = value ? 1f : 0f;
        cg.interactable = cg.blocksRaycasts = value;
    }

    private static IEnumerator DoAfterSeconds(float seconds, Action action){
        yield return new WaitForSeconds(seconds);
        yield return new WaitForEndOfFrame();
        action?.Invoke();
    }

    public static void DoAfterSeconds(this MonoBehaviour go, float seconds, Action action){
        go.StartCoroutine(DoAfterSeconds(seconds, action));
    }
}
