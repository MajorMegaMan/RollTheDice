using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIStates
{
    public enum StateIndex
    {
        idle,
        wander,
        chasePlayer,
        attackPlayer,
        dead,
        beginVault,
        endVault,
        falling,

        // Debug States
        debugAlwaysAttack
    }

    public static class StateBucket
    {
        public static void SetUpStateMachine(StateMachine<AIAgent> target)
        {
            target.AddState(new Idle());
            target.AddState(new Patrol());
            target.AddState(new ChasePlayer());
            target.AddState(new AttackPlayer());
            target.AddState(new Dead());
            target.AddState(new BeginVault());
            target.AddState(new EndVault());
            target.AddState(new Falling());

            // Debug States
            target.AddState(new AlwaysAttackPlayer());
        }
    }

    public abstract class AgentState : IState<AIAgent>
    {
        void IState<AIAgent>.Enter(AIAgent agent)
        {
            Enter(agent);
        }

        void IState<AIAgent>.Update(AIAgent agent)
        {
            Update(agent);
        }

        void IState<AIAgent>.Exit(AIAgent agent)
        {
            Exit(agent);
        }

        public abstract void Enter(AIAgent agent);
        public abstract void Update(AIAgent agent);
        public abstract void Exit(AIAgent agent);
    }

    public abstract class MovementState : IState<AIAgent>
    {
        void IState<AIAgent>.Enter(AIAgent agent)
        {
            agent.agentAudio.PlayClip(agent.agentAudio.idleClip);
            Enter(agent);
        }

        void IState<AIAgent>.Update(AIAgent agent)
        {
            if (agent.navAgent.isOnOffMeshLink)
            {
                if(agent.navAgent.currentOffMeshLinkData.offMeshLink.area == 3)
                {
                    // begin vault
                    agent.ChangeState(StateIndex.beginVault);
                    return;
                }

                // Debug Traversal
                agent.ChangeState(StateIndex.beginVault);
            }

            Update(agent);
        }

        void IState<AIAgent>.Exit(AIAgent agent)
        {
            Exit(agent);
        }

        public abstract void Enter(AIAgent agent);
        public abstract void Update(AIAgent agent);
        public abstract void Exit(AIAgent agent);
    }

    public class Idle : AgentState
    {
        float m_timer = 0.0f;
        float m_currentTargetTime = 0.0f;

        public override void Enter(AIAgent agent)
        {
            // set up idle values
            m_timer = 0.0f;
            m_currentTargetTime = Random.Range(agent.settings.minIdleTime, agent.settings.maxIdleTime);

            agent.StopNavigating();
        }

        public override void Update(AIAgent agent)
        {
            // Check for player Radius
            if(agent.settings.aggresionRadius * agent.settings.aggresionRadius > agent.distToPlayerSquared)
            {
                agent.ChasePlayer();
                return;
            }

            // Check Idle behaviour
            if(agent.patrolNodes.Count == 0)
            {
                agent.ChangeState(StateIndex.idle);
                return;
            }

            if(m_timer > m_currentTargetTime)
            {
                // bam, no more idle
                agent.ChangeState(StateIndex.wander);
            }

            // Update idle timers
            m_timer += Time.deltaTime;
        }

        public override void Exit(AIAgent agent)
        {
            // clean up idle Values
        }
    }

    public class Patrol : MovementState
    {
        Vector3 m_patrolTarget = Vector3.zero;

        public override void Enter(AIAgent agent)
        {
            // set up state values
            m_patrolTarget = agent.patrolNodes[agent.currentPatrolIndex].position;

            agent.StartNavigating();
            agent.navAgent.SetDestination(m_patrolTarget);
        }

        public override void Update(AIAgent agent)
        {
            // Check for player Radius
            if (agent.settings.aggresionRadius * agent.settings.aggresionRadius > agent.distToPlayerSquared)
            {
                agent.ChasePlayer();
                return;
            }

            // Check state behaviour
            if (agent.navAgent.pathPending)
            {
                return;
            }

            if(agent.navAgent.remainingDistance <= agent.settings.stoppingDistance)
            {
                // arrived at wander node
                agent.currentPatrolIndex++;

                if(agent.currentPatrolIndex >= agent.patrolNodes.Count)
                {
                    agent.currentPatrolIndex = 0;
                }

                agent.ChangeState(StateIndex.idle);
            }
        }

        public override void Exit(AIAgent agent)
        {
            // clean up state Values
        }
    }

    public class ChasePlayer : MovementState
    {
        Transform m_playerTransform;

        public override void Enter(AIAgent agent)
        {
            // set up state values
            m_playerTransform = agent.playerTransform;

            agent.StartNavigating();
            agent.navAgent.SetDestination(m_playerTransform.position);
        }

        public override void Update(AIAgent agent)
        {
            // Don't do anything meaningful if the agent is hurting
            if(agent.isHurting)
            {
                agent.safeLeaveIsHurtTimer += Time.deltaTime;
                if(agent.safeLeaveIsHurtTimer > 0.1f)
                {
                    agent.isHurting = false;
                }
                else
                {
                    return;
                }
            }

            Vector3 toPlayer = (m_playerTransform.position - agent.transform.position).normalized;
            toPlayer *= agent.settings.orbWalkRadius;


            // Check state behaviour
            agent.navAgent.SetDestination(m_playerTransform.position - toPlayer);

            if(agent.distToPlayerSquared < agent.settings.attackChargeRadius * agent.settings.attackChargeRadius)
            {
                // in range to charge attack
                agent.attackCharge += Time.deltaTime * agent.settings.attackChargeRate;

                if(agent.attackCharge > agent.settings.attackChargeMax)
                {
                    agent.attackCharge -= agent.settings.attackChargeMax;
                    // The agent has successfully begun it's attack
                    agent.ChangeState(StateIndex.attackPlayer);
                    return;
                }
            }
        }

        public override void Exit(AIAgent agent)
        {
            // clean up state Values
            //agent.navAgent.SetDestination(agent.transform.position);
        }
    }

    public class AttackPlayer : AgentState
    {
        Transform m_playerTransform;
        Vector3 m_originalLocation;
        Vector3 m_attackDirection;

        public override void Enter(AIAgent agent)
        {
            // set up state values
            m_playerTransform = agent.playerTransform;
            m_originalLocation = agent.transform.position;
            m_attackDirection = (agent.playerTransform.position - agent.transform.position);
            m_attackDirection.y = 0;
            m_attackDirection = m_attackDirection.normalized;

            // Set up animation
            agent.StopNavigating();

            agent.anim.SetTrigger("attack");
            agent.anim.SetBool("isAttacking", true);
            agent.anim.SetBool("lockRotation", true);

            // Set up attack type
            agent.attack.AttackEnter(m_playerTransform, m_attackDirection);
        }

        public override void Update(AIAgent agent)
        {
            // this would be the logic where the attack could take place and wait until it has ended.
            if(agent.anim.GetBool("lockRotation"))
            {
                // only aim towards the initial attack direction
                agent.transform.forward = Vector3.Slerp(agent.transform.forward, m_attackDirection, agent.settings.rotationLerpSpeed);
            }
            else
            {
                // start looking towards the player
                Vector3 toPlayer = m_playerTransform.position - agent.transform.position;
                toPlayer.y = 0;
                agent.transform.forward = Vector3.Slerp(agent.transform.forward, toPlayer, agent.settings.afterAttackLerpSpeed);
            }

            // attack type update
            agent.attack.AttackUpdate();

            // check if the agent is still attacking
            if(!agent.anim.GetBool("isAttacking"))
            {
                agent.ChangeState(StateIndex.chasePlayer);
                return;
            }
        }

        public override void Exit(AIAgent agent)
        {
            // clean up state Values
            // clean up animation
            agent.anim.SetBool("isAttacking", false);
            agent.anim.SetBool("lockRotation", false);

            // Clean up attack type
            agent.attack.AttackExit();

            agent.navAgent.nextPosition = agent.transform.position;
        }
    }

    public class Dead : AgentState
    {
        float m_timer = 0.0f;

        delegate void AgentAction(AIAgent agent);
        AgentAction actionDelegate = (agent) => { };

        public override void Enter(AIAgent agent)
        {
            // set up state values
            agent.StopNavigating();
            agent.ragdoll.RagdollOn = true;
            agent.charCollider.enabled = false;
            agent.innerCollider.enabled = false;

            m_timer = 0.0f;
            actionDelegate = DecayTimer;
        }

        public override void Update(AIAgent agent)
        {
            actionDelegate.Invoke(agent);
        }

        public override void Exit(AIAgent agent)
        {
            // clean up state Values
            agent.ragdoll.RagdollOn = false;
            agent.charCollider.enabled = true;
            agent.innerCollider.enabled = true;
            SetDissolveFloat(agent, 0.0f);
            actionDelegate = (clearAgent) => { };
        }

        void DecayTimer(AIAgent agent)
        {
            if (m_timer > agent.settings.bodyDecayTime)
            {
                m_timer = 0.0f;
                actionDelegate = DissolveTimer;
            }

            m_timer += Time.deltaTime;
        }

        void DissolveTimer(AIAgent agent)
        {
            if (m_timer > agent.settings.dissolveTime)
            {
                agent.DisablePoolObject();
                actionDelegate = (clearAgent) => { };
                return;
            }

            m_timer += Time.deltaTime;
            SetDissolveFloat(agent, m_timer / agent.settings.dissolveTime);
        }

        void SetDissolveFloat(AIAgent agent, float value)
        {
            int id = Shader.PropertyToID(agent.settings.dissolveShaderEffect);

            foreach(var mat in agent.dissolveMaterials)
            {
                mat.SetFloat(id, value);
            }
        }
    }

    public class BeginVault : AgentState
    {
        Vector3 m_vaultPosition;
        Vector3 m_vaultEnd;

        Vector3 beginPos;

        Vector3 beginForward;
        Vector3 endForward;

        bool m_startedVault = false;

        float approachSpeed = 0.0f;
        float t = 0.0f;

        public override void Enter(AIAgent agent)
        {
            if (agent.navAgent.currentOffMeshLinkData.offMeshLink.area == 4)
            {
                return;
            }

            // set up state values
            m_vaultPosition = agent.navAgent.currentOffMeshLinkData.startPos;
            m_vaultEnd = agent.navAgent.currentOffMeshLinkData.endPos;

            beginPos = agent.transform.position;

            beginForward = agent.transform.forward;
            endForward = (m_vaultEnd - m_vaultPosition);
            endForward.y = 0;
            endForward = endForward.normalized;

            m_vaultPosition += endForward * agent.settings.vaultStartforwardOffset;

            m_startedVault = false;

            t = 0.0f;
            approachSpeed = agent.previousSpeed;
            approachSpeed = Mathf.Max(approachSpeed, agent.settings.moveSpeed); // TODO: Put a clamp here of the agents walk speed and run speed;
            agent.vaultSpeed = approachSpeed;

            agent.postVaultState = agent.previousState;
        }

        public override void Update(AIAgent agent)
        {
            agent.ReuseLastAnimMoveSpeed();
            if (m_startedVault)
            {
                
                return;
            }

            t += Time.deltaTime * agent.settings.vaultStartSensitivity * approachSpeed;

            if (t >= 1.0f)
            {
                t = 1.0f;
                m_startedVault = true;
                agent.anim.SetTrigger("Vault");
            }

            var current = Vector3.Lerp(beginPos, m_vaultPosition, t);

            agent.transform.position = current;

            agent.transform.forward = Vector3.Slerp(beginForward, endForward, t);
        }

        public override void Exit(AIAgent agent)
        {
            
        }
    }

    public class EndVault : AgentState
    {
        Vector3 m_vaultEnd;

        Vector3 beginPos;

        Vector3 beginForward;
        Vector3 endForward;

        float approachSpeed = 0.0f;
        float t = 0.0f;

        public override void Enter(AIAgent agent)
        {
            // set up state values
            Vector3 vaultPosition = agent.navAgent.currentOffMeshLinkData.startPos;
            m_vaultEnd = agent.navAgent.currentOffMeshLinkData.endPos;

            beginPos = agent.transform.position;

            beginForward = agent.transform.forward;
            endForward = m_vaultEnd - vaultPosition;
            endForward.y = 0;
            endForward = endForward.normalized;

            m_vaultEnd.y = vaultPosition.y;
            m_vaultEnd += endForward * agent.navAgent.radius;

            t = 0.0f;
            approachSpeed = agent.vaultSpeed;
        }

        public override void Update(AIAgent agent)
        {
            t += Time.deltaTime * agent.settings.vaultEndSensitivity * approachSpeed;

            if (t < 1.0f)
            {
                var current = Vector3.Lerp(beginPos, m_vaultEnd, t);

                agent.transform.position = current;
            }

            if (t < 1.0f)
            {
                agent.transform.forward = Vector3.Slerp(beginForward, endForward, t);
            }

            if (t >= 1.0f)
            {
                if(agent.VaultGroundCheck(agent.settings.vaultMinDistanceCheck))
                {
                    agent.CompleteOffMeshLink("IdleTrigger");
                    //agent.navAgent.velocity = Vector3.zero;
                }
                else
                {
                    agent.anim.SetTrigger("Fall");
                    agent.ChangeState(StateIndex.falling);
                }
            }

        }

        public override void Exit(AIAgent agent)
        {

        }
    }

    public class Falling : AgentState
    {
        float m_verticalVelocity = 0.0f;

        public override void Enter(AIAgent agent)
        {
            m_verticalVelocity = agent.settings.vaultInitialFallVelocity;
        }

        public override void Update(AIAgent agent)
        {
            m_verticalVelocity += Physics.gravity.y * Time.deltaTime;

            Vector3 move = Vector3.up * m_verticalVelocity * Time.deltaTime;

            if(agent.VaultGroundCheck(-m_verticalVelocity * Time.deltaTime))
            {
                agent.CompleteOffMeshLink("Land");
                agent.navAgent.velocity = Vector3.zero;
                agent.StopNavigating();
                return;
            }

            agent.transform.position += move;
        }

        public override void Exit(AIAgent agent)
        {
            
        }
    }

    public class AlwaysAttackPlayer : AgentState
    {
        Transform m_playerTransform;
        Vector3 m_originalLocation;
        Vector3 m_attackDirection;

        public override void Enter(AIAgent agent)
        {
            // set up state values
            m_playerTransform = agent.playerTransform;
            m_originalLocation = agent.transform.position;
            m_attackDirection = (agent.playerTransform.position - agent.transform.position);
            m_attackDirection.y = 0;
            m_attackDirection = m_attackDirection.normalized;

            // Set up animation
            agent.StopNavigating();

            agent.anim.SetTrigger("attack");
            agent.anim.SetBool("isAttacking", true);
            agent.anim.SetBool("lockRotation", true);

            // Set up attack type
            agent.attack.AttackEnter(m_playerTransform, m_attackDirection);

            //agent.agentAudio.attackEmitter.Play();
        }

        public override void Update(AIAgent agent)
        {
            // this would be the logic where the attack could take place and wait until it has ended.
            if (agent.anim.GetBool("lockRotation"))
            {
                // only aim towards the initial attack direction
                agent.transform.forward = Vector3.Slerp(agent.transform.forward, m_attackDirection, agent.settings.rotationLerpSpeed);
            }
            else
            {
                // start looking towards the player
                Vector3 toPlayer = m_playerTransform.position - agent.transform.position;
                toPlayer.y = 0;
                agent.transform.forward = Vector3.Slerp(agent.transform.forward, toPlayer, agent.settings.afterAttackLerpSpeed);
            }

            // attack type update
            agent.attack.AttackUpdate();

            // check if the agent is still attacking
            if (!agent.anim.GetBool("isAttacking"))
            {
                agent.ChangeState(StateIndex.debugAlwaysAttack);
                return;
            }
        }

        public override void Exit(AIAgent agent)
        {
            // clean up state Values
            // clean up animation
            agent.anim.SetBool("isAttacking", false);
            agent.anim.SetBool("lockRotation", false);

            // Clean up attack type
            agent.attack.AttackExit();

            agent.navAgent.nextPosition = agent.transform.position;
        }
    }
}
