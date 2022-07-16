using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackType : MonoBehaviour
{
    AIAgent m_agent;
    protected AIAgent agent { get { return m_agent; } }

    protected virtual void Awake()
    {
        m_agent = GetComponent<AIAgent>();
    }

    public abstract void AttackEnter(Transform attackTarget, Vector3 attackDir);

    public abstract void AttackUpdate();

    public abstract void AttackExit();

    public abstract void AnimBeginAttackEvent();

    public abstract void AnimEndAttackEvent();
}