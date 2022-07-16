using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomBounds : MonoBehaviour
{
    Bounds entryBounds;
    Bounds exitBounds;

    public float exitExpansion = 5.0f;

    public List<RoomBounds> neighbours;

    public UnityEvent enterRoom;
    public UnityEvent exitRoom;

    public Vector3 safeSpawnOffset = Vector3.zero;

    // Initialisation variables
    public HashSet<SpawnLocation> spawnLocations = new HashSet<SpawnLocation>();

    public bool singleEvents = true;
    bool m_hasEntered = false;
    bool m_hasExited = false;

    void Awake()
    {
        Init();
    }

    private void Start()
    {
        
    }

    void Init()
    {
        entryBounds.center = transform.position;
        entryBounds.size = Vector3.zero;

        FindMinAndMax(out Vector3 min, out Vector3 max);

        entryBounds.SetMinMax(min, max);

        exitBounds.center = entryBounds.center;
        exitBounds.SetMinMax(min, max);
        exitBounds.Expand(exitExpansion);
    }

    protected virtual void FindMinAndMax(out Vector3 min, out Vector3 max)
    {
        List<Transform> allObjectsHeap = FindAllChildren();
        var allBounds = FindAllBounds(allObjectsHeap);
        FindMinAndMax(allBounds, out min, out max);
    }

    void ProcessEvent(UnityEvent targetEvent)
    {
        targetEvent.Invoke();
        if (singleEvents)
        {
            targetEvent.RemoveAllListeners();
        }
    }

    public void ProcessEnterEvent()
    {
        if(m_hasEntered)
        {
            return;
        }
        m_hasEntered = true;
        ProcessEvent(enterRoom);
    }

    public void ProcessExitEvent()
    {
        if (m_hasExited)
        {
            return;
        }
        m_hasExited = true;
        ProcessEvent(exitRoom);
    }

    public void SetEnterStatus(bool status)
    {
        m_hasEntered = status;
    }

    public void SetExitStatus(bool status)
    {
        m_hasExited = status;
    }

    List<Transform> FindAllChildren()
    {
        List<Transform> allObjectsHeap = new List<Transform>();
        allObjectsHeap.Add(transform);

        for (int i = 0; i < allObjectsHeap.Count; i++)
        {
            FindChildren(allObjectsHeap, allObjectsHeap[i]);
        }

        return allObjectsHeap;
    }

    void FindChildren(List<Transform> allObjectsHeap, Transform transform)
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            allObjectsHeap.Add(child);
        }
    }

    List<Bounds> FindAllBounds(List<Transform> allObjectsHeap)
    {
        List<Bounds> allBounds = new List<Bounds>();

        foreach(Transform obj in allObjectsHeap)
        {
            var collider = obj.GetComponent<Collider>();

            if(collider != null)
            {
                allBounds.Add(collider.bounds);
            }
        }

        return allBounds;
    }

    void FindMinAndMax(List<Bounds> allBounds, out Vector3 min, out Vector3 max)
    {
        min = allBounds[0].min;
        max = allBounds[0].max;

        for(int i = 1; i < allBounds.Count; i++)
        {
            Bounds current = allBounds[i];
            min = FindMins(min, current.min);
            max = FindMaxs(max, current.max);
        }
    }

    Vector3 FindMins(Vector3 currentMin, Vector3 value)
    {
        currentMin.x = FindMin(currentMin.x, value.x);
        currentMin.y = FindMin(currentMin.y, value.y);
        currentMin.z = FindMin(currentMin.z, value.z);

        return currentMin;
    }

    float FindMin(float currentMin, float value)
    {
        if (value < currentMin)
        {
            return value;
        }
        return currentMin;
    }

    Vector3 FindMaxs(Vector3 currentMax, Vector3 value)
    {
        currentMax.x = FindMax(currentMax.x, value.x);
        currentMax.y = FindMax(currentMax.y, value.y);
        currentMax.z = FindMax(currentMax.z, value.z);

        return currentMax;
    }

    float FindMax(float currentMax, float value)
    {
        if (value > currentMax)
        {
            return value;
        }
        return currentMax;
    }

    public float Distance(Vector3 point)
    {
        Vector3 closestPoint = entryBounds.ClosestPoint(point);
        return (closestPoint - point).magnitude;
    }

    public bool EntryContainsPoint(Vector3 point)
    {
        return entryBounds.Contains(point);
    }    

    public bool ExitContainsPoint(Vector3 point)
    {
        return exitBounds.Contains(point);
    }

    public Vector3 GetSafeSpawnPosition()
    {
        return entryBounds.center + safeSpawnOffset;
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            Color colour = Color.red;
            colour.a *= 0.4f;
            Gizmos.color = colour;
    
            Gizmos.DrawCube(entryBounds.center, entryBounds.size);
    
            colour = Color.green;
            colour.a *= 0.4f;
    
            Gizmos.color = colour;
            Gizmos.DrawCube(exitBounds.center, exitBounds.size);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(entryBounds.center + safeSpawnOffset, 0.5f);
        }
    }
}
