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
    [Header("Grabbing Variables")]
    public RaycastHit hit;
    public GameObject grabbedObject;
    private RigidbodyInterpolation objInterpolation;
    private bool isGrabbing = false;
    public float grabRange = 8f;
    public float pushForce = 5f;
    public float holdDistance = 1.5f;
    private Rigidbody grabbedRb;
    private Vector3 objectOffset;
    Vector3 directionToTarget;
    public GameObject grabby;
    private Collider grabbedCollider;
    //[Header("Controls")]
    public string horizontalControl;
    public string verticalControl;
    public string jumpButton;

    [Header("Health/Slapping Variables")]
    public Rigidbody[] bodyParts;  // All the body part rigidbodies for the ragdoll
    public Collider[] colliders;  // Colliders for ragdoll body parts
    public Animator animator;     // Animator of the character
    private int hitCount = 10;     // Counter for how many hits the character has taken
    public int maxHits = 10;      // Maximum hits before ragdolling
    public float resetTime = 10f; // Time to reset the character after ragdolling

    private bool isRagdoll = false; // Flag to check if the character is in ragdoll mode

    // Start is called before the first frame update
    void Start()
    {
        // Ensure body parts and colliders are set up (you can manually assign these in the inspector)
        bodyParts = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
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
        Debug.DrawRay(transform.position, transform.forward * grabRange, Color.red);
        if (Input.GetButtonDown("Grab"))
        {
            Debug.Log("Input");
            FaceObject();
            GrabObject();

            //      TryGrabObject();
        }

        if (isGrabbing) MoveGrabbedObject();

        if (Input.GetButtonUp("Grab"))
        {
            ReleaseObject();
        }
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

        Debug.Log("IsGrounded[" + isGrounded.ToString() + "] IsJumping[" + Input.GetButton("Jump").ToString() + "]");
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
    private void GrabObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, grabRange))
        {
            if (hit.collider.CompareTag("Player")) // Object must have "obj" tag
                Debug.Log("Button pressed");


            grabbedRb = hit.collider.GetComponent<Rigidbody>();
            grabbedCollider = hit.collider; // Store object collider

            if (grabbedRb)
            {

                //  hit.transform.SetParent(grabby.transform, true);
                grabbedRb.useGravity = false;
                grabbedRb.freezeRotation = true;
                grabbedRb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth Movement
                grabbedRb.isKinematic = true; // Prevents physics interactions while held
                Physics.IgnoreCollision(grabbedCollider, GetComponent<Collider>(), true); // Prevents pushing player
                isGrabbing = true;
                maxSpeed = 2.5f;

            }


            if (Input.GetButtonUp("Grab"))
            {
                grabbedObject.GetComponent<Rigidbody>().interpolation = objInterpolation;
                grabbedObject = null;
            }
        }
    }





    private void MoveGrabbedObject()
    {
        if (grabbedRb)
        {
            Vector3 targetPosition = transform.position + transform.forward * holdDistance;
            grabbedRb.position = Vector3.Lerp(grabbedRb.position, targetPosition, Time.deltaTime * maxSpeed);
        }
    }

    private void FaceObject()
    {
        if (grabbedRb)
        {
            Vector3 directionToTarget = (grabbedRb.position - rb.position).normalized;
            directionToTarget.y = 0; // Prevents looking up/down

            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotateSpeed);
            }
        }
    }

    private void PushObject()
    {
        if (grabbedRb)
        {
            grabbedRb.isKinematic = false; // Reactivate physics for push
            grabbedRb.AddForce(transform.forward * pushForce, ForceMode.Impulse);
            ReleaseObject(); // Let go after pushing
        }
    }

    private void ReleaseObject()
    {
        if (grabbedRb)
        {
            grabbedRb.useGravity = true;
            grabbedRb.freezeRotation = false;
            grabbedRb.isKinematic = false; // Restore physics
            Physics.IgnoreCollision(grabbedCollider, GetComponent<Collider>(), false); // Restore collision
            grabbedRb.velocity = Vector3.zero;
            grabbedRb = null;
            grabbedCollider = null;
            isGrabbing = false;
        }
    }



    // Call this method when the character is hit
    public void OnHit()
    {
        hitCount++;

        if (hitCount >= maxHits && !isRagdoll)
        {
            // Enable ragdoll when hit count reaches maxHits
            EnableRagdoll();

            // Start the reset coroutine
            StartCoroutine(ResetAfterDelay());
        }
    }

    // Enable ragdoll physics
    void EnableRagdoll()
    {
        // Disable the animator to prevent it from interfering with ragdoll physics
        if (animator != null)
        {
            animator.enabled = false;
        }

        // Enable ragdoll (set Rigidbody to non-kinematic)
        foreach (Rigidbody rb in bodyParts)
        {
            rb.isKinematic = false;
        }

        // Enable colliders for all body parts
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        isRagdoll = true;
    }

    // Disable ragdoll and reset the character
    void DisableRagdoll()
    {
        // Re-enable the animator
        if (animator != null)
        {
            animator.enabled = true;
        }

        // Set Rigidbody to kinematic (back to normal state)
        foreach (Rigidbody rb in bodyParts)
        {
            rb.isKinematic = true;
        }

        // Disable colliders for ragdoll parts
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        isRagdoll = false;
    }

    // Coroutine to reset the character after the ragdoll knock-out
    IEnumerator ResetAfterDelay()
    {
        // Wait for the specified reset time
        yield return new WaitForSeconds(resetTime);

        // Reset the character to standing position and disable ragdoll
        DisableRagdoll();

    }

    // Reset the character's position or state after ragdoll
  
}