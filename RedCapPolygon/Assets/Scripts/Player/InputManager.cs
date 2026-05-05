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
    public static bool DashWasPressed;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _attackAction;
    private InputAction _dashAction;

    private void Awake()
    {
        // Szukamy komponentu na tym samym obiekcie lub w rodzicu/dzieciach
        PlayerInput = GetComponentInParent<PlayerInput>();

        if (PlayerInput != null)
        {
            _moveAction = PlayerInput.actions["Move"];
            _jumpAction = PlayerInput.actions["Jump"];
            _runAction = PlayerInput.actions["Run"];
            _dashAction = PlayerInput.actions["Dash"];

            if (PlayerInput.actions.FindAction("Attack") != null)
            {
                _attackAction = PlayerInput.actions["Attack"];
            }
        }
        else
        {
            Debug.LogError("No player Input component");
        }
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();

        RunIsHeld = _runAction.IsPressed();

        DashWasPressed = _dashAction.WasPressedThisFrame();

        // Zabezpieczenie, jeśli jeszcze nie dodałeś ataku do Input Actions
        if (_attackAction != null)
        {
            AttackWasPressed = _attackAction.WasPressedThisFrame();
        }
    }
}

