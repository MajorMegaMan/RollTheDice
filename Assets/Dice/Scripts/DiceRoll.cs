using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoll : MonoBehaviour
{
    Rigidbody m_rigidbody;

    [SerializeField] float m_rollForce = 1.0f;

    [SerializeField] bool debugRoll = false;

    Vector3 m_lookDir = Vector3.zero;

    bool m_isRolling = false;

    [SerializeField] float m_rollTime = 0.5f;

    float m_rollTimer = 0.0f;

    // Start is called before the first frame update
    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        Roll();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isRolling)
        {
            m_rigidbody.angularVelocity = Vector3.Lerp(m_rigidbody.angularVelocity, Vector3.zero, 0.1f);
            transform.forward = Vector3.Slerp(transform.forward, m_lookDir, 0.1f);
        }
        else
        {
            m_rollTimer += Time.deltaTime;
            if(m_rollTimer > m_rollTime)
            {
                int rand = Random.Range(0, 5);
                switch(rand)
                {
                    case 0:
                        {
                            StopRolling(Vector3.up);
                            break;
                        }
                    case 1:
                        {
                            StopRolling(Vector3.right);
                            break;
                        }
                    case 2:
                        {
                            StopRolling(Vector3.forward);
                            break;
                        }
                    case 3:
                        {
                            StopRolling(-Vector3.up);
                            break;
                        }
                    case 4:
                        {
                            StopRolling(-Vector3.right);
                            break;
                        }
                    case 5:
                        {
                            StopRolling(-Vector3.forward);
                            break;
                        }
                }
            }
        }
    }

    private void FixedUpdate()
    {

    }

    public void Roll()
    {
        Vector3 thing = Vector3.one;
        thing.x *= (Random.value * 2) - 1;
        thing.y *= (Random.value * 2) - 1;
        thing.z *= (Random.value * 2) - 1;
        m_rigidbody.AddTorque(thing * m_rollForce, ForceMode.Impulse);
        m_isRolling = false;
    }

    public void StopRolling(Vector3 lookDir)
    {
        m_lookDir = lookDir;
        m_isRolling = true;
    }

    private void OnValidate()
    {
        if(Application.isPlaying)
        {
            if(debugRoll)
            {
                debugRoll = false;
                Roll();
            }
        }
    }
}
