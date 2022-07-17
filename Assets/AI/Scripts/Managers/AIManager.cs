using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Serialisable;

public class AIManager : MonoBehaviour
{
    public AIManagerSettings settings;

    List<AIAgent> m_allAgents = new List<AIAgent>();

    public Transform playerTransform;
    public Camera playerCam;
    public Health playerHealth;

    public UnityEvent agentdeathEvent;
    public UnityEvent spawnEvent;
    public UnityEvent damagePlayerEvent;

    public float neighbourRadius = 1.5f;

    public bool playerInTimePeriod = true;

    Dictionary<EnemyType, AgentObjectPool> m_enemyObjectPools;
    EnemyType m_enemyTypeMask = 0;

    public Dictionary<EnemyType, AgentObjectPool> enemyObjectPools { get { return m_enemyObjectPools; } }
    // enemyTypeMask is a bit flag containing the types that are found in the object pool dictionary
    public EnemyType enemyTypeMask { get { return m_enemyTypeMask; } }
    public List<AIAgent> allAgents { get { return m_allAgents; } }
    public List<AIAgent> activeAgents { get { return GetAllActiveAgents(); } }

    int m_aliveCount = 0;
    public int aliveCount { get { return m_aliveCount; } }

    //Debug
    [Header("Debug")]
    public bool showNeighbourRadius = false;
    public bool showEnemyGizmos = false;

    bool isInitialised = false;

    private void Awake()
    {
        isInitialised = true;

        m_enemyObjectPools = settings.CreateObjectPools(this);
        m_enemyTypeMask = settings.GetEnemyTypeMask();

        spawnEvent.AddListener(() => { m_aliveCount++; });
        //agentdeathEvent.AddListener(() => { m_aliveCount--; });
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        if(!playerInTimePeriod)
        {
            return;
        }

        FindNeighbours();
    }

    // Sets pool object to active from the target object pool and gets the resulting gameobject
    public bool SetPoolObjectActive(AgentObjectPool targetPool, out AIAgent resultObject)
    {
        return targetPool.SetObjectActive(out resultObject);
    }

    public bool GetNextPoolObject(AgentObjectPool targetPool, out AIAgent resultObject)
    {
        return targetPool.GetNextObject(out resultObject);
    }

    public void SetTargetObjectActive(AgentObjectPool targetPool, AIAgent targetObject)
    {
        targetPool.SetTargetObjectActive(targetObject);
    }

    public void AllAggro()
    {
        foreach(var agent in m_allAgents)
        {
            if(agent.gameObject.activeInHierarchy)
            {
                agent.ChangeState(AIStates.StateIndex.chasePlayer);
            }
        }
    }

    public void FindNeighbours()
    {
        foreach(AIAgent agent in m_allAgents)
        {
            agent.neighbours.Clear();
        }

        for(int i = 0; i < m_allAgents.Count; i++)
        {
            for(int j = i + 1; j < m_allAgents.Count; j++)
            {
                AIAgent first = m_allAgents[i];
                AIAgent second = m_allAgents[j];

                Neighbour neighbour = Neighbour.empty;
                neighbour.FindNeighbour(first, second);

                float neighbourRadSqr = neighbourRadius * neighbourRadius;

                if (neighbour.distSqrd < neighbourRadSqr)
                {
                    first.neighbours.Add(neighbour);
                    second.neighbours.Add(-neighbour);
                }
            }
        }
    }

    public List<AIAgent> GetAllActiveAgents()
    {
        List<AIAgent> result = new List<AIAgent>();
        foreach (var enemyPoolPair in m_enemyObjectPools)
        {
            foreach (AIAgent activeAgent in enemyPoolPair.Value.activeAgents)
            {
                result.Add(activeAgent);
            }
        }
        return result;
    }

    public void ClearAllAgents()
    {
        List<AIAgent> allAgents = GetAllActiveAgents();
        foreach (AIAgent agent in allAgents)
        {
            agent.DisablePoolObject();
        }
        m_aliveCount = 0;
    }

    // Only meant to be used for agent disable
    public void ReduceAliveCount()
    {
        m_aliveCount--;
    }

    private void OnDrawGizmos()
    {
        List<AIAgent> drawList;

        if(isInitialised)
        {
            drawList = m_allAgents;
        }
        else
        {
            drawList = new List<AIAgent>();

            var agentArray = FindObjectsOfType<AIAgent>();

            foreach (var agent in agentArray)
            {
                drawList.Add(agent);
            }
        }

        if(showNeighbourRadius)
        {
            foreach (var agent in drawList)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(agent.transform.position, neighbourRadius);
            }
        }
    }
}
