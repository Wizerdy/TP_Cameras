using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AView : MonoBehaviour {
	[SerializeField] protected float weight = 1f;

	public float Weight {
		get { return weight; } set { weight = value; if (weight > 1f) weight = 1f; if (weight < 0f) weight = 0f; }
    }

	public abstract CameraConfiguration GetConfiguration();

	public void SetActive(bool isActive) {
		gameObject.SetActive(isActive);
		if (isActive) {
			CameraController.instance.AddView(this);
		} else {
			CameraController.instance.RemoveView(this);
		}
	}

	public virtual void Move(Vector2 direction) { }
}
