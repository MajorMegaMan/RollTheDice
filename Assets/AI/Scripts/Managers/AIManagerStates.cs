using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using State = IState<AIManager>;

namespace AIStates.Manager
{
    public enum ZoneStateIndex
    {
        inside,
        outside
    }

    public class InsideZone : State
    {
        void State.Enter(AIManager manager)
        {
            manager.SetAgentPoolActiveInTimePeriod(true);
        }

        void State.Update(AIManager manager)
        {

        }

        void State.Exit(AIManager manager)
        {

        }
    }

    public class OutsideZone : State
    {
        void State.Enter(AIManager manager)
        {
            manager.SetAgentPoolActiveInTimePeriod(false);
        }

        void State.Update(AIManager manager)
        {

        }

        void State.Exit(AIManager manager)
        {

        }
    }
}
