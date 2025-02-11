using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
    public class StaticObstacle : MonoBehaviour {
    [SerializeField] private StaticObstaclePart[] obstacleParts;
    [SerializeField] private StaticObstacleContourVertex[] contourVertices;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private LineRenderer lineRenderer;
    private PolygonCollider2D polygonCollider2D;

    public void Initialize(StaticObstacleData data){
        obstacleParts = data.obstacleParts;
        contourVertices = data.contourVertices;
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        Set();
    }

    private void Set(){
        SetMesh();
        SetCollider();
        SetContour();
    }

    private void SetMesh(){
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[obstacleParts.Length * 4];
        int[] triangles = new int[obstacleParts.Length * 2 * 3];

        for(int i=0; i < obstacleParts.Length; i++){
            Vector2 bl = obstacleParts[i].bottomLeft;
            Vector2 tr = obstacleParts[i].topRight;
            Vector2 tl = new Vector2(bl.x, tr.y);
            Vector2 br = new Vector2(tr.x, bl.y);

            vertices[i*4+0] = bl;
            vertices[i*4+1] = tl;
            vertices[i*4+2] = tr;
            vertices[i*4+3] = br;

            triangles[i*6+0] = i*4;
            triangles[i*6+1] = i*4+1;
            triangles[i*6+2] = i*4+2;
            triangles[i*6+3] = i*4;
            triangles[i*6+4] = i*4+2;
            triangles[i*6+5] = i*4+3;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        meshFilter.mesh = mesh;
    }

    private void SetCollider(){
        Vector2[] contour = GetContour();
        polygonCollider2D.points = contour;
    }

    private void SetContour(){
        Vector2[] contour = GetContour();
        lineRenderer.positionCount = contour.Length;
        lineRenderer.SetPositions(contour.Select(v => (Vector3)v).ToArray());
    }

    private Vector2[] GetContour(){
        List<Vector2> contour = new List<Vector2>();

        Vector2[,] partCorners = new Vector2[obstacleParts.Length, 4];
        for(int i=0; i<obstacleParts.Length; i++){
            Vector2 bl = obstacleParts[i].bottomLeft;
            Vector2 tr = obstacleParts[i].topRight;
            Vector2 tl = new Vector2(bl.x, tr.y);
            Vector2 br = new Vector2(tr.x, bl.y);
            partCorners[i, 0] = bl;
            partCorners[i, 1] = tl;
            partCorners[i, 2] = tr;
            partCorners[i, 3] = br;
        }

        foreach(StaticObstacleContourVertex c in contourVertices){
            contour.Add(partCorners[c.referencePartIndex, (int)c.corner]);
        }

        return contour.ToArray();
    }
}

[System.Serializable]
public struct StaticObstaclePart {
    public Vector2 bottomLeft, topRight;
}

[System.Serializable]
public struct StaticObstacleContourVertex {
    public int referencePartIndex;
    public StaticObstacleCorner corner;
}

public enum StaticObstacleCorner {
    BottomLeft, TopLeft, TopRight, BottomRight
}

[System.Serializable]
public struct StaticObstacleData {
    public StaticObstaclePart[] obstacleParts;
    public StaticObstacleContourVertex[] contourVertices;
}