using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Rail : MonoBehaviour {
    public bool isLoop = false;
    private List<Transform> rail;
    private float length;

    private void Awake() {
        rail = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++) {
            rail.Add(transform.GetChild(i));
        }
    }

    private void Start() {
        length = 0;
        for (int i = 0; i < rail.Count - 1; i++) {
            length += Vector3.Distance(rail[i].position, rail[i + 1].position);
        }
        if (isLoop) {
            length += Vector3.Distance(rail[^1].position, rail[0].position);
        }
    }

    private void Update() {
        if (!Application.isPlaying) {
            rail = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++) {
                rail.Add(transform.GetChild(i));
            }
        }
    }

    public float GetLength() {
        return length;
    }

    public Vector3 GetPosition(float distance) {
        if (distance == 0) { return rail[0].position; }
        if (distance > length && !isLoop) { return rail[^1].position; }
        if (distance < 0) { distance = (distance % length) + length; }

        float distanceReached = 0;
        int startIndex = 0;
        int endIndex = startIndex + 1;
        float lastDistanceReached = 0;
        while (distanceReached < distance) {
            lastDistanceReached = distanceReached;
            distanceReached += Vector3.Distance(rail[startIndex].position, rail[endIndex].position);
            if (distanceReached >= distance) {
                continue;
            }
            startIndex++;
            startIndex %= rail.Count;
            endIndex++;
            endIndex %= rail.Count;
        }
        Vector3 pos1 = rail[startIndex].position;
        Vector3 pos2 = rail[endIndex].position;
        Vector3 output = Vector3.Lerp(pos1, pos2, (distance - lastDistanceReached) / (distanceReached - lastDistanceReached));
        return output;
    }

    public Vector3 GetPosition(Vector3 target) {
        Vector3 projection = Vector3.zero;
        float distance = Mathf.Infinity;
        for (int i = 0; i < rail.Count - 1; i++) {
            Vector3 proj = MathUtils.GetNearestPointOnSegment(rail[i].position, rail[i+1].position, target);
            float dist = Vector3.Distance(proj, target);
            if (distance > dist) {
                projection = proj;
                distance = dist;
            }
        }
        if (isLoop) {
            Vector3 proj = MathUtils.GetNearestPointOnSegment(rail[^1].position, rail[0].position, target);
            float dist = Vector3.Distance(proj, target);
            if (distance > dist) {
                projection = proj;
                distance = dist;
            }
        }

        return projection;
    }

    public void OnDrawGizmos() {
        if (rail == null || rail.Count <= 0) { return; }

        Gizmos.color = Color.green;
        for (int i = 0; i < rail.Count; i++) {
            Vector3 position = rail[i].position;
            System.Nullable<Vector3> nextposition = null;
            if (i < rail.Count - 1) {
                nextposition = rail[i + 1].position;
            } else if (isLoop) {
                nextposition = rail[0].position;
            }
            Gizmos.DrawSphere(position, 0.25f);
            if (nextposition != null) {
                Gizmos.DrawLine(position, nextposition.Value);
            }
        }
    }
}
