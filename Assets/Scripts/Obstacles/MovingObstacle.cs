using System.Collections;
using UnityEngine;

public class MovingObstacle : MonoBehaviour {
    [SerializeField] private Transform parent, child;
    [SerializeField] private PolygonCollider2D triangleCollider;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite triangleSprite, boxSprite;
    [SerializeField] private bool isSquare = false;
    private bool lastIsSquare;

    [Header("Moving")]
    [SerializeField] private bool isMoving = false;
    [SerializeField] private Vector2 startPos, endPos;
    [SerializeField] private float startRotation, rotationSpeed, halfCycleDuration;
    [SerializeField] private LeanTweenType firstHalfEase, secondHalfEase;

    [Header("Orbiting")]
    [SerializeField] private bool isOrbiting = false;
    [SerializeField] private Vector2 childLocalPos;
    [SerializeField] private float orbitingSpeed;

    public void Initialize(MovingObstacleData data){
        isMoving = data.isMoving;
        startPos = data.startPos;
        endPos = data.endPos;
        startRotation = data.startRotation;
        rotationSpeed = data.rotationSpeed;
        halfCycleDuration = data.halfCycleDuration;
        firstHalfEase = data.firstHalfEase;
        secondHalfEase = data.secondHalfEase;
        isOrbiting = data.isOrbiting;
        childLocalPos = data.childLocalPos;
        orbitingSpeed = data.orbitingSpeed;
        isSquare = data.isSquare;
    }

    private void Start(){
        parent.position = startPos;
        child.localPosition = childLocalPos;

        triangleCollider.enabled = !isSquare;
        boxCollider.enabled = isSquare;
        spriteRenderer.sprite = isSquare ? boxSprite : triangleSprite;

        child.localEulerAngles = Vector3.forward * startRotation;

        if(isMoving) {
            StartCoroutine(Movement());
        }
    }

    private void Update(){
        child.Rotate(0, 0, rotationSpeed * 100 * Time.deltaTime);

        if(isOrbiting){
            parent.Rotate(0, 0, orbitingSpeed * 100 * Time.deltaTime);
        }

        if(lastIsSquare != isSquare){
            lastIsSquare = isSquare;
            triangleCollider.enabled = !isSquare;
            boxCollider.enabled = isSquare;
            spriteRenderer.sprite = isSquare ? boxSprite : triangleSprite;
        }
    }

    private IEnumerator Movement(){
        while(true){
            transform.LeanMove(endPos, halfCycleDuration).setEase(firstHalfEase);
            yield return new WaitForSeconds(halfCycleDuration);
            transform.LeanMove(startPos, halfCycleDuration).setEase(secondHalfEase);
            yield return new WaitForSeconds(halfCycleDuration);
        }
    }
    [ContextMenu("Set Rotation")]
    private void SetRotation(){
        child.localEulerAngles = Vector3.forward * startRotation;
    }
}

[System.Serializable]
public struct MovingObstacleData {
    public bool isSquare;
    public bool isMoving;
    public Vector2 startPos, endPos;
    public float startRotation, rotationSpeed, halfCycleDuration;
    public LeanTweenType firstHalfEase, secondHalfEase;
    public bool isOrbiting;
    public Vector2 childLocalPos;
    public float orbitingSpeed;
}
