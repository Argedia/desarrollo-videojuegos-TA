    using UnityEngine;
    using UnityEngine.InputSystem;

    [RequireComponent(typeof(UniformHorizontalMovement))]
    [RequireComponent(typeof(UniformJump))]
    public class PlayerController : Controller, IController
    {
        private UniformHorizontalMovement horizontalMovement;
        private UniformJump uniformJump;

        private PlayerInputActions controls;
        public HandManager handManager;
        public EnergyManager energyManager;
        private bool inputEnabled = true;
        private int currentEnergy;
        [SerializeField] private float detectionDistance = 1f;
        [SerializeField] private LayerMask platformLayer;

        private BoxCollider2D playerCollider;
    private void Awake()
        {
        playerCollider = GetComponent<BoxCollider2D>();
        horizontalMovement = GetComponent<UniformHorizontalMovement>();
            uniformJump = GetComponent<UniformJump>();
        energyManager = GetComponent<EnergyManager>();
            if (handManager == null)
            Debug.LogWarning("HandManager no asignado en PlayerController");
            controls = new PlayerInputActions();

            // Bind Move action
            controls.Player.Move.performed += ctx =>
            {
                if (!inputEnabled) return;
                Vector2 input = ctx.ReadValue<Vector2>();
                horizontalMovement.Move(input.x);
                if (input.x * facingDir < 0)
                {
                    Flip();
                }
            };
            controls.Player.DropDown.performed += ctx =>
            {
                if (!inputEnabled) return;
                TryPassThroughPlatform();
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
                //Debug.Log("Intentó saltar");
                uniformJump.TryJump();
            };
            // Bind Card 1
            controls.Player.Card1.performed += ctx =>
            {
                useCardKey(0);
            };

            // Bind Card 2
            controls.Player.Card2.performed += ctx =>
            {
                useCardKey(1);
            };
            //Bind Card 3
            controls.Player.Card3.performed += ctx =>
            {
                useCardKey(2);
            };

            // Bind Card 4
            controls.Player.Card4.performed += ctx =>
            {
                useCardKey(3);
            };
        }

    private void useCardKey(int key)
    {
        currentEnergy = energyManager.Energy;
        if (!inputEnabled) return;
        handManager.TryUseCard(key, transform, ref currentEnergy);
        energyManager.Energy = currentEnergy;
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

        private void TryPassThroughPlatform()
        {
            Vector2 origin = transform.position;
            Vector2 size = playerCollider.size;

            RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, detectionDistance, platformLayer);

            if (hit.collider != null)
            {
                PlatformPassThrough platform = hit.collider.GetComponent<PlatformPassThrough>();
                if (platform != null)
                {
                    platform.IgnorePlayerTemporarily(playerCollider);
                }
            }
        }
}
