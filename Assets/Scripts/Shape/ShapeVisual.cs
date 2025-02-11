using UnityEngine;

public class ShapeVisual : MonoBehaviour {
    [SerializeField] private ShapeBase parent;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform[] points;
    [SerializeField] private float alphaVertices, alphaShape;

    [SerializeField] private float unitSize;
    [SerializeField] private ShapeColorsSO colors;
    [SerializeField] private float morphDuration;
    [SerializeField] private AnimationCurve morphEase;
    private float morphTimer = 0;

    [Header("Hand Drawn Animation")]
    [SerializeField] private bool isStatic;
    [SerializeField] private float startRotation, rotationsThreshold, moveYThreshold, moveXStep;
    [SerializeField] private float timeBetweenSnaps;
        
        
    private float timer = 0f;

    private void OnEnable(){
        parent.OnShapeChanged += HandleOnShapeChanged;
        if(parent is MainShape){
            (parent as MainShape).OnDeath += Hide;
            (parent as MainShape).OnReset += Show;
        }
    }
    private void OnDisable(){
        if(parent == null) return;

        parent.OnShapeChanged -= HandleOnShapeChanged;
        if(parent is MainShape){
            (parent as MainShape).OnDeath -= Hide;
            (parent as MainShape).OnReset -= Show;
        }
    }

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    private void Start(){
        Vector2[] positions = ShapeUtils.GetShape[(int)parent.ShapeType](points.Length, unitSize);
        lineRenderer.positionCount = positions.Length;
        for(int i=0; i < positions.Length; i++){
            points[i].localPosition = positions[i];
            lineRenderer.SetPosition(i, positions[i]);
        }
        SetMesh(positions);
        SetColor((int)parent.ShapeType);
    }

    private void Update() {
        if(morphTimer > 0){
            morphTimer -= Time.deltaTime;
            Vector2[] positions = new Vector2[points.Length];
            for(int i=0; i<points.Length; i++){
                lineRenderer.SetPosition(i, points[i].localPosition);
                positions[i] = points[i].localPosition;
            }
            SetMesh(positions);
        }

        if(isStatic == false){
            timer += Time.deltaTime;
            if(timer >= timeBetweenSnaps){
                timer = 0f;

                float rotationOffset = Random.Range(-rotationsThreshold, rotationsThreshold);
                float yOffset = Random.Range(-moveYThreshold, moveYThreshold);
                float xOffset = Random.Range(-4, 5) * moveXStep;

                meshRenderer.material.SetTextureOffset("_MainTex", new Vector2(xOffset, yOffset));
                meshRenderer.material.SetFloat("_Rotation", startRotation + rotationOffset);
            }
        }
    }

    private void HandleOnShapeChanged(int shapeIndex){
        Vector2[] positions = ShapeUtils.GetShape[shapeIndex](points.Length, unitSize);
        for(int i=0; i < positions.Length; i++){
            points[i].LeanMoveLocal(positions[i], morphDuration).setEase(morphEase);
        }
        SetColor(shapeIndex);
        float threshold = 1f;
        morphTimer = morphDuration + threshold;
    }

    private void SetMesh(Vector2[] points){
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[points.Length + 1];
        int[] triangles = new int[3 * points.Length];
        Vector2[] uv = new Vector2[points.Length + 1];
        vertices[0] = Vector3.zero;

        float left = 999, right = -999, bottom = 999, top = -999;

        for(int i=0; i<points.Length; i++){
            vertices[i+1] = points[i];
            triangles[i*3+0] = 0;
            triangles[i*3+1] = (i == points.Length - 1) ? 1 : i+2;
            triangles[i*3+2] = i+1;

            left = Mathf.Min(left, points[i].x);
            right = Mathf.Max(right, points[i].x);
            bottom = Mathf.Min(bottom, points[i].y);
            top = Mathf.Max(top, points[i].y);
        }

        uv[0] = new Vector2(
            Mathf.InverseLerp(left, right, 0),
            Mathf.InverseLerp(bottom, top, 0)
        );
        for(int i=0; i<points.Length; i++){
            uv[i+1] = new Vector2(
                Mathf.InverseLerp(left, right, points[i].x),
                Mathf.InverseLerp(bottom, top, points[i].y)
            );
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        meshFilter.mesh = mesh;
    }

    private void SetColor(int colorIndex, float alphaShape, float alphaVertices){
        Color c = colors.colors[colorIndex];
        Color shapeCol = new Color(c.r, c.g, c.b, alphaShape);
        Color vertsCol = new Color(c.r, c.g, c.b, alphaVertices);
        meshRenderer.material.SetColor("_Color", shapeCol);
        lineRenderer.startColor = shapeCol;
        lineRenderer.endColor = shapeCol;
        foreach(Transform p in points) {
            p.GetComponent<SpriteRenderer>().color = vertsCol;
        }
    }
    private void SetColor(int colorIndex) => SetColor(colorIndex, alphaShape, alphaVertices);
    private void Hide() => SetColor((int)parent.ShapeType, 0f, 0f);
    private void Show() => SetColor((int)parent.ShapeType, alphaShape, alphaVertices);
}
