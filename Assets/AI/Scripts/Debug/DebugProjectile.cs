using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugProjectile : MonoBehaviour
{
    Rigidbody m_rigidBody;
    public float force = 5.0f;

    public DebugArcGizmo debugger;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        ResetDebugBall();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ResetDebugBall();
        }
        if(transform.position.y < -1.0f)
        {
            ResetDebugBall();
        }
    }

    void ResetDebugBall()
    {
        transform.position = Vector3.zero;
        Vector3 luanchVec = debugger.CalcLaunchVec();
        transform.LookAt(transform.position + luanchVec);
        m_rigidBody.velocity = transform.forward * luanchVec.magnitude;
    }
}
