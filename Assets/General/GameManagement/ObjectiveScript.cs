using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectiveScript : MonoBehaviour
{
    [SerializeField] List<Objective> m_objectives;

    // Called when a kill is logged.
    [SerializeField] UnityEvent m_logKillEvent;

    // Called when this is updated.
    [SerializeField] UnityEvent m_tickEvent;

    Objective m_currentObjective = null;

    int m_killCount = 0;
    float m_timer = 0.0f;

    public float killcount { get { return m_killCount; } }
    public float timer { get { return m_timer; } }

    public UnityEvent logKillEvent { get { return m_logKillEvent; } }
    public UnityEvent tickEvent { get { return m_tickEvent; } }

    // Start is called before the first frame update
    void Start()
    {
        
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
        if(m_currentObjective.IsObjectiveComplete(this))
        {
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
    }

    public void EndCurrentObjective()
    {
        m_currentObjective.DiconnectObjective(this);
        m_currentObjective = null;
    }
}
