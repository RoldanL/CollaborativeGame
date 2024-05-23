using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f; // Speed of the bullet
    public float lifetime = 1.2f; // Lifetime of the bullet before it's destroyed
    private bool isFlipped = false; // Indicates if the character is flipped
    private float elapsedTime = 0f; // Timer to track the elapsed time
    private Vector3 previousPosition; // Previous position of the bullet

    private void Start()
    {
        Debug.Log("Bullet appeared");
        // Set velocity to move the bullet in the correct direction
        float direction = isFlipped ? -1f : 1f;
        Vector2 velocity = new Vector2(speed * direction, 0f);

        // Apply velocity to the bullet
        GetComponent<Rigidbody2D>().velocity = velocity;

        // Initialize previous position
        previousPosition = transform.position;
    }

    private void Update()
    {
        // Increment the elapsed time
        elapsedTime += Time.deltaTime;

        // Check for collision using raycasting
        CheckCollision();

        // Destroy the bullet after a specified lifetime
        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }

        // Update previous position
        previousPosition = transform.position;
    }

    private void CheckCollision()
    {
        // Calculate the direction and distance of the bullet's movement
        Vector3 direction = transform.position - previousPosition;
        float distance = direction.magnitude;

        // Normalize the direction vector
        direction.Normalize();

        // Perform the raycast
        RaycastHit2D hit = Physics2D.Raycast(previousPosition, direction, distance);

        // Check if the raycast hit something
        if (hit.collider != null)
        {
            Debug.Log("Bullet collided with: " + hit.collider.gameObject.name);

            // Check if the bullet collides with an enemy
            if (hit.collider.CompareTag("Enemy"))
            {
                // Debug message to confirm collision with enemy
                Debug.Log("Bullet hit an enemy: " + hit.collider.gameObject.name);

                // Start the destruction effects coroutine
                StartCoroutine(EnemyDestructionEffects(hit.collider.gameObject));
            }
            else if (hit.collider.CompareTag("Ground"))
            {
                // Debug message to confirm collision with ground
                Debug.Log("Bullet hit the ground");

                // Destroy the bullet when it collides with the ground
                Destroy(gameObject);
            }
            else
            {
                // Destroy the bullet when it collides with another collider
                Destroy(gameObject);
            }
        }
    }

    // Method to set the direction of the bullet
    public void SetDirection(bool flipped)
    {
        isFlipped = flipped;
    }

    private IEnumerator EnemyDestructionEffects(GameObject enemy)
    {
        // Shrink the enemy before destroying it
        Vector3 originalScale = enemy.transform.localScale;
        float shrinkDuration = 0.5f; // Duration over which the enemy will shrink

        for (float t = 0; t < shrinkDuration; t += Time.deltaTime)
        {
            enemy.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t / shrinkDuration);
            yield return null;
        }

        // Ensure the enemy is completely scaled down
        enemy.transform.localScale = Vector3.zero;

        // Debug message to confirm enemy destruction
        Debug.Log("Destroying enemy: " + enemy.name);

        // Destroy the enemy object
        Destroy(enemy);

        yield return null;
    }
}
