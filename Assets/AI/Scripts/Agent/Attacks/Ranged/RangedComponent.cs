using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedComponent : AttackType
{
    public Projectile projectilePrefab;
    public Transform projectileOrigin;
    Vector3 m_originOffset = Vector3.zero;

    Transform m_currentTarget;

    ProjectileSettings settings { get { return projectilePrefab.settings; } }

    protected override void Awake()
    {
        base.Awake();
        m_originOffset = projectileOrigin.GetComponent<SphereCollider>().center;
    }

    public override void AttackEnter(Transform attackTarget, Vector3 attackDir)
    {
        // Ranged attack
        m_currentTarget = attackTarget;
    }

    public override void AttackUpdate()
    {
        
    }

    public override void AttackExit()
    {
        m_currentTarget = null;
    }

    public override void AnimBeginAttackEvent()
    {
        Vector3 result = projectileOrigin.forward;
        result.x *= m_originOffset.x;
        result.y *= m_originOffset.y;
        result.x *= m_originOffset.z;

        ShootProjectile(projectileOrigin.position + FindHeadOffset());
    }

    void BelchAudioStart()
    {
        agent.agentAudio.PlayClip(agent.agentAudio.attackClip);
    }

    public override void AnimEndAttackEvent()
    {
        // This is not called for belchers
    }

    Vector3 FindHeadOffset()
    {
        Vector3 result = projectileOrigin.localToWorldMatrix * m_originOffset;
        return result;
    }

    public void ShootProjectile(Vector3 origin)
    {
        Projectile newProjectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
        Shoot(newProjectile, origin, m_currentTarget.position + settings.targetOffset);
    }

    public void Shoot(Projectile projectile, Vector3 origin, Vector3 destination)
    {
        Vector3 launchVector = CalcLaunchVec(origin, destination);
        float force = launchVector.magnitude;
        projectile.Shoot(agent, origin, launchVector, force);
    }

    public Vector3 CalcLaunchVec(Vector3 origin, Vector3 destination)
    {
        Vector3 dir = destination - origin;
        return CalcLaunchVec(dir, -Physics.gravity.y, settings.GetDynamicTime(dir.magnitude));
    }

    Vector3 CalcLaunchVec(Vector3 displacement, float gravity, float t)
    {
        return CalcLaunchVec(displacement.x, displacement.y, displacement.z, gravity, t);
    }

    Vector3 CalcLaunchVec(float x, float y, float z, float gravity, float t)
    {
        //float u = (gravity * (t * t) + 2 * y) / 2 * t;
        float u = y / t + gravity * t / 2;

        Vector3 result = new Vector3(x / t, u, z / t);
        return result;
    }

    private void OnDrawGizmos()
    {
        if(agent == null)
        {
            return;
        }
        Gizmos.color = Color.red;

        Vector3 start = projectileOrigin.position + FindHeadOffset();
        Vector3 end = agent.playerTransform.position + settings.targetOffset;

        Gizmos.DrawSphere(start, 0.2f);

        Vector3 launchVector = CalcLaunchVec(start, end);

        int curveCount = 300;

        float timeStep = (1.0f / curveCount) * settings.GetDynamicTime((end - start).magnitude);

        void DrawArc(Vector3 origin, Vector3 launchDir)
        {
            Vector3 grav = Physics.gravity * timeStep;
            Vector3 currentPos = origin;
            // target
            for (int i = 0; i < curveCount; i++)
            {
                Vector3 next = launchDir + grav * i;
                Gizmos.DrawLine(currentPos, currentPos + next * timeStep);
                currentPos += next * timeStep;
            }
        }
        Gizmos.color = Color.blue;

        DrawArc(start, launchVector);

    }
}
