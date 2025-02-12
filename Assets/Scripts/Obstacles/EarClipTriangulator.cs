using System.Collections.Generic;
using UnityEngine;

public static class EarClipTriangulator {
    public static List<int> Triangulate(Vector2[] points) {
        if(points.Length < 3) return new List<int>();

        List<int> indices = new List<int>();
        List<int> remaining = new List<int>();

        for (int i = 0; i < points.Length; i++)
            remaining.Add(i);

        while (remaining.Count > 3){
            bool earFound = false;

            for (int i = 0; i < remaining.Count; i++){
                int prev = remaining[(i - 1 + remaining.Count) % remaining.Count];
                int curr = remaining[i];
                int next = remaining[(i + 1) % remaining.Count];

                if (IsEar(points, prev, curr, next, remaining)){
                    indices.Add(prev);
                    indices.Add(next);
                    indices.Add(curr);
                    remaining.RemoveAt(i);
                    earFound = true;
                    break;
                }
            }

            if (!earFound) break;
        }

        indices.Add(remaining[0]);
        indices.Add(remaining[2]);
        indices.Add(remaining[1]);

        return indices;
    }

    private static bool IsEar(Vector2[] points, int a, int b, int c, List<int> remaining){
        if (!IsConvex(points[a], points[b], points[c])) return false;

        foreach (int i in remaining){
            if (i == a || i == b || i == c) continue;
            if (PointInTriangle(points[i], points[a], points[b], points[c])) return false;
        }
        return true;
    }

    private static bool IsConvex(Vector2 a, Vector2 b, Vector2 c){
        return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x) > 0;
    }

    private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c){
        float Area(Vector2 v1, Vector2 v2, Vector2 v3) =>
            Mathf.Abs((v1.x * (v2.y - v3.y) + v2.x * (v3.y - v1.y) + v3.x * (v1.y - v2.y)) / 2f);

        float A = Area(a, b, c);
        float A1 = Area(p, b, c);
        float A2 = Area(a, p, c);
        float A3 = Area(a, b, p);

        return Mathf.Abs(A - (A1 + A2 + A3)) < 0.001f;
    }
}