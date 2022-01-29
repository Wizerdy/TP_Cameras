using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedView : AView {
	public float yaw, pitch, roll, fov = 60f;

	public override CameraConfiguration GetConfiguration() {
		CameraConfiguration output = new CameraConfiguration()
			.SetYaw(yaw)
			.SetPitch(pitch)
			.SetRoll(roll)
			.SetFov(fov)
			.SetPivot(transform.position)
			.SetDistance(0f);
		return output;
	}
}
