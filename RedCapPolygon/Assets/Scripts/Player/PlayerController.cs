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
        // 0.TEST LOG 1
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Naciśnięto spację - sprawdzam warunki...");
        }

        // 1. ATACK
        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            if (movement.isDashing) combat.dashAttack();
            else combat.lightAttack();
        }

        // 2. DASH
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame && dashTimer <= 0)
        {
            movement.StartDash();
            dashTimer = DASH_DURATION;
        }

        // 3. RUCH / SKOK
        if (!movement.isDashing)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Debug.Log("Warunek !isDashing spełniony, wywołuję movement.Jump()");
                movement.Jump();
            }

            if (Keyboard.current.aKey.isPressed) movement.Move(-1);
            else if (Keyboard.current.dKey.isPressed) movement.Move(1);
            else movement.StopMoving();
        }
        else
        {
            // Jeśli ten log się pojawi po naciśnięciu spacji, to znaczy że DASH blokuje skok
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                Debug.Log("Skok zablokowany, bo isDashing == true!");
        }
    }
}