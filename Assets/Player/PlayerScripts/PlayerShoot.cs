using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] Weapon m_weapon = null;
    [SerializeField] LayerMask m_targetLayer = ~0;

    bool m_canShoot = true;

    float m_shootTimer = 0.0f;

    [SerializeField] string m_enemyTag = "Enemy";

    List<RaycastHit> m_hitInfoList = new List<RaycastHit>();

    private void Update()
    {
        if(!m_canShoot)
        {
            m_shootTimer += Time.deltaTime;
            if(m_shootTimer > m_weapon.shootFrequency)
            {
                m_canShoot = true;
            }
        }
    }

    public void Shoot(Transform source)
    {
        if (!m_canShoot)
        {
            return;
        }

        m_canShoot = false;
        m_shootTimer = 0.0f;

        bool hitResult = m_weapon.ShootWeapon(source, m_hitInfoList, m_targetLayer);
        if (hitResult)
        {
            for(int i = 0; i < m_hitInfoList.Count; i++)
            {
                RaycastHit hitInfo = m_hitInfoList[i];
                if (hitInfo.collider.CompareTag(m_enemyTag))
                {
                    Health enemyHealth = hitInfo.collider.gameObject.GetComponent<Health>();
                    DamageHealth(enemyHealth, hitInfo.collider);
                }
            }
        }
    }

    public void DamageHealth(Health health, Collider targetCollider)
    {
        health.Hit(m_weapon.damage, targetCollider);
    }
}
