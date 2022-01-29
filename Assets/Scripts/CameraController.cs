using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraController : MonoBehaviour {
    public static CameraController instance;

    public new Camera camera;
    public bool isCutRequested;

    [SerializeField] private List<AView> activeViews = new List<AView>();

    public float speed = 1f;

    private CameraConfiguration currentConfiguration = new CameraConfiguration();
    private CameraConfiguration targetConfiguration = new CameraConfiguration();

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        currentConfiguration = new CameraConfiguration();
        targetConfiguration = new CameraConfiguration();
    }

    private void Update() {
        ViewVolumeBlender.Update();

        if (targetConfiguration == null || currentConfiguration == null) { return; }

        ApplyConfiguration();

        if (isCutRequested) {
            isCutRequested = false;
            ApplyConfiguration(targetConfiguration);
        } else {
            SmoothTransition();
        }
    }

    public void ApplyConfiguration(Camera camera, CameraConfiguration configuration) {
        camera.transform.rotation = configuration.Rotation;
        camera.transform.position = configuration.Position;
        camera.fieldOfView = configuration.fov;
        currentConfiguration = configuration;
    }

    public void ApplyConfiguration(CameraConfiguration configuration) {
        ApplyConfiguration(camera, configuration);
    }

    private void ApplyConfiguration() {
        if (activeViews == null || activeViews.Count <= 0) { return; }

        CameraConfiguration[] configurations = activeViews.Select(v => v.GetConfiguration()).ToArray();
        float[] weight = activeViews.Select(v => v.Weight).ToArray();
        CameraConfiguration average = ComputeAverageConfiguration(configurations, weight);
        targetConfiguration = average;
    }

    public void SmoothTransition() {
        float delta = speed * Time.deltaTime;
        if (delta < 1f) {
            float yaw = CameraConfiguration.AverageYaw(currentConfiguration.yaw, targetConfiguration.yaw, delta);
            currentConfiguration += (targetConfiguration - currentConfiguration) * delta;
            currentConfiguration.yaw = yaw;
        } else {
            currentConfiguration = targetConfiguration;
        }
        ApplyConfiguration(currentConfiguration);
    }

    public CameraConfiguration ComputeAverageConfiguration(CameraConfiguration[] configurations, float[] weight) {
        CameraConfiguration output = new CameraConfiguration();

        float totWeight = 0f;

        Vector2 average = ComputeAverageAngleAsVector(configurations.Select(x => x.yaw).ToArray(), weight);
        output.yaw = Vector2.SignedAngle(Vector3.right, average);

        for (int i = 0; i < configurations.Length; i++) {
            totWeight += weight[i];
            output.pitch += configurations[i].pitch * weight[i];
            output.roll += configurations[i].roll * weight[i];
            output.distance += configurations[i].distance * weight[i];
            output.pivot += configurations[i].pivot * weight[i];
            output.fov += configurations[i].fov * weight[i];
        }

        if (totWeight == 0) { return null; }

        output.pitch /= totWeight;
        output.roll /= totWeight;
        output.distance /= totWeight;
        output.pivot /= totWeight;
        output.fov /= totWeight;

        return output;
    }

    public static Vector2 ComputeAverageAngleAsVector(float[] angles, float[] weight) {
        Vector2 sum = Vector2.zero;
        for (int i = 0; i < angles.Length; i++) {
            sum += new Vector2(Mathf.Cos(angles[i] * Mathf.Deg2Rad), Mathf.Sin(angles[i] * Mathf.Deg2Rad)) * weight[i];
        }
        return sum;
    }

    public void AddView(AView view) {
        if (activeViews.Contains(view)) { return; }
        activeViews.Add(view);
    }

    public void RemoveView(AView view) {
        if (activeViews.Contains(view)) {
            activeViews.Remove(view);
        }
    }

    public void OnDrawGizmos() {
        if (activeViews == null || activeViews.Count <= 0) { return; }

        for (int i = 0; i < activeViews.Count; i++) {
            activeViews[i].GetConfiguration().DrawGizmos(Color.red);
        }
    }

    public void Cut(CameraConfiguration newConf) {
        isCutRequested = true;
        targetConfiguration = newConf;
    }

}