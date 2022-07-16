using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableWeapon", menuName = "ScriptableObjects/Player/Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] string m_weaponName = "newWeapon";
    [SerializeField] float m_damage = 5.0f;

    [SerializeField] float m_shootFrequency = 0.2f;

    [SerializeField] int m_bulletsPerShot = 1;

    // Measurement is at a range of 5.0f, 1.0f unit of bloom == 1.0 unit in world space.
    //
    //    \             /
    //     \           /
    //      \ 1 unit  /
    //       \_______/ range 5
    //        \     /
    //         \   /
    //          \ /
    //           |
    //           o Shooter
    [SerializeField] float m_bloomRadius = 1.0f;

    // The chance of bloom reaching the max bloom radius
    [SerializeField] float m_bloomstrength = 0.2f;

    public float damage { get { return m_damage; } }
    public float shootFrequency { get { return m_shootFrequency; } }
    public int bulletsPerShot { get { return m_bulletsPerShot; } }

    public bool ShootWeapon(Transform source, List<RaycastHit> hitInfoList, LayerMask targetLayer)
    {
        hitInfoList.Clear();
        for (int i = 0; i < m_bulletsPerShot; i++)
        {
            if (Physics.Raycast(source.position, source.forward, out RaycastHit hitInfo, float.MaxValue, targetLayer, QueryTriggerInteraction.Ignore))
            {
                hitInfoList.Add(hitInfo);
            }
        }

        return hitInfoList.Count > 0;
    }
}
