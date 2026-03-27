using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private float jumpDisableDuration = 0f;
    private float dashDisableDuration = 0f;

    private void Start()
    {
        // Initialize player class here...
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        checkMovementInput();
    }

    private void checkMovementInput()
    {

        if (jumpDisableDuration > 0) { jumpDisableDuration -= Time.deltaTime; }
        if (dashDisableDuration > 0) { dashDisableDuration -= Time.deltaTime; }

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame && dashDisableDuration <= 0 && (Keyboard.current.aKey.isPressed || Keyboard.current.dKey.isPressed))
        {
            movement.startDash();
            movement.dash();
            dashDisableDuration = 0.2f;
            return;
        }

        if (dashDisableDuration <= 0 && movement.isDashing)
        {
            movement.stopDash();
            return;
        }


        if (Keyboard.current.spaceKey.wasPressedThisFrame && jumpDisableDuration <= 0)
        {
            movement.jump();
            jumpDisableDuration = 0.5f;
        }

        if (dashDisableDuration <= 0)
        {
            if (Keyboard.current.aKey.isPressed)
            {
                movement.moveLeft();
            }
            else if (Keyboard.current.dKey.isPressed)
            {
                movement.moveRight();
            }
            else
            {
                movement.stopMoving();
            }
        }
    }

}
