using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoll : MonoBehaviour
{
    [SerializeField] Transform m_targetBounds;
    [SerializeField] Vector3 m_boundsSize = new Vector3(10.0f, 0.0f, 10.0f);

    Rigidbody m_rigidbody;

    [SerializeField] Vector3 m_initalForce = Vector3.zero;

    [SerializeField] bool debugRoll = false;

    // Start is called before the first frame update
    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        AddForce(m_initalForce);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddForce(Vector3 force)
    {
        m_rigidbody.AddForce(force, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        Vector3 position = transform.position - m_targetBounds.position - m_boundsSize;
        position.x = Mathf.Repeat(position.x, m_boundsSize.x * 2);
        position.z = Mathf.Repeat(position.z, m_boundsSize.z * 2);

        position += m_targetBounds.position - m_boundsSize;
        transform.position = position;
    }

    private void OnValidate()
    {
        if(Application.isPlaying)
        {
            if(debugRoll)
            {
                debugRoll = false;
                AddForce(m_initalForce);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 position = transform.position - m_targetBounds.position - m_boundsSize;
        position.x = Mathf.Repeat(position.x, m_boundsSize.x * 2);
        position.z = Mathf.Repeat(position.z, m_boundsSize.z * 2);

        position += m_targetBounds.position - m_boundsSize;

        Gizmos.DrawCube(position, transform.localScale);
    }
}
