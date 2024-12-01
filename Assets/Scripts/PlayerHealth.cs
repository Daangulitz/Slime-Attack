using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int PlayerID; // Assign a unique ID for each player
    public bool IsAlive = true;
    public int maxHealth = 100; // Maximum health for the player
    private int _currentHealth; // Current health of the player
    private Animator _animator; // Animator for handling animations
    public Image healthBar; // Reference to the health bar UI element

    private PlayerController _playerController; // Reference to PlayerController script
    private BoxCollider2D _boxCollider; // Reference to BoxCollider2D
    private Rigidbody2D _rigidbody; // Reference to Rigidbody2D
    
    public AudioSource HitSound;
    public AudioSource DeathSound;
    
    public ParticleSystem DeathParticles;

    private Gamepad _playerGamepad; // The gamepad assigned to this player
    private PlayerInput _playerInput; // The PlayerInput component associated with this player
    
    public static PlayerHealth instance;
    public GameRoundManager gameRoundManager;

    private void Start()
    {
        _animator = GetComponent<Animator>();// gets animator
        _playerController = GetComponent<PlayerController>(); // gets player controller
        _boxCollider = GetComponent<BoxCollider2D>(); // gets box colidder
        _rigidbody = GetComponent<Rigidbody2D>(); // gets rigibody

        AudioSource[] audioSources = GetComponentsInChildren<AudioSource>(); // finds the audiosources needed for the different sounds
        HitSound = audioSources[0];
        DeathSound = audioSources[1];

        _currentHealth = maxHealth; // sets the current health to the max health
        _animator.SetBool("IsDead", false); // sets the animation to false

        if (healthBar != null)
        {
            UpdateHealthBar(); // sets the health bar
        }

        AssignGamepad(); // assigns a controller so that the player dont swap controllers when playing

        // Initialize GameRoundManager at the start
        if (gameRoundManager == null)
        {
            gameRoundManager = FindObjectOfType<GameRoundManager>(); 
        }
    }


    private void AssignGamepad()
    {
        _playerInput = GetComponent<PlayerInput>(); // gets playerinput so set a controller to it

        if (_playerInput != null && _playerInput.currentControlScheme == "Gamepad")
        {
            _playerGamepad = _playerInput.devices[0] as Gamepad;
            Debug.Log("Assigned Gamepad: " + _playerGamepad?.name);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        _currentHealth -= damageAmount; // do's the damageamount - the currenthealth to get a new current health
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth); // Ensure health stays within valid range
        UpdateHealthBar(); // sets healthbar to new currenthealth

        _animator.SetTrigger("Hit"); // triggers the animation

        HitSound.Play(); // plays hit sound

        if (_currentHealth <= 0) // if the health is 0 or below the the player is dead
        {
            Die();
        }
    }

    private void UpdateHealthBar() // update's the health bar to current health
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)_currentHealth / maxHealth;
        }
    }

    public void Die()
    {
        IsAlive = false; // sets is alive to fals
        _animator.SetBool("IsDead", true); // plays death animation

        DeathSound.Play(); // plays death sound
        DeathParticles.Play(); // shows the death particals

        // Disable player control, collider, and rigidbody
        if (_playerController != null)
            _playerController.enabled = false;

        if (_boxCollider != null)
            _boxCollider.enabled = false;

        if (gameRoundManager != null) // checks if ther is a gameroundmanager so he can send who died
        {
            gameRoundManager.CheckForRoundWinner();
        }
        else
        {
            Debug.LogWarning("GameRoundManager not found! Could not notify about player death.");
        }
    }


    private void OnCollisionEnter(Collision collision) // this is how he takes damage
    {
        Dodgeball dodgeball = collision.gameObject.GetComponent<Dodgeball>();
        if (dodgeball != null)
        {
            TakeDamage(dodgeball.damage);
        }
    }

    public void SetHealthBar(Image newHealthBar) // sets the health bar a beginning
    {
        healthBar = newHealthBar;
        UpdateHealthBar(); 
    }
}
