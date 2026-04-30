using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerCombat combat;

    private float dashTimer = 0f;
    private const float DASH_DURATION = 0.2f;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        HandleTimers();
        HandleInput();
    }

    private void HandleTimers()
    {
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0) movement.StopDash();
        }
    }

    private void HandleInput()
    {
        // Sprawdzenie czy w ogóle nowy Input System nam działa
        if (Keyboard.current == null) return;

        // 1. ATACK
        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            if (movement.isDashing) combat.dashAttack();
            else combat.lightAttack();
        }
        else if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            if (!movement.isDashing && !movement.isAirborne) combat.heavyAttack();
        }

        // 2. DASH
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame && dashTimer <= 0)
        {
            Debug.Log("Wciśnięto Shift");
            movement.StartDash();
            dashTimer = DASH_DURATION;
        }

        // 3. RUCH / SKOK
        if (!movement.isDashing)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Debug.Log("Wciśnięto Spację");
                movement.Jump();
            }

            if (Keyboard.current.aKey.isPressed) movement.Move(-1);
            else if (Keyboard.current.dKey.isPressed) movement.Move(1);
            else movement.StopMoving();
        }
        else
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                Debug.Log("Postać jest w trakcie dasha (isDashing == true)!");
        }
    }
}