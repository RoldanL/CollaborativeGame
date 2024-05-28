using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyPatrol : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool movingRight = true;
    [SerializeField] float speed = 30f;
    [SerializeField] private float distance = 2.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Start the movement coroutine
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        while (true) // Infinite loop for continuous movement
        {
            // Move right for 2 seconds
            if (movingRight)
            {
                rb.velocity = new Vector2(speed, rb.velocity.y);
                spriteRenderer.flipX = false; // Not flipped (facing right)
                yield return new WaitForSeconds(distance);
                movingRight = false;
            }
            // Move left for 2 seconds
            else
            {
                rb.velocity = new Vector2(-speed, rb.velocity.y);
                spriteRenderer.flipX = true; // Flipped (facing left)
                yield return new WaitForSeconds(distance);
                movingRight = true;
            }
        }
    }
}