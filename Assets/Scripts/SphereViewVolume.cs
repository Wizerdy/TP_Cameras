using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereViewVolume : AViewVolume
{
    public GameObject target;
    public float outerRadius;
    public float innerRadius;
    public Color gizmosColor = Color.yellow;

    private float distance;

    private void Awake()
    {
        if (innerRadius > outerRadius)
        {
            innerRadius = outerRadius;
        }
    }

    private void Update()
    {
        distance = Vector3.Distance(target.transform.position, transform.position);

        if (distance <= outerRadius && !IsActive)
        {
            SetActive(true);
        }else if (distance > outerRadius && IsActive)
        {
            SetActive(false);
        }
    }

    public override float ComputeSelfWeight()
    {
        float delta = 1 - ((distance - innerRadius) / (outerRadius - innerRadius));
        return Mathf.Lerp(0f, 1f, delta);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmosColor;
        Gizmos.DrawSphere(transform.position, innerRadius);
        Gizmos.DrawSphere(transform.position, outerRadius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, innerRadius);
        Gizmos.DrawWireSphere(transform.position, outerRadius);
    }
}
