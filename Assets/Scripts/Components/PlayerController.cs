using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UniformHorizontalMovement))]
[RequireComponent(typeof(UniformJump))]
public class PlayerController : MonoBehaviour
{
    private UniformHorizontalMovement horizontalMovement;
    private UniformJump uniformJump;

    private PlayerInputActions controls;
    public HandManager handManager;

    private bool inputEnabled = true;

    private void Awake()
    {
        horizontalMovement = GetComponent<UniformHorizontalMovement>();
        uniformJump = GetComponent<UniformJump>();
        if (handManager == null)
            Debug.LogWarning("HandManager no asignado en PlayerController");
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
        // Bind Card 1
        controls.Player.Card1.performed += ctx =>
        {
            if (!inputEnabled) return;
            handManager.UseCard(0, transform);
        };

        // Bind Card 2
        controls.Player.Card2.performed += ctx =>
        {
            if (!inputEnabled) return;
            handManager.UseCard(1, transform);
        };
        //Bind Card 3
        controls.Player.Card3.performed += ctx =>
        {
            if (!inputEnabled) return;
            handManager.UseCard(2, transform);
        };

        // Bind Card 4
        controls.Player.Card4.performed += ctx =>
        {
            if (!inputEnabled) return;
            handManager.UseCard(3, transform);
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
