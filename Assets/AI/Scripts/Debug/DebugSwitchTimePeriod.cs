using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSwitchTimePeriod : MonoBehaviour
{
    public AIManager aiManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            aiManager.ChangeZoneState(AIStates.Manager.ZoneStateIndex.inside);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            aiManager.ChangeZoneState(AIStates.Manager.ZoneStateIndex.outside);
        }
    }
}
