using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public static bool IsPointerOverUIObject() {
        return IsPointerOverUIObject(Input.mousePosition);
    }
    public static bool IsPointerOverUIObject(Vector3 pos) {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = pos;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
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

