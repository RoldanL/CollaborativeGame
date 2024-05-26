using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBigHealth : MonoBehaviour
{
    public int health = 3; // Initial health

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            // Reduce health and destroy if health is zero
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        health--;
        Debug.Log(gameObject.name + " health: " + health);

        if (health <= 0)
        {
            StartCoroutine(DestructionEffects());
        }
    }

    private IEnumerator DestructionEffects()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            // Dissolve the enemy by reducing its alpha value over time
            float dissolveDuration = 0.3f; // Duration of the dissolve effect
            float startAlpha = renderer.color.a;

            for (float t = 0; t < dissolveDuration; t += Time.deltaTime)
            {
                Color newColor = renderer.color;
                newColor.a = Mathf.Lerp(startAlpha, 0f, t / dissolveDuration);
                renderer.color = newColor;
                yield return null;
            }

            // Ensure the enemy is completely dissolved
            Color finalColor = renderer.color;
            finalColor.a = 0f;
            renderer.color = finalColor;
        }

        // Debug message to confirm enemy destruction
        Debug.Log("Destroying enemy: " + gameObject.name);

        // Destroy the enemy object
        Destroy(gameObject);
    }
}
