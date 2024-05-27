using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Add this line

public class mainCharacter : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private bool isJumping = false;
    private int jumpsLeft = 2; // Number of jumps the character can perform
    private bool isAttacking = false;
    private bool isFlipped = false;
    private bool isSpeedBoosted = false; // Track whether speed is boosted
    private bool isImmune = false; // Track whether the character is immune
    private float originalSpeed = 7f; // Original movement speed
    private float currentSpeed; // Current movement speed
    private int life = 3; // Life of the character
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private GameObject bulletPrefab; // Reference to the bullet prefab
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip powerupsSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip jumpSound;
    private float lastShootTime; // Time of the last shot
    private float shootCooldown = 2f; // Cooldown between shots
    private int enemyLayer; // Layer of the enemies
    private int playerLayer; // Layer of the player
    public GameOverPanel gameOverPanel;
    // Respawn position
    private Vector3 respawnPosition = new Vector3(-0.5f, 5.69f, 0f);
    private ParticleSystem auraParticleSystem;

    // UI Text to show remaining life
    public Text lifeText;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource gameOverSource;
    [SerializeField] private AudioSource levelAccomplishedSource;
    // Start is called before the first frame update
    private void Start()
    {
        // Set the respawn position
        respawnPosition = new Vector3(-0.5f, 5.69f, 0f);

        // Additional initialization code
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Get the layers
        enemyLayer = LayerMask.NameToLayer("Enemy");
        playerLayer = LayerMask.NameToLayer("Player");

        auraParticleSystem = GetComponentInChildren<ParticleSystem>();

        // Initialize the life text
        UpdateLifeText();
    }

    // Update is called once per frame
    private void Update()
    {
        // Check if the speed is boosted
        float speedMultiplier = isSpeedBoosted ? currentSpeed : originalSpeed;

        float dirX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(dirX * speedMultiplier, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && (isGrounded() || jumpsLeft > 1))
        {
            if (!isGrounded() && jumpsLeft > 0) // Perform double jump
            {
                jumpsLeft--;
            }
            rb.velocity = new Vector2(rb.velocity.x, 7f);
            if (jumpSound != null)
            {
                AudioSource.PlayClipAtPoint(jumpSound, transform.position);
            }
        }

        // Check if the attack key is pressed down
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Set the attacking animation parameter to true
            anim.SetBool("Attacking", true);
            // Set the attack state to true
            isAttacking = true;
        }

        // Check if the attack key is released and the player is attacking
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (Time.time - lastShootTime > shootCooldown)
            {
                Shoot();
                lastShootTime = Time.time;
            }
            // Set the attacking animation parameter to false
            anim.SetBool("Attacking", false);
            // Reset the attack state to false
            isAttacking = false;
        }

        if (isGrounded())
        {
            jumpsLeft = 2; // Reset jumps when grounded
        }

        if (rb.velocity.y > 2f)
        {
            anim.SetBool("Jumping", true);
        }
        else
        {
            anim.SetBool("Jumping", false);
        }

        if (rb.velocity.x != 0f)
        {
            anim.SetBool("Running", true);
        }
        else
        {
            anim.SetBool("Running", false);
        }

        if (rb.velocity.x == 0f && rb.velocity.y == 0f)
        {
            anim.SetBool("idle", true);
        }
        else
        {
            anim.SetBool("idle", false);
        }

        if (dirX > 0f)
        {
            spriteRenderer.flipX = false; // Not flipped (facing right)
            isFlipped = false;
        }
        else if (dirX < 0f)
        {
            spriteRenderer.flipX = true; // Flipped (facing left)
            isFlipped = true;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab != null)
        {
            // Adjust the bullet's position based on character flip
            Vector3 bulletOffset = isFlipped ? new Vector3(-0.7f, -0.4f, 0f) : new Vector3(0.7f, -0.4f, 0f);
            GameObject newBullet = Instantiate(bulletPrefab, transform.position + bulletOffset, Quaternion.identity);
            Bullet bulletComponent = newBullet.GetComponent<Bullet>(); // Get the bullet component
            if (bulletComponent != null)
            {
                bulletComponent.SetDirection(isFlipped); // Pass the flip state of the character to the bullet

                // Flip the bullet sprite if necessary
                SpriteRenderer bulletSpriteRenderer = newBullet.GetComponent<SpriteRenderer>();
                if (bulletSpriteRenderer != null)
                {
                    bulletSpriteRenderer.flipX = !isFlipped; // Flip the bullet if character is facing right
                }
            }

            // Play shoot sound if available
            if (shootSound != null)
            {
                AudioSource.PlayClipAtPoint(shootSound, transform.position);
            }
        }
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);

        // Check if the character collides with an object with the "Enemy" tag and is not immune
        if ((collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Obstacle")) && !isImmune)
        {
            Debug.Log("Character collided with enemy or obstacle");

            // Decrease life by one
            life--;

            // Update the life text
            UpdateLifeText();

            // Debug log remaining life
            Debug.Log("Remaining life: " + life);
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
            }

            // Check if life is zero
            if (life <= 0)
            {
                if (gameOverSource != null)
                {
                    gameOverSource.Play();
                    
                }
                Debug.Log("Game over sound played");
                Time.timeScale = 0;
                if (gameOverPanel != null)
                {
                                   gameOverPanel.GameOver();
                }
                else
                {
                    Debug.LogError("GameOverPanel is not assigned in the Inspector.");
                }

                if (musicAudioSource != null)
                {
                    musicAudioSource.Stop();
                }
            }
            else
            {
                // Trigger blinking effect
                StartCoroutine(BlinkingEffect());
            }
        }
        if (collision.gameObject.CompareTag("FallTrigger"))
        {
            life--;
            UpdateLifeText(); // Update the life text
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
            }
            if (life >= 1)
            {
                transform.position = respawnPosition;
                StartCoroutine(BlinkingEffect());
                Debug.Log("Remaining life: " + life);
            }
            else
            {
                if (gameOverSource != null)
                {
                    gameOverSource.Play();
                }
                Time.timeScale = 0;
               
                if (gameOverPanel != null)
                {
                    gameOverPanel.GameOver();
                }
                else
                {
                    Debug.LogError("GameOverPanel is not assigned in the Inspector.");
                }

                if (musicAudioSource != null)
                {
                    musicAudioSource.Stop();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("agilityPotion"))
        {
            StartCoroutine(ApplySpeedBoost(10f, 5f)); // Apply speed boost for 5 seconds
            Destroy(collision.gameObject); // Destroy the potion
            if (powerupsSound != null)
            {
                AudioSource.PlayClipAtPoint(powerupsSound, transform.position);
            }
        }
        else if (collision.gameObject.CompareTag("defensePotion"))
        {
            StartCoroutine(ApplyImmunity(5f)); // Apply immunity for 5 seconds
            Destroy(collision.gameObject); // Destroy the potion
            if (powerupsSound != null)
            {
                AudioSource.PlayClipAtPoint(powerupsSound, transform.position);
            }
        }
        else if (collision.gameObject.CompareTag("healthPotion"))
        {
            life++;
            Debug.Log("Remaining life: " + life);
            StartCoroutine(ShineEffect(1f)); // Trigger shining effect for 1 second
            Destroy(collision.gameObject); // Destroy the potion
            UpdateLifeText(); // Update the life text
            if (powerupsSound != null)
            {
                AudioSource.PlayClipAtPoint(powerupsSound, transform.position);
            }
        }if (powerupsSound != null)
            {
                AudioSource.PlayClipAtPoint(powerupsSound, transform.position);
            }

        if (collision.gameObject.CompareTag("laser") && !isImmune)
        {
            Debug.Log("Character collided with enemy or obstacle");

            // Decrease life by one
            life--;
            UpdateLifeText();
            // Debug log remaining life
            Debug.Log("Remaining life: " + life);
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
            }

            // Check if life is zero
            if (life <= 0)
            {
                if (gameOverSource != null)
                {
                    gameOverSource.Play();
                }
                // Respawn the character at the initial respawn position
                Time.timeScale = 0;
                
                gameOverPanel.GameOver();

                if (musicAudioSource != null)
                {
                    musicAudioSource.Stop();
                }
            }
            else
            {
                // Trigger blinking effect
                StartCoroutine(BlinkingEffect());
            }
        }
        Debug.Log("Triggered: " + collision.gameObject.name); // Debug log for checking if OnTriggerEnter2D is called

        if (collision.gameObject.CompareTag("Door"))
        {
            if (levelAccomplishedSource != null)
            {
                levelAccomplishedSource.Play();

            }

            if (musicAudioSource != null)
            {
                musicAudioSource.Stop();
            }
            //SceneManager.LoadScene("Level 2");
        }
    }

    private void UpdateLifeText()
    {
        if (lifeText != null)
        {
            lifeText.text = ": " + life;
        }
    }

    private IEnumerator BlinkingEffect()
    {
        const float blinkTime = 0.1f; // Duration of each blink
        const int blinkCount = 5; // Number of blinks

        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f); // Set sprite opacity to transparent
            yield return new WaitForSeconds(blinkTime);

            spriteRenderer.color = new Color(1f, 1f, 1f, 1f); // Set sprite opacity to opaque
            yield return new WaitForSeconds(blinkTime);
        }
    }

    private IEnumerator ApplySpeedBoost(float boostAmount, float duration)
    {
        // Apply speed boost
        currentSpeed += boostAmount;
        isSpeedBoosted = true;

        // Wait for duration
        yield return new WaitForSeconds(duration);

        // Reset speed back to original value
        currentSpeed = originalSpeed;
        isSpeedBoosted = false;
    }

    private IEnumerator ApplyImmunity(float duration)
    {
        // Apply immunity
        isImmune = true;

        // Change opacity to indicate immunity
        SetOpacity(0.5f);

        // Disable collision with objects tagged as "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                Physics2D.IgnoreCollision(coll, enemyCollider, true);
            }
        }

        // Disable collision with objects tagged as "Obstacle"
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles)
        {
            Collider2D obstacleCollider = obstacle.GetComponent<Collider2D>();
            if (obstacleCollider != null)
            {
                Physics2D.IgnoreCollision(coll, obstacleCollider, true);
            }
        }

        Debug.Log("Immunity applied, collisions with enemies and obstacles ignored.");

        // Debug log before waiting
        Debug.Log("Waiting for " + duration + " seconds...");

        // Wait for duration
        yield return new WaitForSeconds(duration);

        // Debug log after waiting
        Debug.Log("Immunity ended after " + duration + " seconds.");

        // Re-enable collision with objects tagged as "Enemy"
        foreach (GameObject enemy in enemies)
        {
            Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                Physics2D.IgnoreCollision(coll, enemyCollider, false);
            }
        }

        // Re-enable collision with objects tagged as "Obstacle"
        foreach (GameObject obstacle in obstacles)
        {
            Collider2D obstacleCollider = obstacle.GetComponent<Collider2D>();
            if (obstacleCollider != null)
            {
                Physics2D.IgnoreCollision(coll, obstacleCollider, false);
            }
        }

        Debug.Log("Collisions with enemies and obstacles re-enabled.");

        // Reset opacity back to full
        SetOpacity(1f);

        // Reset immunity
        isImmune = false;
    }

    private void SetOpacity(float opacity)
    {
        Color color = spriteRenderer.color;
        color.a = opacity;
        spriteRenderer.color = color;
    }
   
    private IEnumerator ShineEffect(float duration)
    {
        // Enable the aura particle system
        if (auraParticleSystem != null)
        {
            auraParticleSystem.Play();
            Debug.Log("particle detected");
        }

        // Wait for the duration
        yield return new WaitForSeconds(duration);

        // Disable the aura particle system
        if (auraParticleSystem != null)
        {
            auraParticleSystem.Stop();
            Debug.Log("no particle detected");
        }
    }
}
