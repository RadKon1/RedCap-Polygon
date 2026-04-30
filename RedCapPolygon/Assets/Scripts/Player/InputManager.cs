using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    public static bool RunIsHeld;
    public static bool AttackWasPressed;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _attackAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        // Pobieranie akcji po ich nazwach z komponentu Player Input
        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _runAction = PlayerInput.actions["Run"];

        // Upewnij się, że dodasz akcję "Attack" w ustawieniach Input Actions!
        _attackAction = PlayerInput.actions["Attack"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();

        RunIsHeld = _runAction.IsPressed();

        // Zabezpieczenie, jeśli jeszcze nie dodałeś ataku do Input Actions
        if (_attackAction != null)
        {
            AttackWasPressed = _attackAction.WasPressedThisFrame();
        }
    }
}
