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
        _animator = GetComponent<Animator>();
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
        if (InputManager.DashAttackWasPressed)
        {
            PerformDashAttack();
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

        // Dash Attack
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_attackPoint.position, CombatStats.DashAttackHitBox);
    }

    public bool CanLightAttack()
    {
        return Time.time >= _lastLightAttackTime + CombatStats.LightAttackRate;
    }
    public bool CanHeavyAttack()
    {
        return Time.time >= _lastHeavyAttackTime + CombatStats.HeavyAttackRate;
    }
    public bool CanDashAttack()
    {
        return Time.time >= _lastDashAttackTime + CombatStats.DashAttackRate;
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
    public void PerformDashAttack()
    {
        if (CanDashAttack())
        {
            Debug.Log("Performed Dash Attack with damage: " + CombatStats.DashAttackDamage);
            PerformAttack(CombatStats.DashAttackHitBox, CombatStats.DashAttackDamage);
            _lastDashAttackTime = Time.time;
            _animator.SetTrigger("DashAttack");
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
            Debug.Log("Hit " + enemy.name);

            // hit enemy logic here

            // e.g. enemy.GetComponent<EnemyHealth>().TakeDamage(damage);
        }
    }
}
