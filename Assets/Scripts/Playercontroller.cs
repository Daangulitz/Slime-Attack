using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 25f;
    public float stickForce = 100f;
    public float stickRadius = 1f;
    public LayerMask groundLayer;
    public float dashSpeed = 50f;
    public float dashDuration = 0.2f;
    public bool isDashing = false;
    public float dashTime;

    public Rigidbody2D rb;
    public Vector2 inputDirection;
    public bool isSticking;
    public Vector2 surfaceNormal;
    public Vector2 currentVelocity;

    public Animator animator;

    public GameObject dashTargetMarker;
    public Vector2 dashDirection;

    public PlayerInput playerInput;
    public InputAction moveAction;
    public InputAction dashAction;
    public InputAction dashDirectionAction;
    public InputAction throwAction;
    public InputAction pickupAction;
    
    public GameObject dodgeballPrefab;
    public GameObject heldDodgeball;
    public float throwForce = 15f;
    public LayerMask dodgeballLayer;
    public float pickupRange = 0.6f;
    public Vector2 dodgeballHoldOffset;

    public PlayerHealth playerHealth;  // Reference to PlayerHealth script

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();  // Get PlayerHealth component

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput is missing! Please attach PlayerInput to the same GameObject as PlayerController.");
            return;
        }

        // Initialize all input actions
        moveAction = playerInput.actions["MovePlayer"];
        dashAction = playerInput.actions["Jump"];
        dashDirectionAction = playerInput.actions["Dash"];
        throwAction = playerInput.actions["Throw"];
        pickupAction = playerInput.actions["PickUp"];
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing! Please attach Rigidbody2D to the player GameObject.");
            return;
        }

        rb.gravityScale = 2;

        if (dashTargetMarker != null)
        {
            dashTargetMarker.SetActive(true);
        }
    }

    void Update()
    {
        if (playerInput == null) return;

        inputDirection.x = -moveAction.ReadValue<Vector2>().x;

        Vector2 joystickDirection = dashDirectionAction.ReadValue<Vector2>();
        dashDirection = joystickDirection.magnitude > 0.1f ? joystickDirection.normalized : (Vector2)transform.right;

        if (dashTargetMarker != null)
        {
            dashTargetMarker.transform.position = (Vector2)transform.position + dashDirection * 5f;
        }

        if (dashAction.triggered && !isDashing)
        {
            Dash();
        }

        if (throwAction.triggered && heldDodgeball != null)
        {
            ThrowDodgeball();
        }

        if (pickupAction.triggered && heldDodgeball == null)
        {
            CheckPickupRange();
        }

        UpdateAnimations();

        // Screen wrapping logic (left to right)
        HandleScreenWrapping();
        AlignRotationWithSurface();
    }

    private void FixedUpdate()
    {
        MovePlayer();

        if (!isDashing)
        {
            CheckForSurface();
        }

        if (heldDodgeball != null)
        {
            StickDodgeballToPlayer();
        }
    }

    private void CheckForSurface()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, stickRadius, groundLayer);
        if (hits.Length > 0)
        {
            Collider2D closestSurface = hits[0];
            Vector3 closestPoint = closestSurface.ClosestPoint(transform.position);
            surfaceNormal = (transform.position - closestPoint).normalized;

            Vector2 magneticForce = -surfaceNormal * stickForce;
            rb.AddForce(magneticForce);

            isSticking = true;
            rb.gravityScale = 0;
        }
        else
        {
            isSticking = false;
            rb.gravityScale = 2;
        }
    }

    private void MovePlayer()
    {
        currentVelocity = inputDirection * moveSpeed;

        if (isSticking && !isDashing)
        {
            Vector2 tangent = new Vector2(-surfaceNormal.y, surfaceNormal.x);
            Vector2 movement = tangent * currentVelocity.x;
            rb.linearVelocity = movement;
        }
        else if (!isDashing)
        {
            rb.linearVelocity = new Vector2(currentVelocity.x, rb.linearVelocity.y);
        }
        else if (isDashing)
        {
            HandleDash();
        }

        FlipPlayerSprite();
    }

    private void Dash()
    {
        isDashing = true;
        dashTime = Time.time + dashDuration;
        rb.linearVelocity = dashDirection * dashSpeed;
        isSticking = false;
        rb.gravityScale = 2;
    }

    private void HandleDash()
    {
        if (Time.time > dashTime)
        {
            isDashing = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
            CheckForSurface();

            if (isSticking)
            {
                AlignRotationWithSurface();
            }
        }
    }

    private void AlignRotationWithSurface()
    {
        if (surfaceNormal != Vector2.zero)
        {
            float angle = Mathf.Atan2(surfaceNormal.y, surfaceNormal.x) * Mathf.Rad2Deg;
            angle = angle - 90;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void FlipPlayerSprite()
    {
        if (inputDirection.x < 0)
        {
            transform.localScale = new Vector3(0.4f, 0.4f, transform.localScale.z);
            
        }
        else if (inputDirection.x > 0)
        {
            transform.localScale = new Vector3(-0.4f, 0.4f, transform.localScale.z);
            
        }
        CheckForSurface();
    }
    private void HandleScreenWrapping()
    {
        // Get the screen bounds in world space
        float screenLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        float screenRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        float screenBottom = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
        float screenTop = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;

        // If the player crosses the left side of the screen, teleport them to the right side
        if (transform.position.x < screenLeft)
        {
            Vector3 newPosition = new Vector3(screenRight, transform.position.y, transform.position.z);
            transform.position = newPosition;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y); // Keep the same velocity
        }
        // If the player crosses the right side of the screen, teleport them to the left side
        else if (transform.position.x > screenRight)
        {
            Vector3 newPosition = new Vector3(screenLeft, transform.position.y, transform.position.z);
            transform.position = newPosition;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y); // Keep the same velocity
        }

        if (transform.position.y < screenTop)
        {
            Vector3 newPosition = new Vector3(transform.position.x, screenBottom, transform.position.z);
            transform.position = newPosition;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
        }
        else if (transform.position.y > screenBottom)
        {
            Vector3 newPosition = new Vector3(transform.position.x, screenTop, transform.position.z);
            transform.position = newPosition;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
        }
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("Speed", Mathf.Abs(inputDirection.x));
    }

    private void CheckPickupRange()
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, pickupRange, dodgeballLayer);

        foreach (var obj in nearbyObjects)
        {
            if (obj.CompareTag("Dodgeball") && heldDodgeball == null)
            {
                PickupDodgeball(obj.gameObject);
                break;
            }
        }
    }

    private void PickupDodgeball(GameObject dodgeball)
    {
        heldDodgeball = dodgeball;
        heldDodgeball.transform.SetParent(transform);

        StickDodgeballToPlayer();

        Rigidbody2D dodgeballRb = heldDodgeball.GetComponent<Rigidbody2D>();
        dodgeballRb.bodyType = RigidbodyType2D.Kinematic;
        dodgeballRb.gravityScale = 0f;
    }

    private void ThrowDodgeball()
    {
        if (heldDodgeball != null)
        {
            Rigidbody2D dodgeballRb = heldDodgeball.GetComponent<Rigidbody2D>();

            // Enable physics again for the dodgeball
            dodgeballRb.bodyType = RigidbodyType2D.Dynamic;
            dodgeballRb.gravityScale = 1f;

            // Set the dodgeball's position to be offset along the dash direction
            Vector2 throwPosition = (Vector2)transform.position + dashDirection.normalized * dodgeballHoldOffset.magnitude;

            // Update the dodgeball's position to be in the correct place relative to the player
            heldDodgeball.transform.position = throwPosition;

            // Throw the dodgeball in the dash direction with force
            dodgeballRb.AddForce(dashDirection * throwForce, ForceMode2D.Impulse);

            // Set hasBeenThrown to true so it can deal damage
            Dodgeball dodgeballComponent = heldDodgeball.GetComponent<Dodgeball>();
            if (dodgeballComponent != null)
            {
                dodgeballComponent.hasBeenThrown = true;
            }

            // Detach the dodgeball from the player
            heldDodgeball.transform.SetParent(null);
            heldDodgeball = null;
        }
    }
    private void StickDodgeballToPlayer()
    {
        // Use dashDirection to offset the dodgeball when held (if desired)
        Vector2 offsetPosition = (Vector2)transform.position + dashDirection.normalized * dodgeballHoldOffset.magnitude;
    
        // Set the dodgeball's position relative to the player
        heldDodgeball.transform.position = offsetPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the colliding object is a dodgeball
        Dodgeball dodgeball = collision.gameObject.GetComponent<Dodgeball>();
        if (dodgeball != null && dodgeball.hasBeenThrown && playerHealth != null)
        {
            // Apply damage to the player
            playerHealth.TakeDamage(dodgeball.damage);
        
            // Optional: Reset the hasBeenThrown flag if you want to "disarm" the dodgeball after hitting the player
            dodgeball.hasBeenThrown = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, pickupRange);
        }
    }
}
