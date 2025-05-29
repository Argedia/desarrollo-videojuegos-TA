using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerAttack attack;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Attack.performed += OnAttackPerformed;
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttackPerformed;
        inputActions.Disable();
    }

    private void Update()
    {
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        movement.Run(moveInput.x);

        bool jumpPressed = inputActions.Player.Jump.ReadValue<float>() > 0.1f;
        movement.Jump(jumpPressed);

    }

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        attack.Attack();
    }
}
