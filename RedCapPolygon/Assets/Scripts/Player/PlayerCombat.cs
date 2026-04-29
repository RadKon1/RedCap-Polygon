using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    [Header("Rates")]
    public float lightAttackRate = 2f;
    public float heavyAttackRate = 1f;
    public float dashAttackRate = 2f;

    private float nextAttackTime = 0f;

    public void lightAttack() => PerformAttack("lightAttack", lightAttackRate);
    public void heavyAttack() => PerformAttack("heavyAttack", heavyAttackRate);
    public void dashAttack() => PerformAttack("dashAttack", dashAttackRate);

    private void PerformAttack(string triggerName, float rate)
    {
        if (Time.time < nextAttackTime) return;

        animator.SetTrigger(triggerName);
        nextAttackTime = Time.time + (1f / rate);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Trafiono: " + enemy.name);
            // enemy.GetComponent<Enemy>().TakeDamage(10);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint) Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
