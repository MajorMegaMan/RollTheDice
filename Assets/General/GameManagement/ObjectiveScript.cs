using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectiveScript : MonoBehaviour
{
    [SerializeField] List<Objective> m_objectives;

    [SerializeField] List<EnemySpawner> m_spawners;

    [SerializeField] AIManager m_aiManager;
    [SerializeField] RoomTracker m_playerRoomTracker;
    [SerializeField] DiceHolder m_diceHolder;

    [SerializeField] List<RoomDiceLocationPair> m_diceLocationRooms;

    // Called when a kill is logged.
    [SerializeField] UnityEvent m_logKillEvent;

    // Called when this is updated.
    [SerializeField] UnityEvent m_tickEvent;

    Objective m_currentObjective = null;

    int m_killCount = 0;
    float m_timer = 0.0f;

    public List<EnemySpawner> spawners { get { return m_spawners; } }

    public float killcount { get { return m_killCount; } }
    public float timer { get { return m_timer; } }

    public UnityEvent logKillEvent { get { return m_logKillEvent; } }
    public UnityEvent tickEvent { get { return m_tickEvent; } }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_spawners.Count; i++)
        {
            m_spawners[i].Init();
        }
        ActivateSpawners(false);
    }

    // Update is called once per frame
    void Update()
    {
        m_timer += Time.deltaTime;
        m_tickEvent.Invoke();
    }

    public void LogKill()
    {
        m_killCount++;
        m_logKillEvent.Invoke();
    }

    public void ProcessObjective()
    {
        Debug.Log("Killed Someone");
        if(m_currentObjective.IsObjectiveComplete(this))
        {
            Debug.Log("I won");
            m_currentObjective.OnSuccess(this);
            EndCurrentObjective();
        }
        else
        {

        }
    }

    public void StartObjective(int index)
    {
        index = index % m_objectives.Count;
        m_currentObjective = m_objectives[index];
        m_currentObjective.ConnectObjective(this);

        m_killCount = 0;
        m_timer = 0.0f;

        ActivateSpawners(true);
    }

    public void EndCurrentObjective()
    {
        m_currentObjective.DiconnectObjective(this);
        m_currentObjective = null;

        ActivateSpawners(false);

        m_diceHolder.ReadyObjectiveDice(FindNextDiceLocation());
    }

    public void ActivateSpawners(bool isActive)
    {
        for(int i = 0; i < m_spawners.Count; i++)
        {
            m_spawners[i].gameObject.SetActive(isActive);
        }
    }

    public Vector3 FindNextDiceLocation()
    {
        for(int i = 0; i < m_diceLocationRooms.Count; i++)
        {
            if(m_diceLocationRooms[i].m_roomBounds == m_playerRoomTracker.currentRoom)
            {
                return m_diceLocationRooms[i].m_diceLocations.position;
            }
        }

        return m_diceLocationRooms[0].m_diceLocations.position;
    }
}

[System.Serializable]
struct RoomDiceLocationPair
{
    public RoomBounds m_roomBounds;
    public Transform m_diceLocations;
}
