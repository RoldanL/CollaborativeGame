using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float speed = 10f; // Speed of the bullet
    public float lifetime = 1.2f; // Lifetime of the bullet before it's destroyed
    private bool isFlipped = false; // Indicates if the character is flipped

    private void Start()
    {
        // Set velocity to move the bullet in the correct direction
        float direction = isFlipped ? -1f : 1f;
        Vector2 velocity = new Vector2(speed * direction, 0f);

        // Apply velocity to the bullet
        GetComponent<Rigidbody2D>().velocity = velocity;

        // Destroy the bullet after a specified lifetime
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy the bullet when it collides with another collider
        Destroy(gameObject);
    }

    // Method to set the direction of the bullet
    public void SetDirection(bool flipped)
    {
        isFlipped = flipped;
    }
}
