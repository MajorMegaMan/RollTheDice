using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] Weapon m_weapon = null;
    [SerializeField] LayerMask m_targetLayer = ~0;
    [SerializeField] string m_objectiveDieTag = "ObjectiveDie";
    [SerializeField] Weapondecal m_decalPrefab;
    [SerializeField] PlayerHUD m_playerHUD = null;

    [SerializeField] List<Weapon> m_weaponArray;
    int m_currentWeaponIndex = 0;

    bool m_canShoot = true;

    float m_shootTimer = 0.0f;
    float m_coolDownCoefficient = 1.0f;

    float m_recoilBloom = 0.0f;

    [SerializeField] string m_enemyTag = "Enemy";

    List<RaycastHit> m_hitInfoList = new List<RaycastHit>();

    int m_clipCount = 0;

    private void Start()
    {
        SetWeapon(m_weapon);
    }

    public void SetWeapon(Weapon weapon)
    {
        m_weapon = weapon;
        if (m_playerHUD != null)
        {
            m_clipCount = m_weapon.clipSize;
            m_playerHUD.UpdateWeapon(weapon);
        }
    }

    public void SetPlayerHUD(PlayerHUD playerHUD)
    {
        m_playerHUD = playerHUD;
        SetWeapon(m_weapon);
    }

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

        m_recoilBloom -= Time.deltaTime * m_weapon.bloomCooldownSpeed * m_coolDownCoefficient;
        if(m_recoilBloom > 0.0)
        {
            m_coolDownCoefficient += Time.deltaTime * m_weapon.bloomCooldownRate;
        }
        else
        {
            m_recoilBloom = 0.0f;
        }
        m_playerHUD.SetReticleBloom(m_recoilBloom);
    }

    public void Shoot(Transform source)
    {
        if (!m_canShoot)
        {
            return;
        }

        m_canShoot = false;
        m_shootTimer = 0.0f;

        if (m_clipCount == 0)
        {
            // Trigger Roll Reload
            RollReload();
            return;
        }

        m_clipCount--;
        m_playerHUD.SetClipCount(m_clipCount);

        bool hitResult = m_weapon.ShootWeapon(source, m_hitInfoList, m_targetLayer, m_recoilBloom);
        m_recoilBloom += m_weapon.bloomAddPerShot;
        m_recoilBloom = Mathf.Min(1.0f, m_recoilBloom);
        m_coolDownCoefficient = 1.0f;

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
                else if(hitInfo.collider.CompareTag(m_objectiveDieTag))
                {
                    var die = hitInfo.collider.GetComponent<ObjectiveDie>();
                    if(die.Roll(hitInfo.point, source.forward * m_weapon.physicsForce))
                    {

                    }
                }
                else
                {
                    CreateDecal(source.forward, hitInfo.point);
                }
            }
        }
    }

    // returns true if target layer matches one of include layer
    bool CompareLayers(LayerMask targetLayer, LayerMask includeLayer)
    {
        return ((1 << targetLayer) & includeLayer) != 0;
    }

    public void DamageHealth(Health health, Collider targetCollider)
    {
        health.Hit(m_weapon.damage, targetCollider);
    }

    public void CreateDecal(Vector3 shootDir, Vector3 hitPoint)
    {
        Weapondecal decal = Instantiate(m_decalPrefab);
        decal.transform.position = hitPoint - shootDir;
        decal.transform.LookAt(hitPoint);
    }

    public void RollReload()
    {
        int value = Random.Range(0, 5);
        m_currentWeaponIndex = value % m_weaponArray.Count;
        SetWeapon(m_weaponArray[value % m_weaponArray.Count]);
    }

    private void OnValidate()
    {
        SetPlayerHUD(m_playerHUD);
    }
}
