using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedFollowView : AView {
    [Range(CameraConfiguration.ROLL_MIN, CameraConfiguration.ROLL_MAX)] public float roll;
    [Range(CameraConfiguration.FOV_MIN, CameraConfiguration.FOV_MAX)] public float fov = 60f;
    public float distance;
    public Transform target;
    public Transform centralPoint = null;
    public float yawOffsetMax = 90f, pitchOffsetMax = 90f;

    public override CameraConfiguration GetConfiguration() {
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 centralDirection = (centralPoint.position - transform.position).normalized;
        float yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float pitch = -Mathf.Asin(direction.y) * Mathf.Rad2Deg;
        float centralYaw = Mathf.Atan2(centralDirection.x, centralDirection.z) * Mathf.Rad2Deg;
        float centralPitch = -Mathf.Asin(centralDirection.y) * Mathf.Rad2Deg;
        //yaw = Mathf.Clamp(yaw, centralYaw - yawOffsetMax, centralYaw + yawOffsetMax);
        float yawDelta = yaw - centralYaw;
        if (yawDelta > 180f) { yawDelta -= 360f; }
        if (yawDelta > yawOffsetMax || yawDelta < -yawOffsetMax) {
            if (yawDelta > 0f) { yaw = centralYaw + yawOffsetMax; }
            else { yaw = centralYaw - yawOffsetMax; }
        }
        pitch = Mathf.Clamp(pitch, centralPitch - pitchOffsetMax, centralPitch + pitchOffsetMax);
        return new CameraConfiguration()
            .SetYaw(yaw)
            .SetPitch(pitch)
            .SetRoll(roll)
            .SetPivot(transform.position)
            .SetDistance(distance)
            .SetFov(fov);
    }
}
