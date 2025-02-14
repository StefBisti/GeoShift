using UnityEngine;

public class MenuButton : MonoBehaviour {
    public void TriggerMenu() => LevelManager.Instance.GoToMenu();
}
