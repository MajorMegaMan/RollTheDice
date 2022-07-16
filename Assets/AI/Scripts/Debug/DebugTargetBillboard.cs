using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIAgent))]
public class DebugTargetBillboard : MonoBehaviour
{
    [SerializeField] Transform m_modelTransform;
    AIAgent m_agent;
    Transform m_playerTransform;

    Vector3 m_smoothLookTarget = Vector3.zero;
    Vector3 m_smoothLookVel = Vector3.zero;
    [SerializeField] float m_smoothTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<AIAgent>();
        m_playerTransform = m_agent.aiManager.playerTransform;
        m_smoothLookTarget = m_playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        m_smoothLookTarget = Vector3.SmoothDamp(m_smoothLookTarget, m_playerTransform.position, ref m_smoothLookVel, m_smoothTime);

        m_modelTransform.LookAt(m_smoothLookTarget);
    }
}
