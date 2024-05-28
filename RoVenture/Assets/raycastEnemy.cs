using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycastEnemy : MonoBehaviour
{
    float direction = 1f;
    [SerializeField] private float moveSpeed = 6f;
    private Rigidbody2D rb;
   // private SpriteRenderer spriteRenderer;
   // private bool movingRight = true;
   // public float speed = 30f;
   // [SerializeField] private float distance = 2.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        // Start the movement coroutine
        //StartCoroutine(MoveRoutine());
    }

    private void Update()
    {
        Vector3 origin = new Vector3(transform.position.x + (0.275f * direction), transform.position.y, transform.position.z);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * direction, 0.15f);
        Debug.DrawRay(transform.position, (Vector2.right * 0.15f) * direction, Color.red);

        if (hit.collider!= null)
        {
            Debug.Log(hit.collider.name);

            if (hit.collider.CompareTag("Ground"))
            {
                flipDirection();
            }
        }
        
   
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
    
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            flipDirection();
        }
    }

    void flipDirection()
    {
        direction *= -1;
        if (direction > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (direction < 0) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

    }
}