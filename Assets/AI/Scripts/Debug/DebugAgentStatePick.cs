using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAgentStatePick : MonoBehaviour
{
    public PickerHelper[] agents;

    [System.Serializable]
    public class PickerHelper
    {
        public AIAgent agent;
        public AIStates.StateIndex state;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(var helper in agents)
        {
            helper.agent.Init();
            helper.agent.ChangeState(helper.state);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
