using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentObjectPool 
{
    // object pool
    List<EnemyPoolObject> m_objectPool = new List<EnemyPoolObject>();
    public List<EnemyPoolObject> objectPool { get { return m_objectPool; } }
    int m_currentIndex = 0;
    int m_activeCount = 0;

    int m_maxCount = 0;

    GameObject objectContainer;
    int m_existingCount = 0;

    public int activeCount { get { return m_activeCount; } }
    public List<AIAgent> activeAgents
    {
        get
        {
            List<AIAgent> result = new List<AIAgent>();
            foreach(var poolAgent in objectPool)
            {
                if(poolAgent.isActive)
                {
                    result.Add(poolAgent.agent);
                }
            }
            return result;
        }
    }

    public void InitialiseObjectPool(AIManager manager, string containerName, int maxCount, GameObject enemyPrefab, EnemyType enemyType)
    {
        m_maxCount = maxCount;

        objectContainer = new GameObject(containerName);

        var existingArray = Object.FindObjectsOfType<AIAgent>();
        foreach(AIAgent existingAgent in existingArray)
        {
            if(existingAgent.aiManager == manager)
            {
                // Existing agents should be included in this object pool
                if(existingAgent.settings.enemyType == enemyType)
                {
                    AddExistingAgent(existingAgent);
                }
            }
        }

        // Create Object Pool
        for (int i = m_existingCount; i < maxCount; i++)
        {
            var newCultist = Object.Instantiate(enemyPrefab, objectContainer.transform);
            EnemyPoolObject poolAgent = new EnemyPoolObject(this, newCultist, manager);
            m_objectPool.Add(poolAgent);

            manager.allAgents.Add(poolAgent.agent);
        }
    }

    public void AddExistingAgent(AIAgent agent)
    {
        m_existingCount++;
        agent.transform.parent = objectContainer.transform;
        EnemyPoolObject poolAgent = new EnemyPoolObject(this, agent.gameObject, true);
        m_objectPool.Add(poolAgent);
        m_activeCount++;
    }

    public bool GetNextObject(out AIAgent resultPoolObject)
    {
        bool result = false;

        int startIndex = m_currentIndex;
        resultPoolObject = null;

        do
        {
            m_currentIndex++;
            m_currentIndex = m_currentIndex % m_maxCount;

            EnemyPoolObject target = m_objectPool[m_currentIndex];
            if (!target.gameObject.activeInHierarchy)
            {
                resultPoolObject = target.agent;
                result = true;
                break;
            }

        } while (m_currentIndex != startIndex);

        return result;
    }

    public bool SetObjectActive(out AIAgent targetPoolObject)
    {
        bool result = GetNextObject(out targetPoolObject);
        if(result)
        {
            SetTargetObjectActive(targetPoolObject);
        }

        return result;
    }

    public void SetTargetObjectActive(AIAgent targetPoolObject)
    {
        targetPoolObject.attachedPoolObject.SetActive(true);
        targetPoolObject.gameObject.SetActive(true);
        m_activeCount++;
    }

    public void DisableObject(EnemyPoolObject target)
    {
        target.SetActive(false); // refernce to EnemyPoolObject
        target.gameObject.SetActive(false); // refernce to Unity's GameObject
        m_activeCount--;
    }
}
