using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D PlayerRigidBody2D;
    private float movementSpeed = 5.0f;
    private float dashSpeed = 30.0f;
    private float jumpForce = 15.0f;

    private float originalGravityScale;
    public bool isDashing = false;

    private void Awake()
    {
        PlayerRigidBody2D = GetComponent<Rigidbody2D>();
    }

    public void moveLeft()
    {
        PlayerRigidBody2D.linearVelocity = new Vector2(-movementSpeed, PlayerRigidBody2D.linearVelocity.y);
        Debug.Log("Moving left.");
    }

    public void moveRight()
    {
        PlayerRigidBody2D.linearVelocity = new Vector2(movementSpeed, PlayerRigidBody2D.linearVelocity.y);
        Debug.Log("Moving right.");
    }

    public void stopMoving()
    {
        PlayerRigidBody2D.linearVelocity = new Vector2(0, PlayerRigidBody2D.linearVelocity.y);
    }

    public void jump()
    {
        PlayerRigidBody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        Debug.Log("Jumping.");
    }

    public void dash()
    {
        float direction = (PlayerRigidBody2D.linearVelocity.x >= 0) ? 1.0f : -1.0f;

        PlayerRigidBody2D.linearVelocity = new Vector2(direction * dashSpeed, 0f);
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




}
