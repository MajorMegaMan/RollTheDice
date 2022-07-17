using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFollow : MonoBehaviour
{
    [SerializeField] Transform m_followtransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 position = transform.position;
        position.x = m_followtransform.position.x;
        position.z = m_followtransform.position.z;
        transform.position = position;
    }
}
