using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillcountObjective : Objective
{
    [SerializeField] int m_killCount = 5;
    

    public override bool IsObjectiveComplete(ObjectiveScript objectiveScript)
    {
        return objectiveScript.killcount >= m_killCount;
    }

    public override void ConnectObjective(ObjectiveScript objectiveScript)
    {
        objectiveScript.logKillEvent.AddListener(objectiveScript.ProcessObjective);
    }

    public override void DiconnectObjective(ObjectiveScript objectiveScript)
    {
        objectiveScript.logKillEvent.RemoveListener(objectiveScript.ProcessObjective);
    }

    public override void OnSuccess(ObjectiveScript objectiveScript)
    {

    }
}
