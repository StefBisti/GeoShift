
using UnityEngine;

public class ShapeSelectors : MonoBehaviour {
    [SerializeField] private RectTransform[] buttons;
    [SerializeField] private CanvasGroup[] cgs;
    [SerializeField] private Vector2 distanceBetweenButtons;
    [SerializeField] private float animationTime;
    [SerializeField] private LeanTweenType showAnimationEase, hideAnimationEase;
    private bool ongoingAnimation = false, buttonsShown = false;
    private int selectedIndex = 0;

    private void ShowButtons(){
        if(ongoingAnimation == true) return;
        ongoingAnimation = true;

        foreach(CanvasGroup cg in cgs)
            cg.SetCG(true);

        int c = transform.childCount;
        for(int i=0; i < c-2; i++){
            Vector2 pos = (c-i-1) * distanceBetweenButtons;
            transform.GetChild(i).GetComponent<RectTransform>().LeanMoveLocal(pos, animationTime).setEase(showAnimationEase);
        }
        transform.GetChild(c-2).GetComponent<RectTransform>().LeanMoveLocal(distanceBetweenButtons, animationTime).setEase(showAnimationEase).setOnComplete(() => ongoingAnimation = false);
    }

    private void HideButtons(){
        if(ongoingAnimation == true) return;
        ongoingAnimation = true;
            
        int c = transform.childCount;
        for(int i=0; i < c-1; i++){
            transform.GetChild(i).GetComponent<RectTransform>().LeanMoveLocal(Vector2.zero, animationTime).setEase(hideAnimationEase);
        }

        transform.GetChild(c-1).GetComponent<RectTransform>().LeanMoveLocal(Vector2.zero, animationTime).setEase(hideAnimationEase).setOnComplete(() => {
            ongoingAnimation = false;
            for(int i=0; i<cgs.Length; i++){
                if(i != selectedIndex)
                    cgs[i].SetCG(false);
            }
        });
    }

    public void Toggle(int index){
        if(buttonsShown == false){
            ShowButtons();
            buttonsShown = true;
        }
        else {
            buttons[index].SetAsLastSibling();
            selectedIndex = index;
            HideButtons();
            buttonsShown = false;
        }
    }
}