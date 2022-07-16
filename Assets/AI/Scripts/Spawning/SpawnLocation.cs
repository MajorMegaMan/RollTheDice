using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocation : MonoBehaviour
{
    public enum SpawnType
    {
        FIXED,
        DYNAMIC
    }

    public SpawnType spawnType = 0;
    bool m_isSpawning = false;

    [HideInInspector] public RoomBounds room = null;

    public bool isSpawning { get { return m_isSpawning; } }

    public float skinWidth = 0.0001f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSpawning()
    {
        m_isSpawning = true;

        // DEBUGGONG PURPOSE: END THE SPAWNING STRAIGHT AWAY
        // Ideally the spawn would last as long as the animation that plays it
        EndSpawning();
    }

    public void EndSpawning()
    {
        m_isSpawning = false;
    }

    public Bounds FindBounds(Bounds copyBounds)
    {
        copyBounds.center = transform.position + Vector3.up * copyBounds.extents.y;
        return copyBounds;
    }

    public bool IsInCameraView(Camera camera, Bounds agentBounds)
    {
        var frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        bool inCameraView = GeometryUtility.TestPlanesAABB(frustumPlanes, agentBounds);
        return inCameraView;
    }

    public bool AgentBoundsRayCheck(Bounds bounds, Vector3 target, LayerMask environmentMask)
    {
        Vector3[] origins = new Vector3[8];

        Vector3 extents = bounds.extents - Vector3.one * skinWidth;
        origins[0] = new Vector3(extents.x,   extents.y,  extents.z);
        origins[1] = new Vector3(-extents.x,  extents.y,  extents.z);
        origins[2] = new Vector3(-extents.x, -extents.y,  extents.z);
        origins[3] = new Vector3(extents.x,  -extents.y,  extents.z);

        origins[4] = new Vector3(extents.x,   extents.y, -extents.z);
        origins[5] = new Vector3(-extents.x,  extents.y, -extents.z);
        origins[6] = new Vector3(-extents.x, -extents.y, -extents.z);
        origins[7] = new Vector3(extents.x,  -extents.y, -extents.z);

        return AgentBoundsRayCheck(bounds.center, origins, target, environmentMask);
    }

    // returns true if all rays have hit an object. This would mean that the player can not see this spawn location
    public bool AgentBoundsRayCheck(Vector3 boundsOrigin, Vector3[] extentsArray, Vector3 target, LayerMask environmentMask)
    {
        for (int i = 0; i < 8; i++)
        {
            Vector3 rayOrigin = extentsArray[i] + boundsOrigin;
            Vector3 rayTarget = target;

            Vector3 dir = rayTarget - rayOrigin;
            if (!Physics.Raycast(rayOrigin, dir.normalized, out RaycastHit hitInfo, dir.magnitude, environmentMask))
            {
                // has not hit a wall
                return false;
            }
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        void DrawCube(Vector3 position, Color colour)
        {
            Gizmos.color = colour;
            Gizmos.DrawWireCube(position, Vector3.one);

            colour *= 0.4f;
            Gizmos.color = colour;
            Gizmos.DrawCube(position, Vector3.one);
        }

        Color drawColour = Color.white;
        if(isSpawning)
        {
            drawColour = Color.blue;
        }
        else
        {
            switch (spawnType)
            {
                case SpawnType.FIXED:
                    {
                        drawColour = Color.red;
                        break;
                    }
                case SpawnType.DYNAMIC:
                    {
                        drawColour = Color.yellow;
                        break;
                    }
            }
        }

        DrawCube(transform.position, drawColour);
    }
}
