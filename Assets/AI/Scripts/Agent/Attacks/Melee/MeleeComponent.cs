using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeComponent : AttackType
{
    public HitBox armAttack;

    public override void AttackEnter(Transform attackTarget, Vector3 attackDir)
    {
        // Set to false just in case the last time this tried to attack and the animation didn't exit properly
        agent.anim.SetBool("hitBoxActive", false);
        armAttack.hitIsActive = false;

        agent.agentAudio.PlayClip(agent.agentAudio.attackClip);
    }

    public override void AttackUpdate()
    {
        
    }

    public override void AttackExit()
    {

    }

    public override void AnimBeginAttackEvent()
    {
        armAttack.hitIsActive = true;
    }

    public override void AnimEndAttackEvent()
    {
        agent.anim.SetBool("hitBoxActive", false);
        armAttack.hitIsActive = false;
    }
}
