using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDrawLine : MonoBehaviour
{
    public Transform target;
    public Color colour = Color.red;

    public LayerMask layerMask = ~0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = colour;
        Vector3 dir = target.position - transform.position;
        Vector3 point = target.position;
        if (Physics.Raycast(transform.position, dir, out RaycastHit hitInfo, dir.magnitude, layerMask, QueryTriggerInteraction.Ignore))
        {
            point = hitInfo.point;
        }

        Gizmos.DrawLine(transform.position, point);
    }
}
