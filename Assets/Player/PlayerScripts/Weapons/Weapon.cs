using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewScriptableWeapon", menuName = "ScriptableObjects/Player/Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] string m_weaponName = "newWeapon";
    [Header("Stats")]
    [SerializeField] float m_damage = 5.0f;

    [SerializeField] float m_shootFrequency = 0.2f;

    [SerializeField] int m_bulletsPerShot = 1;
    [SerializeField] int m_clipSize = 6;

    [Header("Bloom")]
    [SerializeField] float m_bloomAngle = 1.0f;

    // The chance of bloom reaching the max bloom radius
    [SerializeField] AnimationCurve m_bloomstrength = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    // The speed that coolDown is applied
    [SerializeField] float m_bloomCooldownSpeed = 1.0f;

    // The rate that cooldown speed is added
    [SerializeField] float m_bloomCooldownRate = 1.0f;
    [SerializeField] float m_bloomAddPerShot = 0.1f;

    [Header("Visual")]
    [SerializeField, Range(0.0f, 1.0f)] float m_minReticleSize = 0.2f;
    [SerializeField, Range(0.0f, 1.0f)] float m_maxReticleSize = 1.0f;

    public string weaponName { get { return m_weaponName; } }
    public float damage { get { return m_damage; } }
    public float shootFrequency { get { return m_shootFrequency; } }
    public int bulletsPerShot { get { return m_bulletsPerShot; } }
    public int clipSize { get { return m_clipSize; } }

    public float bloomAngle { get { return m_bloomAngle; } }
    public AnimationCurve bloomstrength { get { return m_bloomstrength; } }
    public float bloomCooldownSpeed { get { return m_bloomCooldownSpeed; } }
    public float bloomCooldownRate { get { return m_bloomCooldownRate; } }
    public float bloomAddPerShot { get { return m_bloomAddPerShot; } }
    public float minReticleSize { get { return m_minReticleSize; } }
    public float maxReticleSize { get { return m_maxReticleSize; } }

    public bool ShootWeapon(Transform source, List<RaycastHit> hitInfoList, LayerMask targetLayer, float recoilBloom)
    {
        hitInfoList.Clear();

        if (ShootSingle(source, out RaycastHit hitInfo, targetLayer, recoilBloom))
        {
            hitInfoList.Add(hitInfo);
        }

        for (int i = 1; i < m_bulletsPerShot; i++)
        {
            if(ShootSingle(source, out hitInfo, targetLayer, recoilBloom + (i / (float)m_bulletsPerShot)))
            {
                hitInfoList.Add(hitInfo);
            }
        }

        return hitInfoList.Count > 0;
    }

    bool ShootSingle(Transform source, out RaycastHit hitInfo, LayerMask targetLayer, float recoilBloom)
    {
        Vector3 shootDir = source.forward;

        Vector2 bloom = Vector2.zero;
        bloom.x = Random.value;
        bloom.y = Random.value;

        bloom.x = (bloom.x * 2.0f) - 1.0f;
        bloom.y = (bloom.y * 2.0f) - 1.0f;

        bloom *= recoilBloom;

        bloom.x *= bloomstrength.Evaluate(Mathf.Abs(bloom.x));
        bloom.y *= bloomstrength.Evaluate(Mathf.Abs(bloom.y));

        bloom *= bloomAngle;

        Quaternion rot = Quaternion.Euler(bloom.x, bloom.y, 0.0f);

        shootDir = rot * shootDir;

        return Physics.Raycast(source.position, shootDir, out hitInfo, float.MaxValue, targetLayer, QueryTriggerInteraction.Ignore);
    }

    bool ShootMultiple(Transform source, out RaycastHit hitInfo, LayerMask targetLayer, float recoilBloom)
    {
        Vector3 shootDir = source.forward;

        Vector2 bloom = Vector2.zero;
        bloom.x = Random.value;
        bloom.y = Random.value;

        bloom.x = (bloom.x * 2.0f) - 1.0f;
        bloom.y = (bloom.y * 2.0f) - 1.0f;

        bloom *= recoilBloom;

        bloom.x *= bloomstrength.Evaluate(Mathf.Abs(bloom.x));
        bloom.y *= bloomstrength.Evaluate(Mathf.Abs(bloom.y));

        bloom *= bloomAngle;

        Quaternion rot = Quaternion.Euler(bloom.x, bloom.y, 0.0f);

        shootDir = rot * shootDir;

        return Physics.Raycast(source.position, shootDir, out hitInfo, float.MaxValue, targetLayer, QueryTriggerInteraction.Ignore);
    }

    private void OnValidate()
    {
        if(m_minReticleSize > m_maxReticleSize)
        {
            m_maxReticleSize = m_minReticleSize;
        }
    }
}
