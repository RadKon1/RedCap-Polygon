using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealthFromInspector = 50f;
    private float _currentHealth;
    private float _maxHealth;
    private bool _isInitialized = false;
    private bool _isDead = false;

    public void InitializeHealth(float startingHealth)
    {
        _currentHealth = startingHealth;
        _maxHealth = startingHealth;
        _isInitialized = true;
        _isDead = false;
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
        if (_isDead) return;

        _currentHealth -= damageAmount;

        Debug.Log($"{gameObject.name} dostał {damageAmount} dmg. Zostało HP: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        if (gameObject.CompareTag("Player") || GetComponent<PlayerManager>() != null)
        {
            Debug.Log("POTWIERDZONE: Gracz zginął! Ładowanie sceny: GameOverScene");
            SceneManager.LoadScene("GameOverScene");
        }
        else
        {
            Debug.Log($"Przeciwnik {gameObject.name} pokonany. Przyznawanie XP graczowi.");

            PlayerManager pm = FindObjectOfType<PlayerManager>();
            if (pm != null)
            {
                pm.AddXP(25f);
            }

            Destroy(gameObject);
        }
    }
}
