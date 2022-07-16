using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileSettings settings;

    float m_timer = 0.0f;

    Rigidbody m_body;

    [HideInInspector] public AIAgent owner;

    float m_initialForce = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_body = GetComponent<Rigidbody>();
        m_body.velocity = transform.forward * m_initialForce;
        m_timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_timer += Time.deltaTime;
        //transform.position += transform.forward * speed * Time.deltaTime;

        if(m_timer > settings.lifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void Shoot(AIAgent owner, Vector3 origin, Vector3 launchVector, float force)
    {
        this.owner = owner;
        transform.position = origin;
        transform.LookAt(origin + launchVector);
        m_initialForce = force;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            owner.DamagePlayer();
            Destroy(gameObject);
        }
    }
}
