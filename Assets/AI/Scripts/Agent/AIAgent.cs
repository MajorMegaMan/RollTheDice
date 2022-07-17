using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIStates;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Ragdoll))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerPush))]
public class AIAgent : MonoBehaviour
{
    public AIManager aiManager;
    [HideInInspector]public EnemyPoolObject attachedPoolObject;

    public AISettings settings;
    public Animator anim;
    public AttackType attack;
    public CapsuleCollider charCollider;
    public CapsuleCollider innerCollider;
    public AgentAudio agentAudio;
    public Renderer[] agentRenderers;
    List<Material> m_dissovleMaterials;

    bool m_isInitialised = false;
    StateMachine<AIAgent> m_stateMachine;
    StateIndex m_previousState;

    NavMeshAgent m_navAgent;
    Ragdoll m_ragdoll;
    Health m_health;

    float m_distToPlayerSquared;
    float m_currentSpeed = 0.0f;
    float m_previousSpeed = 0.0f;

    List<Neighbour> m_neighbours = new List<Neighbour>();

    // anim
    int m_hurtLayerIndex;
    bool m_isHurting = false;
    [HideInInspector]public float safeLeaveIsHurtTimer = 0.0f;

    // getters
    public NavMeshAgent navAgent { get { return m_navAgent; } }
    public float distToPlayerSquared { get { return m_distToPlayerSquared; } }
    public Transform playerTransform { get { return aiManager.playerTransform; } }
    public Ragdoll ragdoll {  get { return m_ragdoll; } }
    public Health health { get { return m_health; } }
    public List<Material> dissolveMaterials { get { return m_dissovleMaterials; } }
    public List<Neighbour> neighbours { get { return m_neighbours; } }

    public float currentSpeed { get { return m_currentSpeed; } }
    public float previousSpeed { get { return m_previousSpeed; } }

    public bool isAlive { get { return m_stateMachine.currentIndex != (int)StateIndex.dead; } }
    public StateIndex currentState { get { return (StateIndex)m_stateMachine.currentIndex; } }

    public bool isHurting { get { return m_isHurting; } set { m_isHurting = value; } }

    // statmeachine refences
    public StateIndex previousState { get { return m_previousState; } }
    public StateIndex postVaultState;
    public float vaultSpeed = 0.0f;
    public float attackCharge = 0.0f;

    // audio
    bool m_delayingSound = false;

    // Debug
    [Header("Debugging")]
    [SerializeField]
    StateIndex debugCurrentState;

    public int debugNeighbourCount = 0;
    
    public List<Transform> patrolNodes;
    public int currentPatrolIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        m_currentSpeed = m_navAgent.velocity.magnitude;
        m_stateMachine.Update();

        float animMoveSpeed = EvaluateAnimMoveSpeed(m_currentSpeed);
        anim.SetFloat("moveSpeed", animMoveSpeed);
        m_previousSpeed = m_currentSpeed;
        //anim.SetFloat("moveSpeed", m_currentSpeed / navAgent.speed);

        // Debugging
        debugCurrentState = (StateIndex)m_stateMachine.currentIndex;
        debugNeighbourCount = neighbours.Count;
    }

    private void LateUpdate()
    {
        m_distToPlayerSquared = (playerTransform.position - transform.position).sqrMagnitude;
    }

    public void Init()
    {
        if(m_isInitialised)
        {
            return;
        }

        m_isInitialised = true;

        m_navAgent = GetComponent<NavMeshAgent>();
        m_navAgent.autoTraverseOffMeshLink = false;

        m_ragdoll = GetComponent<Ragdoll>();
        m_health = GetComponent<Health>();

        // Create State Machine
        m_stateMachine = new StateMachine<AIAgent>(this);

        // Add States
        StateBucket.SetUpStateMachine(m_stateMachine);

        m_stateMachine.Init();
        m_previousState = (StateIndex)m_stateMachine.currentIndex;

        m_distToPlayerSquared = (playerTransform.position - transform.position).sqrMagnitude;

        GetComponent<PlayerPush>().player = aiManager.playerTransform.GetComponent<PlayerController>();// This is disgusting line but I'm lazy

        StabiliseSettings();
        ResetHealth();

        m_dissovleMaterials = new List<Material>();
        foreach(Renderer render in agentRenderers)
        {
            m_dissovleMaterials.Add(render.material);
        }

        m_hurtLayerIndex = anim.GetLayerIndex("Hurt Layer");
    }

    public void ChangeState(int stateIndex)
    {
        m_stateMachine.ChangeState(stateIndex);
        m_previousState = (StateIndex)m_stateMachine.currentIndex;
    }

    public void ChangeState(StateIndex stateIndex)
    {
        ChangeState((int)stateIndex);
    }

    public void ChasePlayer()
    {
        ChangeState(StateIndex.chasePlayer);
        attackCharge = settings.attackChargeMax;
    }

    public void StartNavigating()
    {
        m_navAgent.isStopped = false;
        //m_navAgent.updatePosition = true;
    }

    public void StopNavigating()
    {
        m_navAgent.isStopped = true;
        //m_navAgent.updatePosition = false;
    }

    public void DamagePlayer()
    {
        aiManager.playerHealth.Hit(settings.attackDamage, null);
        aiManager.damagePlayerEvent.Invoke();
    }

    public void BeforeHit()
    {
        Collider hitCollider = health.hitCollider;

        RagdollExtra.RagdollType type = hitCollider.GetComponent<RagdollExtra>().GetRagdollType();
        switch (type)
        {
            case RagdollExtra.RagdollType.head:
                {
                    health.damageMultiplier = settings.headMultiplier;
                    break;
                }
            case RagdollExtra.RagdollType.upperTorso:
                {
                    health.damageMultiplier = settings.upperTorsoMultipler;
                    break;
                }
            case RagdollExtra.RagdollType.lowerTorso:
                {
                    health.damageMultiplier = settings.lowerTorsoMultiplier;
                    break;
                }
            case RagdollExtra.RagdollType.limb:
                {
                    health.damageMultiplier = settings.limbMultiplier;
                    break;
                }
        }
    }

    public void OnHit()
    {
        if(health.health > 0)
        {
            if(currentState < StateIndex.chasePlayer)
            {
                ChasePlayer();
            }

            // agent is hurt
            //agentAudio.hurtEmitter.Play();
            if(!m_delayingSound)
            {
                m_delayingSound = true;
                StartCoroutine(SoundDelay(agentAudio.hurtClip, settings.hurtDelay));
                StartHurt();
            }
        }
    }

    IEnumerator StartHurt(float timeLength)
    {
        safeLeaveIsHurtTimer = 0.0f;
        if(m_isHurting)
        {
            for (float current = 0.0f; current < 1.0f; current += Time.deltaTime / timeLength)
            {
                anim.SetLayerWeight(m_hurtLayerIndex, current);
                yield return null;
            }
            anim.SetLayerWeight(m_hurtLayerIndex, 1.0f);
        }
    }

    void StartHurt()
    {
        m_isHurting = true;
        anim.SetTrigger("Hurt");
        StartCoroutine(StartHurt(0.01f));
    }

    // Relatable
    void EndHurt()
    {
        m_isHurting = false;
        anim.SetLayerWeight(m_hurtLayerIndex, 0.0f);
    }

    IEnumerator SoundDelay(AudioClip clip, float delay)
    {
        for(float t = 0.0f; t < delay; t += Time.deltaTime)
        {
            yield return null;
        }
        agentAudio.PlayClip(clip);
        m_delayingSound = false;
    }

    public void Die()
    {
        if(m_stateMachine.currentIndex == (int)StateIndex.dead)
        {
            // Check if this is already in the dead state and if it is, do nothing
            return;
        }

        StartCoroutine(SoundDelay(agentAudio.deathClip, settings.deathDelay));

        // This would ideally have an animation as well as some sort of clean up for corpses
        // For now just Change state to dead which will activate a ragdoll
        aiManager.agentdeathEvent.Invoke();
        ChangeState(StateIndex.dead);
    }

    public void ResetHealth()
    {
        m_health.health = m_health.maxHealth;
    }

    public void DisablePoolObject()
    {
        aiManager.ReduceAliveCount();
        attachedPoolObject.Disable();
    }

    // Called in an animation Event
    public void CompleteVault()
    {
        ChangeState(StateIndex.endVault);
    }

    public bool VaultGroundCheck(float verticalVelocity)
    {
        Vector3 rayStart = transform.position;
        rayStart.y += settings.vaultgroundCheckOffset;

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hitinfo, verticalVelocity + settings.vaultgroundCheckOffset, settings.groundCheckLayer, QueryTriggerInteraction.Ignore))
        {
            // hit something
            return true;
        }

        return false;
    }

    public void CompleteOffMeshLink(string animTrigger)
    {
        navAgent.CompleteOffMeshLink();
        ChangeState(postVaultState);
        anim.SetTrigger(animTrigger);
    }

    public void Seek()
    {
        if(neighbours.Count == 0)
        {
            return;
        }

        Vector3 seekDir = Vector3.zero;

        foreach(var neighbour in neighbours)
        {
            seekDir -= neighbour.offset;
        }

        seekDir /= neighbours.Count;
        seekDir = Vector3.ClampMagnitude(seekDir, m_navAgent.speed);

        m_navAgent.Move(seekDir * Time.deltaTime);
    }

    // This function is called to ensure that the agents components match the agent's settings
    public void StabiliseSettings()
    {
        m_navAgent.speed = settings.moveSpeed;
        m_navAgent.angularSpeed = settings.angularSpeed;
        m_navAgent.acceleration = settings.acceleration;

        m_health.maxHealth = settings.health;
    }

    public Bounds GetBounds()
    {
        return charCollider.bounds;
    }

    // This is called as an animation event
    void AgentFootStep()
    {
        agentAudio.PlayClip(agentAudio.footClip);
    }

    public float EvaluateAnimMoveSpeed(float currentMoveSpeed)
    {
        return settings.moveAnimSpeed.Evaluate(currentMoveSpeed / navAgent.speed);
    }

    // This should be called during States in the agents state machine when animations are driving the agent instead of navagent.
    // For example vaulting offmesh links.
    public void ReuseLastAnimMoveSpeed()
    {
        m_currentSpeed = m_previousSpeed;
    }

    private void OnDrawGizmos()
    {
        if(aiManager == null || !aiManager.showEnemyGizmos)
        {
            return;
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, settings.aggresionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, settings.attackChargeRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, settings.orbWalkRadius);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, settings.aggresionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, settings.attackChargeRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, settings.orbWalkRadius);
    }
}
