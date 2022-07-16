using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] float m_radius = 0.2f;
    [SerializeField] LayerMask m_groundLayer = ~0;

    bool m_insideRadius = false;

    [SerializeField] UnityEvent m_enterEvent;
    [SerializeField] UnityEvent m_exitEvent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(m_insideRadius)
        {
            if(!CheckGround())
            {
                ExitRadius();
            }
        }
    }

    public void EnterRadius()
    {
        m_insideRadius = true;
        m_enterEvent.Invoke();
    }

    public void ExitRadius()
    {
        m_insideRadius = false;
        m_exitEvent.Invoke();
    }

    public bool CheckGround()
    {
        return Physics.CheckSphere(transform.position, m_radius, m_groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawSphere(transform.position, m_radius);
    }
}
