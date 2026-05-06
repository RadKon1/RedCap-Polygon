using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public PlayerCombatStats CombatStats;

    [SerializeField] private LayerMask _enemyLayers;
    private Animator _animator; 

    private float _lastLightAttackTime;
    private float _lastHeavyAttackTime;
    private float _lastDashAttackTime;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
            _lastLightAttackTime = Time.time;
            _animator.SetTrigger("LightAttack");
        }
    }
    public void PerformHeavyAttack()
    {
        if (CanHeavyAttack())
        {
            Debug.Log("Performed Heavy Attack with damage: " + CombatStats.HeavyAttackDamage);
            _lastHeavyAttackTime = Time.time;
            _animator.SetTrigger("HeavyAttack");
        }
    }
    public void PerformDashAttack()
    {
        if (CanDashAttack())
        {
            Debug.Log("Performed Dash Attack with damage: " + CombatStats.DashAttackDamage);
            _lastDashAttackTime = Time.time;
            _animator.SetTrigger("DashAttack");
        }
    }
}
