using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTracker : MonoBehaviour
{
    public Vector3 cellSize = Vector3.one;
    public Vector3 offset = Vector3.one * 0.5f;
    [Min(1)]public int cellRange = 1;

    public Transform playerTransform;

    SpawnPartitions m_spawnPartitions;

    private void Awake()
    {
        m_spawnPartitions = new SpawnPartitions(cellSize, offset);

        var spawnerArray = FindObjectsOfType<EnemySpawner>(true);

        foreach(EnemySpawner spawner in spawnerArray)
        {
            spawner.spawnTracker = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public HashSet<SpawnLocation> GetPlayerAdjacentCellSpawnLocations()
    {
        HashSet<SpawnLocation> result = new HashSet<SpawnLocation>();
        foreach (IVec3 cellArray in GetPlayerAdjacentCells())
        {
            HashSet<SpawnLocation> locations = m_spawnPartitions.GetEnemySpawners(cellArray);
            if(locations != null)
            {
                result.UnionWith(locations);
            }
        }

        return result;
    }

    public IVec3 GetPlayerCell()
    {
        return GetPlayerCell(m_spawnPartitions, playerTransform);
    }

    public static IVec3 GetPlayerCell(SpawnPartitions spawnPartitions, Transform playerTransform)
    {
        return spawnPartitions.GetCell(playerTransform.position);
    }

    public IVec3[,,] GetPlayerAdjacentCells()
    {
        return GetPlayerAdjacentCells(this, m_spawnPartitions, playerTransform);
    }

    public static IVec3[,,] GetPlayerAdjacentCells(SpawnTracker spawnTracker, SpawnPartitions spawnPartitions, Transform playerTransform)
    {
        IVec3 playerCell = GetPlayerCell(spawnPartitions, playerTransform);
        int arraySize = 1 + (spawnTracker.cellRange * 2);
        IVec3[,,] playerCellArray = new IVec3[arraySize, arraySize, arraySize];

        for (int x = 0; x < arraySize; x++)
        {
            for (int y = 0; y < arraySize; y++)
            {
                for (int z = 0; z < arraySize; z++)
                {
                    IVec3 adjacentCell = IVec3.zero;
                    adjacentCell.x = playerCell.x + x - spawnTracker.cellRange;
                    adjacentCell.y = playerCell.y + y - spawnTracker.cellRange;
                    adjacentCell.z = playerCell.z + z - spawnTracker.cellRange;

                    playerCellArray[x, y, z] = adjacentCell;
                }
            }
        }

        return playerCellArray;
    }

    private void OnValidate()
    {
        offset.x = Mathf.Clamp(offset.x, -1.0f, 1.0f);
        offset.y = Mathf.Clamp(offset.y, -1.0f, 1.0f);
        offset.z = Mathf.Clamp(offset.z, -1.0f, 1.0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        SpawnPartitions spawnPartitions = m_spawnPartitions;
        if (spawnPartitions == null)
        {
            spawnPartitions = new SpawnPartitions(cellSize, offset);
        }

        IVec3[,,] playerCellArray = GetPlayerAdjacentCells(this, spawnPartitions, playerTransform);

        foreach (var cell in playerCellArray)
        {
            Vector3 pos = offset;
            pos.x *= cellSize.x;
            pos.y *= cellSize.y;
            pos.z *= cellSize.z;

            pos.x += (cell.x + 0.5f) * cellSize.x;
            pos.y += (cell.y + 0.5f) * cellSize.y;
            pos.z += (cell.z + 0.5f) * cellSize.z;
            Gizmos.DrawWireCube(pos, cellSize);
        }

        var spawnLocations = Object.FindObjectsOfType<SpawnLocation>();

        foreach(var location in spawnLocations)
        {
            IVec3 locationCell = spawnPartitions.GetCell(location);
            foreach(var playerCell in playerCellArray)
            {
                if(locationCell == playerCell)
                {
                    Gizmos.DrawSphere(location.transform.position, 1.0f);
                }
            }
        }
    }
}
