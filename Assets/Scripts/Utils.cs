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

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        private static T _instance;
        public static T Instance {
            get => _instance;
        }

        private void Awake() {
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
            DontDestroyOnLoad(_instance);
            SingletonInit();
        }

        private void OnDestroy() {
            if (_instance == this)
                _instance = null;
        }

        protected virtual void SingletonInit() { }
    }

