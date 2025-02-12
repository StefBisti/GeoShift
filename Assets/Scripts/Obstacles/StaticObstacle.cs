using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
    public class StaticObstacle : MonoBehaviour {
    [SerializeField] private Vector2[] points;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private LineRenderer lineRenderer;
    private PolygonCollider2D polygonCollider2D;

    public void Initialize(StaticObstacleData data){
        points = data.points;
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        Set();
    }

    [ContextMenu("Set")]
    private void Set(){
        SetMesh();
        SetContour();
        SetCollider();
    }

    private void SetMesh(){
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[points.Length];
        for(int i=0; i<points.Length; i++){
            vertices[i] = points[i];
        }
        
        mesh.vertices = vertices;
        mesh.triangles = EarClipTriangulator.Triangulate(points).ToArray();
        meshFilter.mesh = mesh;
    }

    private void SetCollider(){
        if(polygonCollider2D == null) polygonCollider2D = GetComponent<PolygonCollider2D>();
        polygonCollider2D.points = points;
    }

    private void SetContour(){
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points.Select(v => (Vector3)v).ToArray());
    }
}

[System.Serializable]
public struct StaticObstacleData {
    public Vector2[] points;
}