using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Scriptables/AISettings")]
public class AISettings : ScriptableObject
{
    [Header("Type")]
    public EnemyType enemyType;

    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float angularSpeed = 120.0f;
    public float acceleration = 8.0f;

    [Header("Gameplay")]
    public float health = 30;
    public float headMultiplier = 1.5f;
    public float upperTorsoMultipler = 1.0f;
    public float lowerTorsoMultiplier = 0.9f;
    public float limbMultiplier = 0.7f;

    [Header("Idle")]
    public float maxIdleTime = 2.0f;
    public float minIdleTime = 1.0f;

    [Header("Wander")]
    public float stoppingDistance = 0.5f;

    [Header("Aggro")]
    public float aggresionRadius = 4.0f;
    public float orbWalkRadius = 1.0f;
    public float attackChargeRadius = 1.0f;
    public float attackChargeMax = 1.0f;
    public float attackChargeRate = 1.0f;

    [Header("Attack")]
    public float attackDamage = 10.0f;
    public float rotationLerpSpeed = 0.1f;
    public float afterAttackLerpSpeed = 0.01f;

    [Header("Death")]
    public float bodyDecayTime = 5.0f;
    public string dissolveShaderEffect;
    public float dissolveTime = 1.0f;

    [Header("Animation")]
    public AnimationCurve moveAnimSpeed;

    [Header("Vaulting")]
    public float vaultMinDistanceCheck = 0.5f;
    public float vaultgroundCheckOffset = 0.1f;
    public float vaultStartSensitivity = 1.0f;
    public float vaultEndSensitivity = 1.0f;
    public LayerMask groundCheckLayer = ~0;
    public float vaultInitialFallVelocity = 0.0f;
    public float vaultStartforwardOffset = 0.0f;

    [Header("Audio")]
    public float hurtDelay = 0.1f;
    public float deathDelay = 0.1f;

    void StabiliseExistingEnemyTypeObjects(EnemyType enemyType)
    {
        var managers = FindObjectsOfType<AIManager>();
        foreach (var manager in managers)
        {
            foreach (var agent in manager.activeAgents)
            {
                if (agent.settings.enemyType == enemyType)
                {
                    agent.StabiliseSettings();
                }
            }
        }
    }

    private void OnValidate()
    {
        if(Application.isPlaying)
        {
            StabiliseExistingEnemyTypeObjects(enemyType);
        }
    }
}
