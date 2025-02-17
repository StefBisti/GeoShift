using UnityEngine;

public class Customizations : Singleton<Customizations> {
    private int[] encodings;
    public int[] Encodings { get => encodings; }

    public int SelectedIndex{
        get {
            for(int i=0; i<encodings.Length; i++){
                if(encodings[i] == 2) return i;
            }
            return 0;
        }
    }

    protected override void SingletonInit(){
        encodings = Decode(PlayerPrefs.GetInt("Customizations", 0));

        if(encodings[0] == 0){
            encodings[0] = 2;
            encodings[1] = encodings[2] = 1;
            int encoded = Encode(encodings);
            PlayerPrefs.SetInt("Customizations", encoded);
            PlayerPrefs.Save();
        }
    }

    public void Deselect(int index){
        encodings[index] = 1;
    }

    public void Select(int index){
        encodings[index] = 2;
        int encoded = Encode(encodings);
        PlayerPrefs.SetInt("Customizations", encoded);
        PlayerPrefs.Save();
    }

    public void Unlock(int index){
        encodings[index] = 1;
        int encoded = Encode(encodings);
        PlayerPrefs.SetInt("Customizations", encoded);
        PlayerPrefs.Save();
    }

    private int Encode(int[] numbers){
        int encoded = 0;
        foreach (int num in numbers){
            encoded = encoded * 10 + num;
        }
        return encoded;
    }

    private int[] Decode(int encoded){
        int[] numbers = new int[9];
        for (int i = 8; i >= 0; i--){
            numbers[i] = encoded % 10;
            encoded /= 10;
        }
        return numbers;
    }
}
