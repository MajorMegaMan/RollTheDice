using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceHolder : MonoBehaviour
{
    [SerializeField] Transform m_die1Position;
    [SerializeField] Transform m_die2Position;

    [SerializeField] ObjectiveDie m_die1;
    [SerializeField] ObjectiveDie m_die2;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadyObjectiveDice(Vector3 position)
    {
        transform.position = position;
        m_die1.InitFloating(m_die1Position.position);
        m_die2.InitFloating(m_die2Position.position);
    }
}
