using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIAgent))]
public class DebugTargetBillboard : MonoBehaviour
{
    [SerializeField] Transform m_modelTransform;
    [SerializeField] Vector3 m_localOffset = Vector3.up;
    AIAgent m_agent;
    Transform m_playerTransform;

    Vector3 m_smoothLookTarget = Vector3.zero;
    Vector3 m_smoothLookVel = Vector3.zero;
    [SerializeField] float m_smoothTime = 0.1f;

    [SerializeField] AnimationCurve m_wiggle = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] float m_wiggleTime = 1.0f;
    float m_time = 0.0f;

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
        if(m_wiggleTime != 0.0f)
        {
            m_time += (Time.deltaTime / m_wiggleTime) * Random.value;
        }
        if(m_time > 1.0f)
        {
            m_time -= 1.0f;
        }
        Vector3 right = Vector3.right * m_wiggle.Evaluate(m_time);
        Vector3 wigglePos = right;
        m_modelTransform.localPosition = m_localOffset + wigglePos;

        m_smoothLookTarget = Vector3.SmoothDamp(m_smoothLookTarget, m_playerTransform.position, ref m_smoothLookVel, m_smoothTime);

        m_modelTransform.LookAt(m_smoothLookTarget);
    }
}
