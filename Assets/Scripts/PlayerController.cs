using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;

    [Header("Movement Settings")]
    public float maxSpeed = 5;
    public float accelerateTime = 0.2f;
    public float decelerateTime = 0.2f;
    float acceleration;
    float deceleration;
    public float rotateSpeed;
    Quaternion currentRotation;

    [Header("Jump Settings")]
    float gravity;
    float initialJumpSpeed;
    public float apexHeight = 3f;
    public float apexTime = 0.5f;
    public float maxVelocity = 15f;

    [Header("Ground Check Settings")]
    bool isGrounded = false;
    public float groundCheckOffset = 0.5f;
    public Vector3 groundCheckSize = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;

    //[SerializeField]
    Vector3 velocity;

    //[Header("Controls")]
    public string horizontalControl;
    public string verticalControl;
    public string jumpButton;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        //rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent rolling but also stops movement uh oh
        //currentRotation = transform.rotation;

        gravity = -2 * apexHeight / (Mathf.Pow(apexTime, 2));
        initialJumpSpeed = 2 * apexHeight / apexTime;

        acceleration = maxSpeed / accelerateTime;
        deceleration = maxSpeed / decelerateTime;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForGround();

        //OnDrawGizmos();
    }

    private void FixedUpdate()
    {
        velocity = rb.velocity;
        RotatePlayer();
        Vector3 playerInput = new Vector3(Input.GetAxisRaw(horizontalControl), 0, Input.GetAxisRaw(verticalControl));

        MovementUpdate(playerInput);

        JumpUpdate();

        rb.velocity = new Vector3(velocity.x, velocity.y, velocity.z);
    }

    private void MovementUpdate(Vector3 playerInput)
    {
        velocity.x = CalculateMovementInput(playerInput.x, velocity.x);
        velocity.z = CalculateMovementInput(playerInput.z, velocity.z);
    }

    private float CalculateMovementInput(float input, float velocity)
    {
        if (input != 0)
        {
            velocity += acceleration * input * Time.fixedDeltaTime;
            velocity = Mathf.Clamp(velocity, -maxSpeed, maxSpeed);
        }
        else
        {
            velocity = Mathf.MoveTowards(velocity, 0, deceleration * Time.fixedDeltaTime);
        }
        return velocity;
    }

    private void RotatePlayer()
    {
        Vector3 movementDirection = new Vector3(velocity.x, 0, velocity.z);

        if (movementDirection.sqrMagnitude > 0.0001f) // Only update rotation when moving
        {
            currentRotation = Quaternion.LookRotation(movementDirection.normalized, Vector3.up);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, currentRotation, Time.deltaTime * rotateSpeed);
    }

    private void JumpUpdate()
    {
        if (!isGrounded)
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
        else
        {
            velocity.y = -0.1f;
        }

        Debug.Log("IsGrounded["+ isGrounded.ToString()+ "] IsJumping["+ Input.GetButton("Jump") .ToString()+ "]");
        if (isGrounded && Input.GetButton(jumpButton))
        {
            Debug.Log("Jump!");
            velocity.y = initialJumpSpeed;
            isGrounded = false;
        }

        // Clamp falling speed to terminal velocity
        velocity.y = Mathf.Max(velocity.y, -maxVelocity);
    }

    private void CheckForGround()
    {
        //Debug.DrawLine(transform.position + Vector3.down * groundCheckOffset, transform.position + Vector3.down * groundCheckOffset - Vector3.down * groundCheckSize.y / 2, Color.red);
        isGrounded = Physics.CheckBox(transform.position + Vector3.down * groundCheckOffset, groundCheckSize / 2, Quaternion.identity, groundCheckMask.value); //if physics box collides with the ground, player is grounded
    }

    void OnDrawGizmos()
    {
        // Define the position of the box (down from the player's position)
        Vector3 boxPosition = transform.position + Vector3.down * groundCheckOffset;

        // Visualize the box with Gizmos at the check position
        Gizmos.color = Color.green; // Set the color for the gizmo
        Gizmos.DrawWireCube(boxPosition, groundCheckSize); // Draw a wireframe box
    }
}
