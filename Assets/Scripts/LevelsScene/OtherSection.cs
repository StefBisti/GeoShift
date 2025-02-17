using TMPro;
using UnityEngine;

public class OtherSection : ShopSection {

    [SerializeField] private CanvasGroup videoHeartsCG, buyHeartsCG;
    [SerializeField] private float disabledHeartAlpha;
    [SerializeField] private TextMeshProUGUI heartPriceText;
    [SerializeField] private int heartPrice;
    [SerializeField] private TextMeshProUGUI[] extraTRCounts, extraTRPriceTexts;
    [SerializeField] private int extraTrPrice;
    [SerializeField] private string priceFormat;
    [SerializeField] private Shop shop;

    public override void Initialize(){
        if(Hearts.Instance.IsFull){
            videoHeartsCG.interactable = videoHeartsCG.blocksRaycasts = false;
            buyHeartsCG.interactable = buyHeartsCG.blocksRaycasts = false;
            videoHeartsCG.alpha = disabledHeartAlpha;
            buyHeartsCG.alpha = disabledHeartAlpha;
        }
        else{
            videoHeartsCG.SetCG(true);
            buyHeartsCG.SetCG(true);
        }
        heartPriceText.text = string.Format(priceFormat, heartPrice);
        foreach(TextMeshProUGUI text in extraTRPriceTexts){
            text.text = string.Format(priceFormat, extraTrPrice);
        }
        int[] extraTr = ExtraTransformations.Instance.ExtraTransformationsCounts;
        for(int i=0; i<extraTRCounts.Length; i++){
            extraTRCounts[i].text = extraTr[i].ToString();
        }
    }

    public void WatchVideoForHeart(){
        Audio.Instance.PlaySfx(AudioFx.ButtonPress);
        Hearts.Instance.AddHeart();
    }

    public void BuyHeart(){
        Audio.Instance.PlaySfx(AudioFx.ButtonPress);
        if(heartPrice > Diamonds.Instance.DiamondsCount){
            shop.ShakeDiamonds();
        }
        else{
            Diamonds.Instance.TrySpendDiamonds(heartPrice);
            Hearts.Instance.AddHeart();
            if(Hearts.Instance.IsFull){
                videoHeartsCG.interactable = videoHeartsCG.blocksRaycasts = false;
                buyHeartsCG.interactable = buyHeartsCG.blocksRaycasts = false;
                videoHeartsCG.alpha = disabledHeartAlpha;
                buyHeartsCG.alpha = disabledHeartAlpha;
            }
            shop.UpdateDiamonds();
        }
    }

    public void SelectTr(int index){
        Audio.Instance.PlaySfx(AudioFx.ButtonPress);
        if(extraTrPrice > Diamonds.Instance.DiamondsCount){
            shop.ShakeDiamonds();
        }
        else{
            Diamonds.Instance.TrySpendDiamonds(extraTrPrice);
            ExtraTransformations.Instance.AddExtraTransformation(index);
            int[] extraTr = ExtraTransformations.Instance.ExtraTransformationsCounts;
            for(int i=0; i<extraTRCounts.Length; i++){
                extraTRCounts[i].text = extraTr[i].ToString();
            }
            shop.UpdateDiamonds();
        }
    }
}
