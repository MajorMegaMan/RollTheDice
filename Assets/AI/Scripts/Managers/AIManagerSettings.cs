using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serialisable;

[CreateAssetMenu(menuName = "AI/Scriptables/AIManagerSettings")]
public class AIManagerSettings : ScriptableObject
{
    [System.Serializable]
    public struct EnemyInfo
    {
        public GameObject targetPrefab;
        public int maxCount;
        public string containerName;
    }

    [SerializeField] SerialisedDictionary<EnemyType, EnemyInfo> m_enemies = new SerialisedDictionary<EnemyType, EnemyInfo>();

    public Dictionary<EnemyType, AgentObjectPool> CreateObjectPools(AIManager aiManager)
    {
        Dictionary<EnemyType, AgentObjectPool> enemyPoolList = new Dictionary<EnemyType, AgentObjectPool>();
        Dictionary<EnemyType, EnemyInfo> enemyDict = m_enemies.GetDictionary();
        foreach(var enemyInfoPair in enemyDict)
        {
            enemyPoolList.Add(enemyInfoPair.Key, CreatePool(aiManager, enemyInfoPair.Value));
        }
        return enemyPoolList;
    }

    static AgentObjectPool CreatePool(AIManager aiManager, EnemyInfo enemyInfo)
    {
        AgentObjectPool enemyPool = new AgentObjectPool();
        EnemyType enemyType = enemyInfo.targetPrefab.GetComponent<AIAgent>().settings.enemyType;
        enemyPool.InitialiseObjectPool(aiManager, enemyInfo.containerName, enemyInfo.maxCount, enemyInfo.targetPrefab, enemyType);
        return enemyPool;
    }

    public EnemyType GetEnemyTypeMask()
    {
        EnemyType result = 0;
        Dictionary<EnemyType, EnemyInfo> enemyDict = m_enemies.GetDictionary();
        foreach (var enemyInfoPair in enemyDict)
        {
            result = result | enemyInfoPair.Key;
        }
        return result;
    }

    private void OnValidate()
    {
        
    }
}
