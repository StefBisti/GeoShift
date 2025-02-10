using UnityEngine;

namespace Playground.LinearTransformations {
    public class CameraHandler : MonoBehaviour {
        //[SerializeField] private MainShapeBehaviour shapeBehaviour;
        [SerializeField] private AnimationCurve shakeMagnitudeCurve;
        [SerializeField] private float shakeDuration, shakeMinMagnitude, shakeMaxMagnitude;
        private float shakeTimer = 0f;
        private Vector2 initialPosition;
        private Camera cam;


        private void OnEnable(){
            //shapeBehaviour.OnDeath += HandleOnDeath;
        }

        private void OnDisable(){
            //if(shapeBehaviour != null)
            //    shapeBehaviour.OnDeath -= HandleOnDeath;
        }

        private void Start() {
            cam = Camera.main;
            initialPosition = cam.transform.position;
            shakeTimer = shakeDuration;
        }

        private void Update() {
            if (shakeTimer < shakeDuration){
                float mag = Mathf.Lerp(shakeMinMagnitude, shakeMaxMagnitude, shakeMagnitudeCurve.Evaluate(shakeTimer / shakeDuration));
                transform.localPosition = initialPosition + Random.insideUnitCircle.normalized * mag;
                shakeTimer += Time.deltaTime;
            }
        }

        private void HandleOnDeath(){
            shakeTimer = 0f;
        }
    }
}