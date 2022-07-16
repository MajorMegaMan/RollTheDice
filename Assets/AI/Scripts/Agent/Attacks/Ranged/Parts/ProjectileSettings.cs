using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Scriptables/ProjectileSettings")]
public class ProjectileSettings : ScriptableObject
{
    public float animationShootTime = 1.7f;

    public float lifeTime = 5.0f;
    [Header("Arc Control")]
    public Vector3 targetOffset = Vector3.up * 1.8f;
    public float controlTime = 5.0f;
    public float controlExponent = 2.0f;

    public float GetDynamicTime(float magnitude)
    {
        float result = magnitude / Mathf.Pow(controlTime, controlExponent);
        return result;
    }
}
