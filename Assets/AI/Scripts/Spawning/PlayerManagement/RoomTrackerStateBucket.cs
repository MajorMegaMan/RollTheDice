using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoomTrackerStateBucket
{
    public static void SetUpStateMachine(StateMachine<RoomTracker> target)
    {
        target.AddState(new InitialState());
        target.AddState(new FindStateDirection());
        target.AddState(new InRoom());
        target.AddState(new TryFindNeighbour());
        target.AddState(new TryFindEnter());
    }
}

enum RoomStateIndex
{
    initial,
    directional,
    inRoom,
    findNeighbour,
    findEnter
}


public class InitialState : IState<RoomTracker>
{
    void IState<RoomTracker>.Enter(RoomTracker tracker)
    {
        if(tracker.currentRoom != null)
        {
            tracker.SetState((int)RoomStateIndex.inRoom);
        }
        else
        {
            tracker.SetState((int)RoomStateIndex.directional);
        }
    }

    void IState<RoomTracker>.Update(RoomTracker tracker)
    {
        // We should never have reached here, but just in case set the state to search for rooms
        tracker.SetState((int)RoomStateIndex.findNeighbour);
    }

    void IState<RoomTracker>.Exit(RoomTracker tracker)
    {

    }
}

public class FindStateDirection : IState<RoomTracker>
{
    void IState<RoomTracker>.Enter(RoomTracker tracker)
    {
        // Try to find neighbour room of previous first
        if (tracker.FindCurrentRoomFromPrevNeighbours())
        {
            Debug.Log("Found Neighbour: instantly");
            tracker.SetState((int)RoomStateIndex.inRoom);
            return;
        }
        else
        {
            // If we get here we did not enter another room
            // now we should try to find the neighbour room repeatedly for a small amount of time
            tracker.SetState((int)RoomStateIndex.findNeighbour);
        }
    }

    void IState<RoomTracker>.Update(RoomTracker tracker)
    {
        // We should never have reached here, but just in case set the state to search for rooms
        tracker.SetState((int)RoomStateIndex.findNeighbour);
    }

    void IState<RoomTracker>.Exit(RoomTracker tracker)
    {

    }
}

// 1
public class InRoom : IState<RoomTracker>
{
    void IState<RoomTracker>.Enter(RoomTracker tracker)
    {
        tracker.currentRoom.ProcessEnterEvent();
    }

    void IState<RoomTracker>.Update(RoomTracker tracker)
    {
        Vector3 position = tracker.transform.position;
        // check if the player has exited the current room
        if (!tracker.currentRoom.ExitContainsPoint(position))
        {
            Debug.Log("Exit room");
            tracker.SetState((int)RoomStateIndex.directional);// Change to Find State
        }
    }

    void IState<RoomTracker>.Exit(RoomTracker tracker)
    {
        tracker.currentRoom.ProcessExitEvent();

        tracker.previousRoom = tracker.currentRoom;
        tracker.currentRoom = null;
    }
}

// 2
public class TryFindNeighbour : IState<RoomTracker>
{
    int tryCount = 0;

    void IState<RoomTracker>.Enter(RoomTracker tracker)
    {
        tryCount = 0;
    }

    void IState<RoomTracker>.Update(RoomTracker tracker)
    {
        if (tryCount > 60)
        {
            Debug.Log("Could not find neighbour");
            tracker.SetState((int)RoomStateIndex.findEnter);
        }

        // Try to find neighbour room of previous
        if (tracker.FindCurrentRoomFromPrevNeighbours())
        {
            Debug.Log("Found current from prev neighbours");
            tracker.SetState((int)RoomStateIndex.inRoom);
        }
        tryCount++;
    }

    void IState<RoomTracker>.Exit(RoomTracker tracker)
    {

    }
}

// 3
public class TryFindEnter : IState<RoomTracker>
{
    RoomBounds enterRoom = null;

    void IState<RoomTracker>.Enter(RoomTracker tracker)
    {
        enterRoom = null;
    }

    void IState<RoomTracker>.Update(RoomTracker tracker)
    {
        foreach(var room in RoomTracker.allRooms)
        {
            if(room.EntryContainsPoint(tracker.transform.position))
            {
                Debug.Log("Found new room from all");
                enterRoom = room;
                tracker.SetState((int)RoomStateIndex.inRoom);
            }
        }
    }

    void IState<RoomTracker>.Exit(RoomTracker tracker)
    {
        tracker.currentRoom = enterRoom;
    }
}
