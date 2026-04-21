using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerCombat combat;
    private float jumpDisableDuration = 0f;
    private float dashDisableDuration = 0f;

    private void Start()
    {
        // Initialize player class here...
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        checkMovementInput();
        checkAttackInput();
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
            movement.startJump();
            movement.jump();
            jumpDisableDuration = 0.5f;
        }

        if (jumpDisableDuration <= 0 && movement.isAirborne)
        {
            movement.endJump();
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

    private void checkAttackInput()
    {
        if (Keyboard.current.jKey.wasPressedThisFrame && dashDisableDuration > 0)
        {
            combat.dashAttack();
        }
        else if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            combat.lightAttack();
        }
        else if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            combat.heavyAttack();
        }
    }
}
