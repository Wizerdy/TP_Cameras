using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis { X, Y, Z, W }

public static class Tools {
    public static Vector2 Vectorize(this float angle) {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    #region Vectors

    public static Vector2 To2D(this Vector3 vector, Axis axisToIgnore = Axis.Z) {
        switch (axisToIgnore) {
            case Axis.X:
                return new Vector2(vector.y, vector.z);
            case Axis.Y:
                return new Vector2(vector.x, vector.z);
            case Axis.Z:
                return new Vector2(vector.x, vector.y);
            default:
                return new Vector2(vector.x, vector.y);
        }
    }

    public static Vector2Int To2D(this Vector3Int vector, Axis axisToIgnore = Axis.Z) {
        switch (axisToIgnore) {
            case Axis.X:
                return new Vector2Int(vector.y, vector.z);
            case Axis.Y:
                return new Vector2Int(vector.x, vector.z);
            case Axis.Z:
                return new Vector2Int(vector.x, vector.y);
            default:
                return new Vector2Int(vector.x, vector.y);
        }
    }

    public static Vector3 To3D(this Vector2 vector, float value = 0f, Axis axis = Axis.Z) {
        switch (axis) {
            case Axis.X:
                return new Vector3(value, vector.x, vector.y);
            case Axis.Y:
                return new Vector3(vector.x, value, vector.y);
            case Axis.Z:
                return new Vector3(vector.x, vector.y, value);
            default:
                return new Vector3(vector.x, vector.y, value);
        }
    }

    public static Vector3Int To3D(this Vector2Int vector, int value = 0, Axis axis = Axis.Z) {
        switch (axis) {
            case Axis.X:
                return new Vector3Int(value, vector.x, vector.y);
            case Axis.Y:
                return new Vector3Int(vector.x, value, vector.y);
            case Axis.Z:
                return new Vector3Int(vector.x, vector.y, value);
            default:
                return new Vector3Int(vector.x, vector.y, value);
        }
    }

    public static Vector3 To3D(this Vector4 vector, Axis axisToIgnore = Axis.W) {
        switch (axisToIgnore) {
            case Axis.X:
                return new Vector3(vector.y, vector.z, vector.w);
            case Axis.Y:
                return new Vector3(vector.x, vector.z, vector.w);
            case Axis.Z:
                return new Vector3(vector.x, vector.y, vector.w);
            case Axis.W:
                return new Vector3(vector.x, vector.y, vector.z);
            default:
                return new Vector3(vector.x, vector.y, vector.z);
        }
    }

    public static Vector3 Override(this Vector3 vector, float value, Axis axis = Axis.Y) {
        switch (axis) {
            case Axis.X:
                vector.x = value;
                break;
            case Axis.Y:
                vector.y = value;
                break;
            case Axis.Z:
                vector.z = value;
                break;
            default:
                vector.y = value;
                break;
        }

        return vector;
    }

    public static Vector2 Override(this Vector2 vector, float value, Axis axis = Axis.Y) {
        switch (axis) {
            case Axis.X:
                vector.x = value;
                break;
            case Axis.Y:
                vector.y = value;
                break;
            case Axis.Z:
                Debug.LogWarning("Can't override Vector2 z axis, using default axis : y");
                vector.y = value;
                break;
            default:
                vector.y = value;
                break;
        }

        return vector;
    }

    #endregion
}

public static class MathUtils {
    public static Vector3 GetNearestPointOnSegment(Vector3 a, Vector3 b, Vector3 target) {
        Vector3 ac = target - a;
        Vector3 ab = b - a;
        Vector3 n = ab.normalized;
        float dot = Vector3.Dot(ac, n);
        float abmag = ab.magnitude;
        dot = Mathf.Clamp(dot, 0, abmag);
        Vector3 proj = a + n * dot;
        return proj;
    }

    public static Vector3 LinearBezier(Vector3 a, Vector3 b, float t) {
        t = Mathf.Clamp01(t);
        return (1 - t) * a + t * b;
    }

    public static Vector3 QuadraticBezier(Vector3 a, Vector3 b, Vector3 c, float t) {
        t = Mathf.Clamp01(t);
        return (1 - t) * LinearBezier(a, b, t) + t * LinearBezier(b, c, t);
    }

    public static Vector3 QuadraticBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t) {
        t = Mathf.Clamp01(t);
        return (1 - t) * QuadraticBezier(a, b, c, t) + t * QuadraticBezier(b, c, d, t);
    }
}

[System.Serializable]
public class Curve {
    public Vector3 a, b, c, d;

    public Vector3 GetPosition(float t) {
        return MathUtils.QuadraticBezier(a, b, c, d, t);
    }

    public Vector3 GetPosition(float t, Matrix4x4 localToWorldMatrix) {
        return localToWorldMatrix.MultiplyPoint(MathUtils.QuadraticBezier(a, b, c, d, t));
    }

    public void DrawGizmo(Color color, Matrix4x4 localToWorldMatrix, int accuracy = 20) {
        Gizmos.color = color;
        Gizmos.DrawSphere(localToWorldMatrix.MultiplyPoint(a), 0.25f);
        Gizmos.DrawSphere(localToWorldMatrix.MultiplyPoint(b), 0.25f);
        Gizmos.DrawSphere(localToWorldMatrix.MultiplyPoint(c), 0.25f);
        Gizmos.DrawSphere(localToWorldMatrix.MultiplyPoint(d), 0.25f);
        if (accuracy <= 1) { return; }
        for (int i = 0; i < accuracy; i++) {
            Vector3 startPos = GetPosition((1f / accuracy) * (float)i, localToWorldMatrix);
            Vector3 endPos = GetPosition((1f / accuracy) * (float)(i + 1), localToWorldMatrix);
            Gizmos.DrawLine(startPos, endPos);
        }
    }
}