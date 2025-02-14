using UnityEngine;

public class Audio : Singleton<Audio> {
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource[] sfxSources;
    private int volume = 1;

    public int Volume { get => volume; }

    protected override void SingletonInit(){
        volume = PlayerPrefs.GetInt("Volume", 1);
        AudioListener.volume = volume;
    }

    public void PlaySfx(AudioFx audioFx){
        sfxSources[(int)audioFx].Play();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        PlaySfx(AudioFx.Swoosh);
    }

    public void ToggleVolume(){
        volume = 1 - volume;
        PlayerPrefs.SetInt("Volume", volume);
        AudioListener.volume = volume;
    }
}

public enum AudioFx {
    Collect, Explosion, PartyHorn, RustlingPaper, BoxOpening, ButtonPress, Swoosh
}
