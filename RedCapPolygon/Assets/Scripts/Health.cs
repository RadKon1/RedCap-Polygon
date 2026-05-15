using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealthFromInspector = 50f;
    private float _currentHealth;
    private float _maxHealth;
    private bool _isInitialized = false;

    public void InitializeHealth(float startingHealth)
    {
        _currentHealth = startingHealth;
        _maxHealth = startingHealth;
        _isInitialized = true;
    }

    private void Awake()
    {
        if (!_isInitialized)
        {
            InitializeHealth(maxHealthFromInspector);
        }
    }
    public void TakeDamage(float damageAmount)
    {
        _currentHealth -= damageAmount;

        Debug.Log($"{gameObject.name} dostał {damageAmount} dmg. Zostało HP: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (CompareTag("Player"))
        {
            Debug.Log("Gracz zginął! Ekran Game Over...");
        }
        else
        {
            PlayerManager pm = FindObjectOfType<PlayerManager>();
            if (pm != null)
            {
                pm.AddXP(25f); // 25 XP per enemy for now
                Debug.Log("Enemy defeated! Player gains 25 XP.");
            }

            Destroy(gameObject);
        }
    }
}
