using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Player stats
    private double currentHealth = 100.0f;
    private double maxHealth = 100.0f;
    private double defense = 3.0f;
    private double luck = 1.0f;
    private double experiencePoints = 0.0f;

    // Combat stats
    private double attackDamage = 10.0f;
    private double criticalChance = 0.1f;


    // Game-Logic data
    private double invincibilityDuration = 0.5f;

}
