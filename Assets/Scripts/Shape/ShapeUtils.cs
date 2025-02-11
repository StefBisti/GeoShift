using System;
using UnityEngine;

public static class ShapeUtils {
    public delegate Vector2[] GetShapeFunction(int n, float unitSize);
    public static GetShapeFunction[] GetShape = new GetShapeFunction[] { GetCircle, GetSquare, GetTriangle, GetStar };

    private static Vector2[] GetCircle(int n, float unitSize){
        if(n == 0) return new Vector2[0];

        Vector2[] res = new Vector2[n];
        int k = 0;

        float radius = unitSize / 2;
        float step = 2 * Mathf.PI / n;
        for(int i=0; i < n; i++){
            float angle = step * i + Mathf.PI / 2;
            res[k++] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }
        return res;
    }

    private static Vector2[] GetSquare(int n, float unitSize){
        if(n == 0) return new Vector2[0];

        float half = unitSize / 2.5f;
        Vector2 topRight = new Vector2(half, half);
        Vector2 topLeft = new Vector2(-half, half);
        Vector2 bottomLeft = new Vector2(-half, -half);
        Vector2 bottomRight = new Vector2(half, -half);
            
            
        Vector2[] res = new Vector2[n];
        int r = 0;

        float step = n / 4f;
        for(int k=0; k<4; k++){
            int start = Mathf.FloorToInt(step * k);
            int end = Mathf.FloorToInt(step * (k+1));
            for(int i=start; i < end; i++){
                float t = Mathf.InverseLerp(start, end, i);
                if(k == 0) res[r++] = Vector2.Lerp(topRight, topLeft, t);
                else if(k == 1) res[r++] = Vector2.Lerp(topLeft, bottomLeft, t);
                else if(k == 2) res[r++] = Vector2.Lerp(bottomLeft, bottomRight, t);
                else if(k == 3) res[r++] = Vector2.Lerp(bottomRight, topRight, t);
            }
        }

        Vector2[] result = new Vector2[res.Length];
        int nd = n / 8;
        Array.Copy(res, nd, result, 0, res.Length - nd);
        Array.Copy(res, 0, result, res.Length - nd, nd);
        return result;
    }

    private static Vector2[] GetTriangle(int n, float unitSize){
        if(n == 0) return new Vector2[0];

        float yOffset = 0;
        float sqr3 = Mathf.Sqrt(3f);
        Vector2 p1 = new Vector2(0, unitSize / sqr3 + yOffset);
        Vector2 p2 = new Vector2(-unitSize / 2, -unitSize / (2 * sqr3) + yOffset);
        Vector2 p3 = new Vector2(unitSize / 2, -unitSize / (2 * sqr3) + yOffset);

        Vector2[] res = new Vector2[n];
        int r = 0;

        float step = n / 3f;
        for(int k=0; k<3; k++){
            int start = Mathf.FloorToInt(step * k);
            int end = Mathf.FloorToInt(step * (k+1));
            for(int i=start; i < end; i++){
                float t = Mathf.InverseLerp(start, end, i);
                if(k == 0) res[r++] = Vector2.Lerp(p1, p2, t);
                else if(k == 1) res[r++] = Vector2.Lerp(p2, p3, t);
                else if(k == 2) res[r++] = Vector2.Lerp(p3, p1, t);
            }
        }
            
        return res;
    }

    private static Vector2[] GetStar(int n, float unitSize){
        if(n == 0) return new Vector2[0];

        Vector2[] innerPoints = new Vector2[5], outerPoints = new Vector2[5];
        float inRadius = unitSize / 4;
        float outRadius = unitSize / 2;
        float angleStep = 2 * Mathf.PI / 5f;
        for(int i=0; i < 5; i++){
            float angle = angleStep * i;
            float innerAngle = angle + 7 * Mathf.PI / 10;
            float outerAngle = angle + Mathf.PI / 2;
            innerPoints[i] = new Vector2(Mathf.Cos(innerAngle), Mathf.Sin(innerAngle)) * inRadius;
            outerPoints[i] = new Vector2(Mathf.Cos(outerAngle), Mathf.Sin(outerAngle)) * outRadius;
        }

        Vector2[] res = new Vector2[n];
        int r = 0;

        float step = n / 10f;
        Vector2 startPoint = outerPoints[0];
        Vector2 endPoint = innerPoints[0];
        int pIndex = 0;

        for(int k=0; k<10; k++){
            int start = Mathf.FloorToInt(step * k);
            int end = Mathf.FloorToInt(step * (k+1));
            for(int i=start; i < end; i++){
                float t = Mathf.InverseLerp(start, end, i);
                res[r++] = Vector2.Lerp(startPoint, endPoint, t);
            }
            pIndex++;
            startPoint = endPoint;
            endPoint = (pIndex % 2 == 0 ? innerPoints : outerPoints)[(pIndex + 1) / 2 % 5];
        }

        return res;
    }
}
