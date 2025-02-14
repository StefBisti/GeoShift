using UnityEngine.UI;
using UnityEngine;

public class SoundOnOff : MonoBehaviour {
    [SerializeField] private Image image;
    [SerializeField] private Sprite onSprite, offSprite;
    private bool on = true;

    private void Start(){
        on = Audio.Instance.Volume == 1;
        image.sprite = on ? onSprite : offSprite;
    }

    public void Toggle(){
        on = !on;
        image.sprite = on ? onSprite : offSprite;
        Audio.Instance.ToggleVolume();
    }
}
