using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed = 10.0f;

    public FreeFollowView ffView = null;

    Rigidbody _rigidbody = null;

    protected bool IsActive { get; private set; }

    public void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0)) {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Cursor.lockState == CursorLockMode.Locked) {
            Vector2 direction = Vector2.zero;
            direction += new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
            direction.Normalize();
            //ffView.Move(direction);
            CameraController.instance.MoveActiveViews(direction);
            if (ffView != null) {
                transform.rotation = Quaternion.Euler(0f, ffView.Yaw, 0f);
            }
        }
    }

    void FixedUpdate() {
        Vector3 direction = Vector3.zero;
        direction += Input.GetAxisRaw("Horizontal") * transform.right;
        direction += Input.GetAxisRaw("Vertical") * transform.forward;
        direction.Normalize();
        _rigidbody.velocity = direction * speed + Vector3.up * _rigidbody.velocity.y;
    }
}
