using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugBloomSprayPrediction : MonoBehaviour
{
    [SerializeField] Transform m_source;
    [SerializeField] Weapon m_weapon;
    [SerializeField] float m_targetGroupRange = 5.0f;
    [SerializeField] float m_targetGroupRadius = 0.5f;

    List<Vector3> m_predictionDirections = new List<Vector3>();

    [SerializeField] bool m_useStrength = true;
    [SerializeField] bool m_generatePredictions = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePredictions()
    {
        m_generatePredictions = false;
        if (m_source == null || m_weapon == null)
        {
            return;
        }

        m_predictionDirections.Clear();

        for(int i = 0; i < m_weapon.bulletsPerShot; i++)
        {
            Vector3 shootDir = m_source.forward;

            Vector2 bloom = Vector2.zero;
            bloom.x = Random.value;
            bloom.y = Random.value;

            bloom.x = (bloom.x * 2.0f) - 1.0f;
            bloom.y = (bloom.y * 2.0f) - 1.0f;

            if (m_useStrength)
            {
                bloom.x *= m_weapon.bloomstrength.Evaluate(Mathf.Abs(bloom.x));
                bloom.y *= m_weapon.bloomstrength.Evaluate(Mathf.Abs(bloom.y));
            }

            bloom *= m_weapon.bloomAngle;

            Quaternion rot = Quaternion.Euler(bloom.x, bloom.y, 0.0f);
            
            shootDir = rot * shootDir;

            m_predictionDirections.Add(shootDir);
        }
    }

    private void OnValidate()
    {
        UpdatePredictions();
    }

    private void OnDrawGizmos()
    {
        if(m_source == null || m_weapon == null)
        {
            return;
        }

        Color red = Color.red;
        Gizmos.color = red;

        Gizmos.DrawWireSphere(m_source.position + m_source.forward * m_targetGroupRange, m_targetGroupRadius);
        red.a *= 0.4f;
        Gizmos.color = red;
        Gizmos.DrawSphere(m_source.position + m_source.forward * m_targetGroupRange, m_targetGroupRadius);

        Gizmos.color = Color.green;

        for (int i = 0; i < m_predictionDirections.Count; i++)
        {
            Gizmos.DrawLine(m_source.position, m_source.position + m_predictionDirections[i] * 50.0f);
        }
    }
}
