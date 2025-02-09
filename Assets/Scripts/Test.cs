using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MovableObject : MonoBehaviour
{
    private Rigidbody rb;
    public float dragSpeed = 10f;

    // Reference to the player
    public Transform player;

    // Store if object is grabbed
    public bool isGrabbed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isGrabbed)
        {
            // Get the movement direction based on player input
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            // Get direction based on input
            Vector3 playerDirection = new Vector3(moveX, 0f, moveZ).normalized;

            if (playerDirection.magnitude > 0f)
            {
                // Move the object along with the player
                Vector3 targetPosition = player.position + playerDirection * dragSpeed * Time.deltaTime;
                rb.MovePosition(targetPosition);
            }

            // Optionally, you can also set the object's position to the player's position directly
            // Without using forces to avoid physics interactions that might cause the object to be dropped
            // transform.position = player.position;
        }
    }
}