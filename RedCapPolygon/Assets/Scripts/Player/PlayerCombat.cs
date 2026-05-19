using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public PlayerCombatStats CombatStats;

    [SerializeField] private LayerMask _enemyLayers;
    [SerializeField] private Transform _attackPoint;
    private Animator _animator; 

    private float _lastLightAttackTime;
    private float _lastHeavyAttackTime;
    private float _lastDashAttackTime;
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();

        if (CombatStats != null)
        {
            CombatStats = Instantiate(CombatStats);
        }
    }

    public void LevelUp()
    {
        CombatStats.LightAttackDamage += 5;
        CombatStats.HeavyAttackDamage += 15;
        Debug.Log($"Combat stats have increased");
    }

    private void Update()
    {
        if (InputManager.LightAttackWasPressed)
        {
            PerformLightAttack();
        }
        if (InputManager.HeavyAttackWasPressed)
        {
            PerformHeavyAttack();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (CombatStats == null || _attackPoint == null) return;

        // Light Attack
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_attackPoint.position, CombatStats.LightAttackHitBox);

        // Heavy Attack
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_attackPoint.position, CombatStats.HeavyAttackHitBox);

    }

    public bool CanLightAttack()
    {
        return Time.time >= _lastLightAttackTime + CombatStats.LightAttackRate;
    }
    public bool CanHeavyAttack()
    {
        return Time.time >= _lastHeavyAttackTime + CombatStats.HeavyAttackRate;
    }

    public void PerformLightAttack()
    {
        if (CanLightAttack())
        {
            Debug.Log("Performed Light Attack with damage: " + CombatStats.LightAttackDamage);
            PerformAttack(CombatStats.LightAttackHitBox, CombatStats.LightAttackDamage);
            _lastLightAttackTime = Time.time;
            _animator.SetTrigger("LightAttack");
        }
    }
    public void PerformHeavyAttack()
    {
        if (CanHeavyAttack())
        {
            Debug.Log("Performed Heavy Attack with damage: " + CombatStats.HeavyAttackDamage);
            PerformAttack(CombatStats.HeavyAttackHitBox, CombatStats.HeavyAttackDamage);
            _lastHeavyAttackTime = Time.time;
            _animator.SetTrigger("HeavyAttack");
        }
    }
   private  void PerformAttack(float attackHitbox, int damage)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attackPoint.position, attackHitbox, _enemyLayers);
        if (hitEnemies.Length > 0)
        {
            TimeManager.Instance.HitStop();
        }
        foreach (Collider2D enemy in hitEnemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            Debug.Log("Hit " + enemy.name);
            
            enemyHealth.TakeDamage(damage);
            Debug.Log($"Trafiono {enemy.name}! Zadano {damage} dmg.");
        }
    }
}
