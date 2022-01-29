using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredViewVolume : AViewVolume
{
    public string target;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == target)
        {
            SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == target)
        {
            SetActive(false);
        }
    }
}
