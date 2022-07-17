using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketComponent : AttackType
{
    public RocketProjectile projectilePrefab;
    [SerializeField] Vector3 m_spawnOffset = Vector3.up;

    RocketProjectileSettings settings { get { return projectilePrefab.settings; } }

    protected override void Awake()
    {
        base.Awake();
    }

    public override void AttackEnter(Transform attackTarget, Vector3 attackDir)
    {
        // Ranged attack

        ShootRocketProjectile(agent.transform.position, attackDir);
        agent.anim.SetBool("isAttacking", false);
    }

    public override void AttackUpdate()
    {

    }

    public override void AttackExit()
    {
        
    }

    public override void AnimBeginAttackEvent()
    {
        
    }

    public override void AnimEndAttackEvent()
    {
        // This is not called for belchers
    }

    public void ShootRocketProjectile(Vector3 origin, Vector3 attackDir)
    {
        RocketProjectile newProjectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
        newProjectile.Shoot(agent, origin + m_spawnOffset, attackDir, settings.initialForce);
    }
}
