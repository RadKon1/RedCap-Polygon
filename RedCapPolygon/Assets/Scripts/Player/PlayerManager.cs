using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats _stats;

    private float _currentHealth;
    private float _maxHealth;
    private float _currentXP;
    private int _currentLevel;

    private void Awake()
    {
        _maxHealth = _stats.BaseMaxHealth;
        _currentHealth = _maxHealth;
        _currentLevel = 1;
    }

    public void AddXP(float amount)
    {
        _currentXP += amount;
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        float requiredXP = _stats.XpToLevelUp * Mathf.Pow(_stats.XpMultiplier, _currentLevel - 1);

        if (_currentXP >= requiredXP)
        {
            LevelUp(requiredXP);
        }
    }

    private void LevelUp(float usedXP)
    {
        _currentLevel++;
        _currentXP -= usedXP;

        _maxHealth += _stats.HealthPerLevel;
        _currentHealth = _maxHealth; // Full heal

        Debug.Log($"Level Up! Obecny poziom: {_currentLevel}");
    }
}
