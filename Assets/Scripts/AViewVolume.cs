using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AViewVolume : MonoBehaviour {
    public int Priority = 0;
    public AView view;
    public bool isCutOnSwitch = false;

    public virtual float ComputeSelfWeight() {
        return 1.0f;
    }

    protected bool IsActive { get; private set; }

    protected void SetActive(bool isActive) {
        if (CameraController.instance == null) { return; }

        if (isCutOnSwitch) {
            ViewVolumeBlender.Instance.Update();
            CameraController.instance.Cut(view.GetConfiguration());
        }

        IsActive = isActive;
        if (IsActive) {
            ViewVolumeBlender.Instance.AddVolume(this);
        } else {
            ViewVolumeBlender.Instance.RemoveVolume(this);
        }
        view.SetActive(isActive);
    }
}
