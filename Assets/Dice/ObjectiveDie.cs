using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectiveDie : MonoBehaviour
{
    Rigidbody m_rigidbody;

    [SerializeField] Vector3 m_initalForce = Vector3.zero;

    [SerializeField] bool debugRoll = false;

    List<float> m_dotSides;

    // How many times is the player aloowed to "Roll" the dice
    [SerializeField] int m_availableTouchCount = 5;
    int m_touchCount = 0;

    private void Awake()
    {
        m_dotSides = new List<float>();
        m_rigidbody = GetComponent<Rigidbody>();
        Float();
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 randomDir = Vector3.zero;
        randomDir.x = (Random.value * 2) - 1;
        randomDir.y = (Random.value * 2) - 1;
        randomDir.z = (Random.value * 2) - 1;

        randomDir.x *= m_initalForce.x;
        randomDir.y *= m_initalForce.y;
        randomDir.z *= m_initalForce.z;

        m_rigidbody.AddTorque(randomDir, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void Float()
    {
        m_rigidbody.useGravity = false;
    }

    public void Roll(Vector3 force)
    {
        m_rigidbody.useGravity = true;
        m_rigidbody.AddTorque(force, ForceMode.Impulse);
        m_rigidbody.AddForce(force, ForceMode.Impulse);
        m_touchCount++;
    }

    public void Roll(Vector3 hitPoint, Vector3 force)
    {
        m_rigidbody.useGravity = true;

        // Too much force added. But whatevs.
        m_rigidbody.AddTorque(force, ForceMode.Impulse);
        m_rigidbody.AddForce(force, ForceMode.Impulse);
        m_rigidbody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
        m_touchCount++;
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
                Roll(m_initalForce);
            }
        }
    }
}
