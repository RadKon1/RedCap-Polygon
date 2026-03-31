using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void lightAttack()
    {
        if (Time.time < nextAttackTime) { return; }

        // Trigger light attack animation here...
        nextAttackTime = Time.time + attackCooldown;
        Debug.Log("Light attack.");
    }

    public void heavyAttack()
    {
        if (Time.time < nextAttackTime) { return; }

        // Trigger heavy attack animation here...
        nextAttackTime = Time.time + attackCooldown;
        Debug.Log("Heavy attack.");
    }
}
