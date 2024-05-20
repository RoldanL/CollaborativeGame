using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private GameObject bulletPrefab; // Reference to the bullet prefab
    private float lastShootTime; // Time of the last shot
    private float shootCooldown = 2f; // Cooldown between shots

    // Respawn position
    private Vector3 respawnPosition = new Vector3(-0.5f, 5.69f, 0f);

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
    }

    // Update is called once per frame
    private void Update()
    {
        float dirX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(dirX * 7f, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && (isGrounded() || jumpsLeft > 1))
        {
            if (!isGrounded() && jumpsLeft > 0) // Perform double jump
            {
                jumpsLeft--;
            }
            rb.velocity = new Vector2(rb.velocity.x, 7f);
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
            // Set the attacking animation parameter to false
            anim.SetBool("Attacking", false);
            // Reset the attack state to false
            isAttacking = false;

            // Shoot only if the cooldown time has passed
            if (Time.time - lastShootTime > shootCooldown)
            {
                Shoot();
                lastShootTime = Time.time;
            }
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
            bullet bulletComponent = newBullet.GetComponent<bullet>(); // Get the bullet component
            if (bulletComponent != null)
            {
                bulletComponent.SetDirection(isFlipped); // Pass the flip state of the character to the bullet
            }
        }
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);

        // Check if the character collides with an object with the "Enemy" tag
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Character collided with enemy");

            // Respawn the character at the initial respawn position
            transform.position = respawnPosition;

            Debug.Log("Character respawned at: " + respawnPosition);
        }
    }
}
