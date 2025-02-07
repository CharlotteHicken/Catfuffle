using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerNew : MonoBehaviour
{
    Rigidbody rb;

    private PlayerControls playerControls;

    InputActionMap player;

    InputAction walkAction;
    InputAction jumpAction;

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
    Vector3 forceDirection;


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

        playerControls = new PlayerControls(); // Instantiate PlayerControls
        player = playerControls.Player; // Assign the "Player" action map

        // Bind actions (move and jump)
        walkAction = playerControls.Player.Walk; // Get the Walk action
        jumpAction = playerControls.Player.Jump; // Get the Jump action
    }

  

    private void OnEnable()
    {
        if (walkAction != null && jumpAction != null)
        {
            walkAction.Enable();
            jumpAction.Enable();

            // Subscribe to jump action (make sure we are using the correct event)
            jumpAction.started += JumpUpdate;  // Trigger JumpUpdate when the jump action starts
        }
        else
        {
            Debug.LogError("walkAction or jumpAction is null in OnEnable.");
        }
    }

    private void OnDisable()
    {
        if (jumpAction != null)
        {
            jumpAction.started -= JumpUpdate;
        }

        // Disable the actions to avoid unexpected behavior
        if (walkAction != null)
        {
            walkAction.Disable();
        }
        if (jumpAction != null)
        {
            jumpAction.Disable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckForGround();

        //OnDrawGizmos();
    }

    private void FixedUpdate()
    {
        forceDirection += walkAction.ReadValue<Vector2>().x * Vector3.right * maxSpeed;
        forceDirection += walkAction.ReadValue<Vector2>().y * Vector3.forward * maxSpeed;

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        if (rb.velocity.y < 0f)
            rb.velocity -= Vector3.down * gravity * Time.fixedDeltaTime;

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;

        LookAt();
    }



    private void LookAt()
    {
        Vector3 direction = rb.velocity;
        direction.y = 0f;

        if (walkAction.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        else
            rb.angularVelocity = Vector3.zero;
    }

    private void JumpUpdate(InputAction.CallbackContext obj)
    {
        if (isGrounded)
        {
            forceDirection += Vector3.up * initialJumpSpeed;
        }
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
