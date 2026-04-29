using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;


    [Header("Attack Settings")]
    [SerializeField] private float lightAttackCooldown = 0.5f;
    [SerializeField] private float dashAttackCooldown = 1f;
    [SerializeField] private float heavyAttackCooldown = 2f;
    private float nextAttackTime = 0f;

    [Header("Hitbox Setup")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void lightAttack()
    {
        if (Time.time < nextAttackTime) { return; }

        // Trigger light attack animation here...
        // animator.SetTrigger("lightAttack");

        nextAttackTime = Time.time + lightAttackCooldown;

        Debug.Log("Light attack.");
    }


    public void heavyAttack()
    {
        if (Time.time < nextAttackTime) { return; }
  
        // Trigger heavy attack animation here...
        // animator.SetTrigger("heavyAttack");

        nextAttackTime = Time.time + heavyAttackCooldown;

        Debug.Log("Heavy attack.");
    }

    public void dashAttack()
    {
        if (Time.time < nextAttackTime) { return; }

        // Trigger dash attack animation here...
        // animator.SetTrigger("dashAttack");


        nextAttackTime = Time.time + dashAttackCooldown;
        Debug.Log("dash attack.");
    }

    public void Hit(string attackType)
    {
        float currentDamage = 10f; // later change get this from PlaayerStats
        float currentRange = attackRange;

        if (attackType == "heavy")
        {
            currentDamage *= 2.5f;
            currentRange *= 1.2f;
        }

        // also handle dash attack but for now we will just use the same values as light attack

        // --- HIT BOX LOGICC ---
        // Creating a virtual circcle and collecting all enemies within it
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, currentRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Enemy hit");
        }
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null) { return; }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
