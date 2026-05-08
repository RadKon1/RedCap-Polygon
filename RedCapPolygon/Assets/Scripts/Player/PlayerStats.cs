using UnityEngine;

[CreateAssetMenu(menuName = "PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Health Settings")]
    public float BaseMaxHealth = 100.0f;
    public float HealthPerLevel = 20.0f;

    [Header("Defense & Luck")]
    public float BaseDefense = 3.0f;
    public float BaseLuck = 1.0f;

    [Header("Leveling System")]
    public float XpToLevelUp = 100f;
    [Range(1f, 2f)] public float XpMultiplier = 1.2f;
}
