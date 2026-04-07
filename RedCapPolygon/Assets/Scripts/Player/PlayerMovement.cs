using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D PlayerRigidBody2D;
    private Animator animator;
    private float movementSpeed = 1.5f;
    private float dashSpeed = 5.0f;
    private float jumpForce = 6.25f;

    private float originalGravityScale;
    public bool isDashing = false;
    public bool isAirborne = false;

    private void Awake()
    {
        PlayerRigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void moveLeft()
    {
        PlayerRigidBody2D.linearVelocity = new Vector2(-movementSpeed, PlayerRigidBody2D.linearVelocity.y);
        SetAnimatorMovement(new Vector2(-1, 0));
        Debug.Log("Moving left.");
    }

    public void moveRight()
    {
        PlayerRigidBody2D.linearVelocity = new Vector2(movementSpeed, PlayerRigidBody2D.linearVelocity.y);
        SetAnimatorMovement(new Vector2(1, 0));
        Debug.Log("Moving right.");
    }

    public void stopMoving()
    {
        PlayerRigidBody2D.linearVelocity = new Vector2(0, PlayerRigidBody2D.linearVelocity.y);
        SetAnimatorMovement(Vector2.zero);
    }

    public void jump()
    {
        PlayerRigidBody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        SetAnimatorMovement(new Vector2(PlayerRigidBody2D.linearVelocity.x, 1));
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

        if (Mathf.Abs(direction.x) > 0.01f)
        {
            animator.SetFloat("xDir", direction.x);
        }
    }

}
