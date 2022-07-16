using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public EnemyInfo[] enemies = new EnemyInfo[1];

    public List<AgentObjectPool> CreateObjectPools(AIManager aiManager)
    {
        List<AgentObjectPool> enemyPoolList = new List<AgentObjectPool>();
        foreach(EnemyInfo enemyInfo in enemies)
        {
            enemyPoolList.Add(CreatePool(aiManager, enemyInfo));
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

    private void OnValidate()
    {
        if(enemies.Length == 0)
        {
            enemies = new EnemyInfo[1];
            Debug.LogWarning("There needs to be at least one enemy type");
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            if(enemies[i].maxCount < 1)
            {
                enemies[i].maxCount = 1;
            }
        }
    }
}
