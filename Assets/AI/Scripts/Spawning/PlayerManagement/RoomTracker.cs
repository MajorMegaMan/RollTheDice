using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTracker : MonoBehaviour
{
    [HideInInspector] public RoomBounds currentRoom;
    RoomBounds m_previousRoom;
    [HideInInspector] public RoomBounds previousRoom { get { return m_previousRoom; } set { m_previousRoom = value; if (value) { Debug.Log(value.name); } else { Debug.Log("null"); } } }

    public int index = 0;
    public string roomName = "";
    public string prevName = "";

    StateMachine<RoomTracker> stateMachine;

    static RoomBounds[] _allRooms;
    public static RoomBounds[] allRooms { get { return _allRooms; } }

    // Start is called before the first frame update
    void Start()
    {
        _allRooms = FindObjectsOfType<RoomBounds>();

        List<SpawnLocation> spawnLocations = new List<SpawnLocation>(FindObjectsOfType<SpawnLocation>());

        foreach (var room in _allRooms)
        {
            // Find Player's room
            if(room.EntryContainsPoint(transform.position))
            {
                currentRoom = room;
            }

            // find Spawn Locations rooms
            for(int i = 0; i < spawnLocations.Count; i++)
            {
                if(room.EntryContainsPoint(spawnLocations[i].transform.position))
                {
                    spawnLocations[i].room = room;
                    room.spawnLocations.Add(spawnLocations[i]);
                    spawnLocations.RemoveAt(i);
                    i--;
                }
            }
        }

        if(currentRoom == null)
        {
            // Player did not start in a room, should find closest
            float lowestDistance = _allRooms[0].Distance(transform.position);
            currentRoom = _allRooms[0];

            for(int  i = 1; i < _allRooms.Length; i++)
            {
                float dist = _allRooms[i].Distance(transform.position);
                if(dist < lowestDistance)
                {
                    lowestDistance = dist;
                    currentRoom = _allRooms[i];
                }
            }
        }

        previousRoom = currentRoom;

        stateMachine = new StateMachine<RoomTracker>(this);
        RoomTrackerStateBucket.SetUpStateMachine(stateMachine);
        stateMachine.Init();

        var spawnerArray = FindObjectsOfType<EnemySpawner>(true);
        foreach (EnemySpawner spawner in spawnerArray)
        {
            spawner.roomTracker = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        index = stateMachine.currentIndex;
        if(currentRoom != null)
        {
            roomName = currentRoom.name;
        }
        else
        {
            roomName = "null";
        }
        prevName = previousRoom.name;
        stateMachine.Update();
    }

    public void SetState(int stateIndex)
    {
        stateMachine.ChangeState(stateIndex);
    }

    public bool FindCurrentRoomFromPrevNeighbours()
    {
        Vector3 position = transform.position;
        // Try to find neighbour room of previous
        foreach (RoomBounds neighbour in previousRoom.neighbours)
        {
            if (neighbour.EntryContainsPoint(position))
            {
                currentRoom = neighbour;
                return true;
            }
        }

        // did not find a relevant neighbour
        return false;
    }

    public bool ValidateSpawnLocation(SpawnLocation location)
    {
        if(currentRoom != null)
        {
            // player is in a room
            return ValidateSpawnLocation(location, currentRoom);
        }
        else
        {
            // player is somehow outside all rooms
            return ValidateSpawnLocation(location, previousRoom);
        }
    }

    static bool ValidateSpawnLocation(SpawnLocation location, RoomBounds targetRoom)
    {
        if(location.room == targetRoom)
        {
            // spawn location is in current room
            return true;
        }
        
        foreach(var neighbour in targetRoom.neighbours)
        {
            if(location.room == neighbour)
            {
                // spawn location is in neighbour room
                return true;
            }
        }

        return false;
    }

    private void OnDestroy()
    {
        _allRooms = null;
    }
}
