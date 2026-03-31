using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float lightAttackCooldown = 0.5f;
    [SerializeField] private float dashAttackCooldown = 1f;
    [SerializeField] private float heavyAttackCooldown = 2f;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void lightAttack()
    {
        if (Time.time < nextAttackTime) { return; }

        // Trigger light attack animation here...
        nextAttackTime = Time.time + lightAttackCooldown;
        Debug.Log("Light attack.");
    }

    public void heavyAttack()
    {
        if (Time.time < nextAttackTime) { return; }

        // Trigger heavy attack animation here...
        nextAttackTime = Time.time + heavyAttackCooldown;
        Debug.Log("Heavy attack.");
    }

    public void dashAttack()
    {
        if (Time.time < nextAttackTime) { return; }

        // Trigger dash attack animation here...
        nextAttackTime = Time.time + dashAttackCooldown;
        Debug.Log("dash attack.");
    }
}
