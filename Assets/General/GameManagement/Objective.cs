using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Objective : ScriptableObject
{
    [SerializeField] string objectiveName = "newObjective";

    public abstract bool IsObjectiveComplete(ObjectiveScript objectiveScript);

    public abstract void ConnectObjective(ObjectiveScript objectiveScript);

    public abstract void DiconnectObjective(ObjectiveScript objectiveScript);

    public abstract void OnSuccess(ObjectiveScript objectiveScript);
}
