using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class ObjectiveDie : MonoBehaviour
{
    Rigidbody m_rigidbody;

    [SerializeField] UnityEvent m_firstRollEvent;

    [SerializeField] Vector3 m_initalForce = Vector3.zero;
    [SerializeField] float m_maxRestingSpeed = 0.2f;

    // The time it takes for the dice to be considered resting
    [SerializeField] float m_restTime = 2.0f;

    float m_restTimer = 0.0f;

    bool m_isResting = false;

    [SerializeField] UnityEvent m_enterRestEvent;

    List<float> m_dotSides;

    [SerializeField] bool debugRoll = false;

    // How many times is the player aloowed to "Roll" the dice
    [SerializeField] int m_availableTouchCount = 5;
    int m_touchCount = 0;

    // These dice will also roll when this is rolled.
    [SerializeField] List<ObjectiveDie> m_buddyDice;

    private void Awake()
    {
        m_dotSides = new List<float>();
        m_rigidbody = GetComponent<Rigidbody>();
        Float();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitFloating(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isResting)
        {
            return;
        }

        if(m_rigidbody.useGravity)
        {
            if(m_rigidbody.angularVelocity.magnitude < m_maxRestingSpeed)
            {
                m_restTimer += Time.deltaTime;
                if (m_restTimer > m_restTime)
                {
                    EnterRest();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        
    }

    public void InitFloating(Vector3 position)
    {
        Float();

        Vector3 randomDir = Vector3.zero;
        randomDir.x = (Random.value * 2) - 1;
        randomDir.y = (Random.value * 2) - 1;
        randomDir.z = (Random.value * 2) - 1;

        randomDir.x *= m_initalForce.x;
        randomDir.y *= m_initalForce.y;
        randomDir.z *= m_initalForce.z;

        m_rigidbody.AddTorque(randomDir, ForceMode.Impulse);
    }

    public void Float()
    {
        m_isResting = false;
        m_rigidbody.useGravity = false;
    }

    void EnterRest()
    {
        m_touchCount = m_availableTouchCount;
        m_enterRestEvent.Invoke();
    }

    // Returns true if the die was rolled.
    public bool Roll(Vector3 hitPoint, Vector3 force)
    {
        if(SelfishRoll(hitPoint, force))
        {
            for(int i = 0; i < m_buddyDice.Count; i++)
            {
                m_buddyDice[i].SelfishRoll(hitPoint, force);
            }
        }
        return false;
    }

    // Returns true if the die was rolled.
    bool SelfishRoll(Vector3 hitPoint, Vector3 force)
    {
        if (m_touchCount >= m_availableTouchCount)
        {
            return false;
        }

        if (m_rigidbody.useGravity)
        {
            m_firstRollEvent.Invoke();
        }

        m_rigidbody.useGravity = true;

        // Too much force added. But whatevs.
        m_rigidbody.AddTorque(force, ForceMode.Impulse);
        m_rigidbody.AddForce(force, ForceMode.Impulse);
        m_rigidbody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
        m_touchCount++;

        return true;
    }

    public int GetUpSideIndex()
    {
        m_dotSides.Clear();

        m_dotSides.Add(Vector3.Dot(Vector3.up, transform.up));
        m_dotSides.Add(Vector3.Dot(Vector3.up, transform.right));
        m_dotSides.Add(Vector3.Dot(Vector3.up, transform.forward));
        m_dotSides.Add(Vector3.Dot(Vector3.up, -transform.up));
        m_dotSides.Add(Vector3.Dot(Vector3.up, -transform.right));
        m_dotSides.Add(Vector3.Dot(Vector3.up, -transform.forward));

        int highestDot = 0;
        for(int i = 1; i < 6; i++)
        {
            if(m_dotSides[i] > m_dotSides[highestDot])
            {
                highestDot = i;
            }
        }

        return highestDot;
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            if (debugRoll)
            {
                debugRoll = false;
                Roll(transform.position, m_initalForce);
            }
        }
    }
}
