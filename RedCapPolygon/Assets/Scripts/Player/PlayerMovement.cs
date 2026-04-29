using UnityEngine;

public partial class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float dashSpeed = 12.0f;
    [SerializeField] private float jumpForce = 15.0f;

    [Header("Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastLength = 0.6f;

    public bool isDashing { get; private set; }
    public bool isAirborne { get; private set; }
    private float originalGravityScale;

    public Transform groundCheckPoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update() => CheckGround();

    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastLength, groundLayer);

        if (hit.collider != null)
        {
            // LOG DIAGNOSTYCZNY - powie Ci w co trafiłeś
            Debug.Log("Ziemia wykryta! Trafiłem w: " + hit.collider.name);

            isAirborne = false;
            animator.SetBool("isAirborne", false);
        }
        else
        {
            isAirborne = true;
            animator.SetBool("isAirborne", true);
        }
    }

    public void Move(float direction)
    {
        if (isDashing) return;

        rb.linearVelocity = new Vector2(direction * movementSpeed, rb.linearVelocity.y);
        animator.SetFloat("speed", Mathf.Abs(direction));

        // Obracanie całej postaci
        if (direction != 0)
        {
            float scaleX = direction > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        }
    }

    public void StopMoving()
    {
        if (isDashing) return;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetFloat("speed", 0);
    }

    public void Jump()
    {
        // Jeśli skrypt myśli, że jesteś w powietrzu, nie pozwoli skoczyć.
        // Dlatego RaycastLength jest tak kluczowy!
        if (isAirborne || isDashing) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        Debug.Log("Skok wywołany!");
    }

    public void StartDash()
    {
        if (isDashing) return;
        isDashing = true;
        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0f;

        float dir = transform.localScale.x > 0 ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * dashSpeed, 0f);
    }

    public void StopDash()
    {
        isDashing = false;
        rb.gravityScale = originalGravityScale;
        rb.linearVelocity = Vector2.zero;
    }
}
