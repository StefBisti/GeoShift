using System.Collections;
using UnityEngine;

public class PointerAnimationHandlerUI : MonoBehaviour {
    [SerializeField] private float pointerMoveY, animationDuration;
    [SerializeField] private LeanTweenType upEase, downEase;
    private RectTransform rt;

    private void Start(){
        rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, -pointerMoveY);
        StartCoroutine(AnimatePointer());
    }

    private IEnumerator AnimatePointer(){
        while(true){
            rt.LeanMoveLocal(new Vector2(0, pointerMoveY), animationDuration).setEase(upEase);
            yield return new WaitForSeconds(animationDuration);
            rt.LeanMoveLocal(new Vector2(0, -pointerMoveY), animationDuration).setEase(downEase);
            yield return new WaitForSeconds(animationDuration);
        }
    }
}