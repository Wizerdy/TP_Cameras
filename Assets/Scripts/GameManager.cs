using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public List<AView> views;
    public float speed = 0.1f;
    public DollyView dollyView;
    public FreeFollowView ffView;

    public bool lerp2Views = false;

    private int polar = 1;

    void Start() {
        for (int i = 0; i < views.Count; i++) {
            //CameraController.instance.AddView(views[i]);
            views[i].SetActive(true);
        }
        //CameraController.instance.currentConfiguration = views[0].GetConfiguration();
        //CameraController.instance.targetConfiguration = views[1].GetConfiguration();
        if (lerp2Views) {
            views[0].Weight = 1;
            views[1].Weight = 0;
        }
    }

    void Update() {
        if (lerp2Views) {
            views[0].Weight -= Time.deltaTime * speed * polar;
            views[1].Weight += Time.deltaTime * speed * polar;

            if (views[1].Weight > 0.9999f) { polar = -1; }
            if (views[0].Weight > 0.9999f) { polar = 1; }
        }

        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (dollyView != null) {
            dollyView.Move(direction.x);
        }

        if (ffView != null) {
            ffView.Move(direction);
        }
    }
}
