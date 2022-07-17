using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Scriptables/RocketProjectileSettings")]
public class RocketProjectileSettings : ScriptableObject
{
    public float lifeTime = 5.0f;

    public float initialForce = 5.0f;
}
