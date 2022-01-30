using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFollowView : AView {
    [Range(CameraConfiguration.PITCH_MIN, CameraConfiguration.PITCH_MAX)] public float[] pitch = new float[3];
    [Range(CameraConfiguration.ROLL_MIN, CameraConfiguration.ROLL_MAX)] public float[] roll = new float[3];
    [Range(CameraConfiguration.FOV_MIN, CameraConfiguration.FOV_MAX)] public float[] fov = new float[3];
    public Transform target;
    public float yawSpeed = 180f;
    public Curve curve;
    public float curveSpeed = 0.5f;

    [SerializeField] private float curvePosition = 0.5f;
    [SerializeField] private float yaw = 0f;

    public float Yaw { get { return yaw; } }

    public override CameraConfiguration GetConfiguration() {
        curvePosition = Mathf.Clamp01(curvePosition);
        Matrix4x4 curveToWorldMatrix = ComputeCurveToWorldMatrix();
        Vector3 position = curve.GetPosition(curvePosition, curveToWorldMatrix);
        transform.position = curve.GetPosition(curvePosition, curveToWorldMatrix);

        float length = pitch.Length;
        if (length > roll.Length)
            length = roll.Length;
        if (length > fov.Length)
            length = fov.Length;
        float percentage = curvePosition * 2f;
        Vector3 prf = Vector3.zero;
        //float p = 0;
        //float r = 0;
        //float f = 0;
        float step = 1f / (length / 3f);
        for (int i = 0; i < length; i++) {
            if (Mathf.Abs(percentage) < step) {
                Vector3 values = new Vector3(pitch[i], roll[i], fov[i]);
                float perc = (1 - Mathf.Abs(percentage / step));
                prf += values * perc;
                //p += perc * pitch[i];
                //r += perc * roll[i];
                //f += perc * fov[i];
            }
            percentage -= step;
        }

        return new CameraConfiguration()
            .SetYaw(yaw)
            .SetPitch(prf.x)
            .SetRoll(prf.y)
            .SetPivot(position)
            .SetFov(prf.z);
    }

    public override void Move(Vector2 direction) {
        yaw += direction.x * Time.deltaTime * yawSpeed;
        curvePosition += direction.y * Time.deltaTime * curveSpeed;
        curvePosition = Mathf.Clamp01(curvePosition);
    }

    private Matrix4x4 ComputeCurveToWorldMatrix() {
        Vector3 position = target != null ? target.position : transform.position;
        Quaternion rotation = Quaternion.Euler(0, yaw, 0);
        return Matrix4x4.TRS(position, rotation, Vector3.one);
    }

    private void OnDrawGizmosSelected() {
        curve.DrawGizmo(Color.red, ComputeCurveToWorldMatrix());
    }
}
