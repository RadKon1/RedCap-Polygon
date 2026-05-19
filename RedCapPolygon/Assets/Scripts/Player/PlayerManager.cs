using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats _stats;
    private Health _health;

    private float _currentXP;
    private int _currentLevel;

    private void Awake()
    {
        // initializing health based on stats
        _health = GetComponent<Health>();
        // reset number of coins on game start
        _stats.NumberOfCoins = 0;
        UpdatePlayerHealth();

        _currentLevel = 1;
    }
    
    private void UpdatePlayerHealth()
    {
        //float newMax = _stats.BaseMaxHealth + (_stats.HealthPerLevel * (_currentLevel - 1));
        //_health.InitializeHealth(newMax);
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

        UpdatePlayerHealth();
        Debug.Log($"Level Up! Current Level: {_currentLevel}");
    }
}
