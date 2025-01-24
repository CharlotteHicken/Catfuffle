using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float acceleration;
    float deceleration;
    public float maxSpeed = 5;
    public float accelerateTime = 1;
    public float decelerateTime = 1;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        acceleration = maxSpeed / accelerateTime;
        deceleration = maxSpeed / decelerateTime;
    }

    // Update is called once per frame
    void Update()
    {
        float leftRightInput = Input.GetAxis("Horizontal");
        float upDownInput = Input.GetAxis("Vertical");
        Vector3 inputValues = new Vector3(leftRightInput, 0, upDownInput);

        if (inputValues.magnitude != 0)
        {
            velocity += inputValues * acceleration * Time.deltaTime;
        }
        else
        {
            velocity -= velocity * deceleration * Time.deltaTime;
        }

        if (velocity.magnitude < 0.001f)
        {
            velocity = Vector3.zero;
        }

        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        transform.position += velocity * Time.deltaTime;
    }
}
