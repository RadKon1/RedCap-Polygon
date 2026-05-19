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
    public static bool LightAttackWasPressed;
    public static bool HeavyAttackWasPressed;
    public static bool DashAttackWasPressed;
    public static bool DashWasPressed;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _dashAction;
    private InputAction _lightAttackAction;
    private InputAction _heavyAttackAction;
    private InputAction _dashAttackAction;


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
            _lightAttackAction = PlayerInput.actions["LightAttack"];
            _heavyAttackAction = PlayerInput.actions["HeavyAttack"];
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

        LightAttackWasPressed = _lightAttackAction.WasPressedThisFrame();
        HeavyAttackWasPressed = _heavyAttackAction.WasPressedThisFrame();
    }
}

