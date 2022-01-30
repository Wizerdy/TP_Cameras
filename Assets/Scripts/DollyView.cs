using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyView : AView {
    public float roll, distance, fov = 60f;
    public Transform target;
    public bool isAuto = false;
    public Rail rail;
    public float distanceOnRail;
    public float speed = 1f;

    public override CameraConfiguration GetConfiguration() {
        Vector3 position = isAuto ? rail.GetPosition(target.position) : rail.GetPosition(distanceOnRail);
        transform.position = position;
        Vector3 direction = (target.position - position).normalized;
        float yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float pitch = -Mathf.Asin(direction.y) * Mathf.Rad2Deg;
        return new CameraConfiguration()
            .SetYaw(yaw)
            .SetPitch(pitch)
            .SetRoll(roll)
            .SetPivot(position)
            .SetDistance(distance)
            .SetFov(fov);
    }

    public override void Move(Vector2 direction) {
        distanceOnRail += speed * Time.deltaTime * direction.x;
    }
}
