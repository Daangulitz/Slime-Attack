using System.Collections;
using UnityEngine;

public class Dodgeball : MonoBehaviour
{
    public LayerMask playerLayer;            // Layer mask to detect the player
    public Transform[] spawnPoints;          // Assign spawn points in the Inspector
    public int damage = 10;            // Damage dealt by the dodgeball
    public bool hasBeenThrown = false; // Track if the dodgeball has been thrown
    public float respawnTime = 2f;           // Time before the dodgeball respawns
    public float throwStrength = 15f;        // Force applied to the dodgeball when thrown
    public float pickupRange = 2f;           // Range in which player can pick up the dodgeball

    private Rigidbody2D rb;
    private bool isHeld = false;             // Whether the ball is currently held by a player
    private Transform playerHolding;         // Reference to the player holding the dodgeball

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // gets the rigibody of the player
        Respawn();  // respawns the player if needed
    }

    private void Update()
    {
        // If dodgeball is held, position it near the player slightly above
        if (isHeld && playerHolding != null)
        {
            transform.position = playerHolding.position + new Vector3(0.5f, 0.2f, 0); // sets a position if the player is holding the ball so it does not look wrong
        }
        
        // finds if the player held the ball and if you then push the button then u throw the ball
        if (isHeld && Input.GetButtonDown("Throw"))
        {
            Throw(); 
        }
        
        // if the dodgeball is out of bound then respawns the ball to a random position
        if (transform.position.y < -10) 
        {
            StartCoroutine(RespawnAfterDelay());
        }
        HandleScreenWrapping();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is in the player layer
        if ((playerLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            // gets the distans to the player
            float distanceToPlayer = Vector2.Distance(transform.position, other.transform.position);

            // If player is within range and dodgeball is not held, allow pickup
            if (!isHeld && distanceToPlayer <= pickupRange)
            {
                Pickup(other.transform);
            }
            // If dodgeball has been thrown, is held, and hits another player, deal damage
            else if (hasBeenThrown && isHeld && other.transform != playerHolding)
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage); // if thrown then deel damage
                
                    // Reset hasBeenThrown and initiate respawn
                    hasBeenThrown = false;
                }
            }
        }
    }
    private void Pickup(Transform player)
    {
        // finds if player has the ball picked up
        isHeld = true;
        playerHolding = player;

        // Set Rigidbody to Dynamic with zero gravity when picked up
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;  // Stop any existing motion

        // Position dodgeball slightly above and to the right of the player
        transform.position = player.position + new Vector3(0.5f, 0.2f, 0);
    }

    private void Throw()
    {
        // throws the ball towarts the position of a inticator 
        isHeld = false;
        playerHolding = null;  // Clear reference to player

        // Enable gravity and apply throw force
        rb.gravityScale = 1f;
        Vector2 throwDirection = transform.right; 
        rb.AddForce(throwDirection * throwStrength, ForceMode2D.Impulse);

        // Start respawn timer after throwing
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        // sets respawn after a delay so it do's no swap instant
        yield return new WaitForSeconds(respawnTime);
        Respawn();
    }

    private void Respawn()
    {
        // Set the dodgeball to a random spawn point and reset its velocity
        int randomIndex = Random.Range(0, spawnPoints.Length);
        transform.position = spawnPoints[randomIndex].position;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 1f; // Ensure gravity is set for respawn
        hasBeenThrown = false;
    }
    private void HandleScreenWrapping()
    {
        // Get the screen bounds in world space
        float screenLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        float screenRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

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
    }

}
