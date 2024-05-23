using UnityEngine;

public class agilityPotion : MonoBehaviour
{
    public float moveDistance = 3f; // Distance to move up and down
    public float moveSpeed = 5f; // Speed of the movement

    private Vector3 startPosition; // Initial position of the object
    private Vector3 endPosition; // Final position of the object
    private bool movingUp = true; // Flag to track movement direction

    void Start()
    {
        // Store the initial position of the object
        startPosition = transform.localPosition;
        // Calculate the end position based on the move distance
        endPosition = startPosition + Vector3.up * moveDistance;
    }

    void Update()
    {
        // Calculate the target position based on the current direction
        Vector3 targetPosition = movingUp ? endPosition : startPosition;

        // Move the potion towards the target position
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);

        // Check if the potion has reached the target position
        if (transform.localPosition == targetPosition)
        {
            // Toggle the movement direction
            movingUp = !movingUp;
        }
    }
}
