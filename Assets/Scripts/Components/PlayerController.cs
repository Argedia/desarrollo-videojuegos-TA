using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UniformHorizontalMovement))]
[RequireComponent(typeof(UniformJump))]
public class PlayerController : MonoBehaviour
{
    private UniformHorizontalMovement horizontalMovement;
    private UniformJump uniformJump;

    private PlayerInputActions controls;

    private bool inputEnabled = true;

    private void Awake()
    {
        horizontalMovement = GetComponent<UniformHorizontalMovement>();
        uniformJump = GetComponent<UniformJump>();

        controls = new PlayerInputActions();

        // Bind Move action
        controls.Player.Move.performed += ctx =>
        {
            if (!inputEnabled) return;
            Vector2 input = ctx.ReadValue<Vector2>();
            horizontalMovement.Move(input.x);
        };
        controls.Player.Move.canceled += ctx =>
        {
            if (!inputEnabled) return;
            horizontalMovement.Move(0f);
        };

        // Bind Jump action
        controls.Player.Jump.performed += ctx =>
        {
            if (!inputEnabled) return;
            Debug.Log("Intentó saltar");
            uniformJump.TryJump();
        };
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    public void DisableInput()
    {
        inputEnabled = false;
        horizontalMovement.Move(0f); // stop movement immediately
    }

    public void EnableInput()
    {
        inputEnabled = true;
    }
}
