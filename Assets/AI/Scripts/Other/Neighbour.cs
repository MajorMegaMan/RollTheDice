using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Neighbour
{
    static Neighbour _empty;
    public static Neighbour empty { get { return _empty; } }

    AIAgent m_fromAgent;
    AIAgent m_toAgent;
    float m_distSqrd;
    Vector3 m_offset;

    public AIAgent fromAgent { get { return m_fromAgent; } }
    public AIAgent toAgent { get { return m_toAgent; } }
    public float distSqrd { get { return m_distSqrd; } }
    public Vector3 offset { get { return m_offset; } }

    static Neighbour()
    {
        _empty = new Neighbour();
    }

    public void FindNeighbour(AIAgent fromAgent, AIAgent toAgent)
    {
        this.m_fromAgent = fromAgent;
        this.m_toAgent = toAgent;

        m_offset = toAgent.transform.position - fromAgent.transform.position;
        m_distSqrd = m_offset.sqrMagnitude;
    }

    public static Neighbour operator-(Neighbour target)
    {
        Neighbour result = empty;

        result.FindNeighbour(target.m_toAgent, target.m_fromAgent);

        return result;
    }
}
