using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Weapondecal : MonoBehaviour
{
    [SerializeField] float m_destroyTime = 5.0f;
    float m_timer = 0.0f;

    private void Start()
    {
        m_timer = 0.0f;
    }

    private void Update()
    {
        m_timer += Time.deltaTime;
        if(m_timer > m_destroyTime)
        {
            Destroy(gameObject);
        }
    }
}
