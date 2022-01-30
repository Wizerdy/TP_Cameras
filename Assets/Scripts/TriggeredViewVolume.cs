using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredViewVolume : AViewVolume
{
    public string target;
    public bool onExit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == target)
        {
            SetActive(!onExit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == target)
        {
            SetActive(onExit);
        }
    }
}
