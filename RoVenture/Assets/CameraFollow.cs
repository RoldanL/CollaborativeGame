using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // Reference to the player's transform
    public float smoothSpeed = 0.125f; // How quickly the camera will follow

    public Vector3 offset = new Vector3(0f, 10f, -10f);
    private Vector3 velocity = Vector3.zero;


    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        }
    }
}
