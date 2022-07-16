using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewScriptableWeapon", menuName = "ScriptableObjects/Player/Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] string m_weaponName = "newWeapon";
    [SerializeField] float m_damage = 5.0f;

    [SerializeField] float m_shootFrequency = 0.2f;

    [SerializeField] int m_bulletsPerShot = 1;

    // 
    [SerializeField] float m_bloomAngle = 1.0f;

    // The chance of bloom reaching the max bloom radius
    [SerializeField] AnimationCurve m_bloomstrength = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    public float damage { get { return m_damage; } }
    public float shootFrequency { get { return m_shootFrequency; } }
    public int bulletsPerShot { get { return m_bulletsPerShot; } }

    public float bloomAngle { get { return m_bloomAngle; } }
    public AnimationCurve bloomstrength { get { return m_bloomstrength; } }

    public bool ShootWeapon(Transform source, List<RaycastHit> hitInfoList, LayerMask targetLayer)
    {
        hitInfoList.Clear();

        if (ShootSingle(source, out RaycastHit hitInfo, targetLayer))
        {
            hitInfoList.Add(hitInfo);
        }

        for (int i = 1; i < m_bulletsPerShot; i++)
        {
            if(ShootSingle(source, out hitInfo, targetLayer))
            {
                hitInfoList.Add(hitInfo);
            }
        }

        return hitInfoList.Count > 0;
    }

    bool ShootSingle(Transform source, out RaycastHit hitInfo, LayerMask targetLayer)
    {
        Vector3 shootDir = source.forward;

        Vector2 bloom = Vector2.zero;
        bloom.x = Random.value;
        bloom.y = Random.value;

        bloom.x = (bloom.x * 2.0f) - 1.0f;
        bloom.y = (bloom.y * 2.0f) - 1.0f;

        bloom.x *= bloomstrength.Evaluate(Mathf.Abs(bloom.x));
        bloom.y *= bloomstrength.Evaluate(Mathf.Abs(bloom.y));

        bloom *= bloomAngle;

        Quaternion rot = Quaternion.Euler(bloom.x, bloom.y, 0.0f);

        shootDir = rot * shootDir;

        return Physics.Raycast(source.position, shootDir, out hitInfo, float.MaxValue, targetLayer, QueryTriggerInteraction.Ignore);
    }
}
