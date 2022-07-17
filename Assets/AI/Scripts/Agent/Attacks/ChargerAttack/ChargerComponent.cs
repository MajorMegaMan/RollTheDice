using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerComponent : AttackType
{
    public HitBox bodyHitBox;

    public float movingSpeed = 10.0f;
    public float attackTime = 1.0f;
    float m_timer = 0.0f;

    public override void AttackEnter(Transform attackTarget, Vector3 attackDir)
    {
        // Set to false just in case the last time this tried to attack and the animation didn't exit properly
        agent.anim.SetBool("hitBoxActive", true);
        bodyHitBox.hitIsActive = true;

        agent.agentAudio.PlayClip(agent.agentAudio.attackClip);

        m_timer = 0.0f;
    }

    public override void AttackUpdate()
    {
        agent.navAgent.Move(agent.transform.forward * movingSpeed * Time.deltaTime);
        m_timer += Time.deltaTime;
        if(m_timer > attackTime)
        {
            agent.anim.SetBool("isAttacking", false);
            Debug.Log("Attacking");
        }
    }

    public override void AttackExit()
    {
        agent.anim.SetBool("hitBoxActive", false);
        bodyHitBox.hitIsActive = false;
    }

    public override void AnimBeginAttackEvent()
    {
        bodyHitBox.hitIsActive = true;
    }

    public override void AnimEndAttackEvent()
    {
        agent.anim.SetBool("hitBoxActive", false);
        bodyHitBox.hitIsActive = false;
    }
}
