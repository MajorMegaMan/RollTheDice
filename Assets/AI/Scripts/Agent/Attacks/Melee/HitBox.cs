using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public AIAgent attachedAgent;
    [HideInInspector]public bool hitIsActive = false;

    private void OnTriggerEnter(Collider other)
    {
        HitPlayer(other);
    }

    private void OnTriggerStay(Collider other)
    {
        HitPlayer(other);
    }

    void HitPlayer(Collider other)
    {
        if (hitIsActive)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Hit Player");
                attachedAgent.DamagePlayer();
                hitIsActive = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(hitIsActive)
        {
            Debug.Log(collision.collider.name);
            if (collision.gameObject.CompareTag("Player"))
            {
                attachedAgent.DamagePlayer();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(hitIsActive)
        {
            BoxCollider collider = GetComponent<BoxCollider>();

            Vector3 scale = new Vector3();
            scale = collider.size;
            //scale.x = collider.radius;
            //scale.y = collider.height;
            //scale.z = collider.radius;

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.red;
            Gizmos.DrawCube(collider.center, scale);
        }
    }
}
