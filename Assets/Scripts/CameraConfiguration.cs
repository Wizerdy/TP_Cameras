using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraConfiguration {
	public const float PITCH_MIN = -90f, PITCH_MAX = 90f, ROLL_MIN = -180f, ROLL_MAX = 180f, FOV_MIN = 15f, FOV_MAX = 180f;

	public float yaw = 0f, pitch = 0f, roll = 0f;
	public Vector3 pivot = Vector3.zero;
	public float distance = 0f;
	public float fov = 0f;

    #region Properties
    public Quaternion Rotation { get { return Quaternion.Euler(pitch, yaw, roll); } }
	public Vector3 Position { get { return Rotation * (Vector3.back * distance) + pivot; } }
    #endregion

    public Quaternion GetRotation() { return Rotation; }
	public Vector3 GetPosition() { return Position; }

	#region Builder
	public CameraConfiguration SetYaw(float yaw) { this.yaw = yaw; return this; }
	public CameraConfiguration SetPitch(float pitch) { this.pitch = pitch; return this; }
	public CameraConfiguration SetRoll(float roll) { this.roll = roll; return this; }
	public CameraConfiguration SetFov(float fov) { this.fov = fov; return this; }
	public CameraConfiguration SetPivot(Vector3 pivot) { this.pivot = pivot; return this; }
	public CameraConfiguration SetDistance(float distance) { this.distance = distance; return this; }
    #endregion

    public override string ToString() {
		string toString = "Yaw : " + yaw + 
			", Pitch : " + pitch + 
			", Roll : " + roll + 
			", Pivot : " + pivot + 
			", distance : " + distance + 
			", fov : " + fov;
        return toString;
    }

    #region Operators
    public static CameraConfiguration operator +(CameraConfiguration c1, CameraConfiguration c2) =>
		new CameraConfiguration()
			//.SetYaw(Vector2.SignedAngle(Vector2.right, c1.yaw.Vectorize() + c2.yaw.Vectorize()))
			.SetYaw(c1.yaw + c2.yaw)
			.SetPitch(c1.pitch + c2.pitch)
			.SetRoll(c1.roll + c2.roll)
			.SetPivot(c1.pivot + c2.pivot)
			.SetDistance(c1.distance + c2.distance)
			.SetFov(c1.fov + c2.fov);

	public static CameraConfiguration operator -(CameraConfiguration c1, CameraConfiguration c2) =>
		new CameraConfiguration()
			//.SetYaw(Vector2.SignedAngle(Vector2.right, c2.yaw.Vectorize() - c1.yaw.Vectorize()))
			.SetYaw(c1.yaw - c2.yaw)
			.SetPitch(c1.pitch - c2.pitch)
			.SetRoll(c1.roll - c2.roll)
			.SetPivot(c1.pivot - c2.pivot)
			.SetDistance(c1.distance - c2.distance)
			.SetFov(c1.fov - c2.fov);

	public static CameraConfiguration operator *(CameraConfiguration c1, float f) =>
		new CameraConfiguration()
			.SetYaw(c1.yaw * f)
			.SetPitch(c1.pitch * f)
			.SetRoll(c1.roll * f)
			.SetPivot(c1.pivot * f)
			.SetDistance(c1.distance * f)
			.SetFov(c1.fov * f);
    #endregion

	public static float AverageYaw(float yaw1, float yaw2, float t) {
		return Vector2.SignedAngle(Vector2.right, yaw1.Vectorize() * (1 - t) + yaw2.Vectorize() * t);
    }

    public void DrawGizmos(Color color) {
		Gizmos.color = color;
		Gizmos.DrawSphere(pivot, 0.25f);
		Vector3 position = GetPosition();
		Gizmos.DrawLine(pivot, position);
		Gizmos.matrix = Matrix4x4.TRS(position, GetRotation(), Vector3.one);
		Gizmos.DrawFrustum(Vector3.zero, fov, 0.5f, 0f, Camera.main.aspect);
		Gizmos.matrix = Matrix4x4.identity;
	}
}