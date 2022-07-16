using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPartitions
{
    Vector3 m_cellSize = Vector3.one;
    Vector3 m_offset = Vector3.zero;

    SpawnLocation[] m_spawnLocations;

    SortedDictionary<IVec3, HashSet<SpawnLocation>> m_cellSpawns = new SortedDictionary<IVec3, HashSet<SpawnLocation>>(new IVec3Comparer());

    public Vector3 cellSize {  get { return m_cellSize; } }

    public SpawnPartitions()
    {
        Init();
    }

    public SpawnPartitions(float uniformCellSize)
    {
        m_cellSize = Vector3.one * uniformCellSize;

        Init();
    }

    public SpawnPartitions(Vector3 cellSize, Vector3 offset)
    {
        m_cellSize = cellSize;
        m_offset = offset;
        m_offset.x *= cellSize.x;
        m_offset.y *= cellSize.y;
        m_offset.z *= cellSize.z;

        Init();
    }

    public void Init()
    {
        m_spawnLocations = Object.FindObjectsOfType<SpawnLocation>();

        foreach (SpawnLocation location in m_spawnLocations)
        {
            IVec3 cellKey = GetCell(location);
            if(!m_cellSpawns.ContainsKey(cellKey))
            {
                m_cellSpawns.Add(cellKey, new HashSet<SpawnLocation>());
            }
            var list = m_cellSpawns[cellKey];
            list.Add(location);
        }
    }

    public HashSet<SpawnLocation> GetEnemySpawners(IVec3 targetCell)
    {
        if(m_cellSpawns.TryGetValue(targetCell, out HashSet<SpawnLocation> result))
        {
            return result;
        }
        else
        {
            return null;
        }
    }

    public IVec3 GetCell(SpawnLocation spawnLocation)
    {
        return GetCell(spawnLocation.transform.position);
    }

    public IVec3 GetCell(Vector3 targetPosition)
    {
        Vector3 normalised = targetPosition - m_offset;
        normalised.x /= m_cellSize.x;
        normalised.y /= m_cellSize.y;
        normalised.z /= m_cellSize.z;

        // Fix negative mod error
        if(normalised.x < 0.0f)
        {
            normalised.x -= 1.0f;
        }
        if (normalised.y < 0.0f)
        {
            normalised.y -= 1.0f;
        }
        if (normalised.z < 0.0f)
        {
            normalised.z -= 1.0f;
        }

        IVec3 result = IVec3.zero;
        result.x = (int)normalised.x;
        result.y = (int)normalised.y;
        result.z = (int)normalised.z;

        return result;
    }
}

public struct IVec3
{
    public int x;
    public int y;
    public int z;

    static IVec3 _zero;
    public static IVec3 zero { get { return _zero; } }

    public IVec3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    static IVec3()
    {
        _zero = new IVec3(0, 0, 0);
    }

    public static bool operator ==(IVec3 lhs, IVec3 rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
    }

    public static bool operator !=(IVec3 lhs, IVec3 rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
    }
}

public class IVec3Comparer : IComparer<IVec3>
{
    public int Compare(IVec3 lhs, IVec3 rhs)
    {
        // check x
        if (lhs.x < rhs.x)
        {
            return -1;
        }
        else if (lhs.x > rhs.x)
        {
            return 1;
        }

        // check y
        if (lhs.y < rhs.y)
        {
            return -1;
        }
        else if (lhs.y > rhs.y)
        {
            return 1;
        }

        // check z
        if (lhs.z < rhs.z)
        {
            return -1;
        }
        else if (lhs.z > rhs.z)
        {
            return 1;
        }

        return 0;
    }
}
