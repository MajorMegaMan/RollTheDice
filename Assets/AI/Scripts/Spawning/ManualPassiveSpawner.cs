using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualPassiveSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct PassiveSpawnPackage
    {
        public string name;
        public Transform location;
        public List<Transform> patrolNodes;
    }

    public EnemySpawner enemySpawner;
    public List<PassiveSpawnPackage> passiveSpawnPackages;

    private void Start()
    {
        enemySpawner.Init();
    }

    public void PassiveSpawn(int index)
    {
        enemySpawner.SpawnPassive(passiveSpawnPackages[index].location.position, passiveSpawnPackages[index].patrolNodes);
    }

    public void AggroSpawn(int index)
    {
        enemySpawner.SpawnAggro(passiveSpawnPackages[index].location.position);
    }
}
