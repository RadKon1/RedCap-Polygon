using UnityEngine;

[CreateAssetMenu( menuName = "PlayerCombat")]

public class PlayerCombatStats : ScriptableObject
{
    [Header("Light Attack")]
    [Range(1, 100)] public int LightAttackDamage = 10;
    [Range(0.1f, 1.5f)] public float LightAttackRate = 0.3f;
    [Range(0.1f, 5f)] public float LightAttackHitBox = 1.2f;

    [Header("Heavy Attack")]
    [Range(10, 300)] public int HeavyAttackDamage = 35;
    [Range(0.1f, 2f)] public float HeavyAttackRate = 1f;
    [Range(0.1f, 7f)] public float HeavyAttackHitBox = 2f;

    [Header("Dash Attack")]
    [Range(1, 100)] public int DashAttackDamage = 15;
    [Range(0.1f, 2f)] public float DashAttackRate = 1.5f;
    [Range(0.1f, 7f)] public float DashAttackHitBox = 3f;
    [Range(0.01f, 0.3f)] public float DashInvincibilityTime = 0.1f;

    [Header("Combat Physics")]
    [Range(0f, 100f)] public float KnockbackForce = 15f;
    [Range(0.01f, 0.3f)] public float HitStopTime = 0.05f;
}
