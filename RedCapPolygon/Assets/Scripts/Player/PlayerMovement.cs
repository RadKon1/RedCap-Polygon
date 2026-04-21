using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D PlayerRigidBody2D;
    private Animator animator;
    private PlayerCombat playerCombat;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5.0f; // Podbiłem bazowo, żeby nie "płynął"
    [SerializeField] private float dashSpeed = 12.0f;
    [SerializeField] private float jumpForce = 15.0f;    // Znacznie zwiększone dla dużych modeli

    [Header("Detection Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackOffset;
    [SerializeField] private float raycastLength = 0.5f; // Zwiększyłem, żeby przy dużym modelu łapał grunt

    private Vector2 slopeNormalPerpendicular;
    private float originalGravityScale;

    public bool isDashing = false;
    public bool isAirborne = false;
    public bool onStairs = false;

    private Vector3 savedScale;

    private void Awake()
    {
        PlayerRigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        playerCombat = GetComponent<PlayerCombat>();
        savedScale = transform.localScale;
    }

    private void Update()
    {
        CheckGround();
    }

    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastLength, groundLayer);
        if (hit.collider != null)
        {
            isAirborne = false;
            animator.SetBool("isAirborne", false);

            slopeNormalPerpendicular = new Vector2(hit.normal.y, -hit.normal.x).normalized;
            float angle = Vector2.Angle(hit.normal, Vector2.up);

            if (angle > 5f && angle < 60f)
            {
                onStairs = true;
                animator.SetBool("onStairs", true);
            }
            else
            {
                onStairs = false;
                animator.SetBool("onStairs", false);
            }
            Debug.DrawRay(transform.position, Vector2.down * raycastLength, Color.red);
        }
        else
        {
            isAirborne = true;
            animator.SetBool("isAirborne", true);
            onStairs = false;
            animator.SetBool("onStairs", false);
        }
    }

    public void moveLeft()
    {
        PlayerRigidBody2D.linearVelocity = new Vector2(-movementSpeed, PlayerRigidBody2D.linearVelocity.y);

        if (playerCombat != null && playerCombat.attackPoint != null)
            playerCombat.attackPoint.localPosition = new Vector3(-attackOffset, 0, 0);

        transform.localScale = new Vector3(-savedScale.x, savedScale.y, savedScale.z);

        animator.SetFloat("speed", 1f);
    }

    public void moveRight()
    {
        PlayerRigidBody2D.linearVelocity = new Vector2(movementSpeed, PlayerRigidBody2D.linearVelocity.y);

        if (playerCombat != null && playerCombat.attackPoint != null)
            playerCombat.attackPoint.localPosition = new Vector3(attackOffset, 0, 0);

        transform.localScale = new Vector3(savedScale.x, savedScale.y, savedScale.z);

        animator.SetFloat("speed", 1f);
    }

    public void stopMoving()
    {
        PlayerRigidBody2D.linearVelocity = new Vector2(0f, PlayerRigidBody2D.linearVelocity.y);
        animator.SetFloat("speed", 0f);
    }

    public void jump()
    {
        // Physics Jump Logic
        PlayerRigidBody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Animator Update
        animator.SetBool("isAirborne", true);

        Debug.Log("Jumping.");
    }

    public void startJump()
    {
        isAirborne = true;
    }

    public void endJump()
    {
        isAirborne = true;
    }

    public void dash()
    {
        float direction = (PlayerRigidBody2D.linearVelocity.x >= 0) ? 1.0f : -1.0f;
        PlayerRigidBody2D.linearVelocity = new Vector2(direction * dashSpeed, 0f);

        SetAnimatorMovement(new Vector2(direction, 0));
        Debug.Log("Dashing.");
    }

    public void startDash()
    {
        if (isDashing) return; // Prevent starting a new dash if already dashing

        isDashing = true;
        originalGravityScale = PlayerRigidBody2D.gravityScale;
        PlayerRigidBody2D.gravityScale = 0f;
    }

    public void stopDash()
    {
        isDashing = false;
        PlayerRigidBody2D.gravityScale = originalGravityScale;
        PlayerRigidBody2D.linearVelocity = new Vector2(0f, PlayerRigidBody2D.linearVelocity.y);
    }

    private void SetAnimatorMovement(Vector2 direction)
    {
        animator.SetFloat("speed", Mathf.Abs(direction.x));
    }
}
