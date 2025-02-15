using TMPro;
using UnityEngine;

public class ShapeSection : ShopSection {
    [SerializeField] private CanvasGroup[] ticksCGs, locksCGs, pricesCGs;
    [SerializeField] private Transform[] ticksTransforms, locksTransforms, pricesTransforms;
    [SerializeField] private TextMeshProUGUI[] pricesTexts;
    [SerializeField] private RectTransform[] contentRTs;
    [SerializeField] private string priceFormat;
    [SerializeField] private Shop shop;
    [SerializeField] private int[] prices;
    [SerializeField] private float tickAnimationDuration;
    [SerializeField] private LeanTweenType tickAnimationEase;

    public override void Initialize(){
        int[] encodings = Customizations.Instance.Encodings;
        for(int i=0; i<ticksCGs.Length; i++){
            ticksCGs[i].alpha = encodings[i] == 2 ? 1f : 0f;

            if(encodings[i] == 0){
                locksCGs[i].alpha = 1f;
                pricesCGs[i].alpha = 1f;
                pricesTexts[i].text = string.Format(priceFormat, prices[i].ToString());
            }
            else{
                locksCGs[i].alpha = 0f;
                pricesCGs[i].alpha = 0f;
                contentRTs[i].anchoredPosition = Vector2.zero;
            }
        }
    }

    public void Select(int index){
        Audio.Instance.PlaySfx(AudioFx.ButtonPress);
        int[] encodings = Customizations.Instance.Encodings;
        if(encodings[index] == 2) return;
        if(encodings[index] == 1){
            int current = Customizations.Instance.SelectedIndex;
            ticksTransforms[current].LeanScale(Vector3.zero, tickAnimationDuration).setEase(tickAnimationEase).setOnComplete(() => ticksCGs[current].alpha = 0f);
            ticksCGs[index].alpha = 1f;
            ticksTransforms[index].localScale = Vector3.zero;
            ticksTransforms[index].LeanScale(Vector3.one, tickAnimationDuration).setEase(tickAnimationEase);

            Customizations.Instance.Deselect(current);
            Customizations.Instance.Select(index);
            return;
        }
        if(prices[index] > Diamonds.Instance.DiamondsCount){
            shop.ShakeDiamonds();
        }
        else{
            int current = Customizations.Instance.SelectedIndex;
            Diamonds.Instance.TrySpendDiamonds(prices[index]);
            shop.UpdateDiamonds();

            ticksTransforms[current].LeanScale(Vector3.zero, tickAnimationDuration).setEase(tickAnimationEase).setOnComplete(() => ticksCGs[current].alpha = 0f);
            Customizations.Instance.Deselect(current);
            Customizations.Instance.Select(index);

            pricesTransforms[index].LeanScale(Vector3.zero, tickAnimationDuration).setEase(tickAnimationEase).setOnComplete(() => pricesCGs[index].alpha = 0f);
            locksTransforms[index].LeanScale(Vector3.zero, tickAnimationDuration).setEase(tickAnimationEase).setOnComplete(() => locksCGs[index].alpha = 0f);
            ticksCGs[index].alpha = 1f;
            ticksTransforms[index].localScale = Vector3.zero;
            ticksTransforms[index].LeanScale(Vector3.one, tickAnimationDuration).setEase(tickAnimationEase);
            Vector2 startPos = contentRTs[index].anchoredPosition;
            LeanTween.value(contentRTs[index].gameObject, v => UpdatePosition(contentRTs[index], v), startPos, Vector2.zero, tickAnimationDuration).setEase(tickAnimationEase);
        }
    }

    private void UpdatePosition(RectTransform rt, Vector2 pos) => rt.anchoredPosition = pos;
}
