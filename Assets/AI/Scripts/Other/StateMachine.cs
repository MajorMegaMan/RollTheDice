using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    T objRef;
    List<IState<T>> m_states = new List<IState<T>>();

    IState<T> m_currentState;
    int m_currentIndex = 0;

    public int currentIndex { get { return m_currentIndex; } }

    public StateMachine(T obj)
    {
        objRef = obj;
    }

    public bool Init()
    {
        if(m_states.Count > 0)
        {
            m_currentState = m_states[0];
            m_currentState.Enter(objRef);
            m_currentIndex = 0;
            return true;
        }
        return false;
    }

    public void AddState(IState<T> newState)
    {
        m_states.Add(newState);
    }

    public void ChangeState(int index)
    {
        m_currentState.Exit(objRef);

        m_currentState = m_states[index];
        m_currentIndex = index;

        m_currentState.Enter(objRef);
    }

    public void Update()
    {
        m_currentState.Update(objRef);
    }
}

public interface IState<T>
{
    void Enter(T obj);

    void Exit(T obj);

    void Update(T obj);
}
