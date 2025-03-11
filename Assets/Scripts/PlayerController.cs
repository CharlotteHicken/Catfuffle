using NodeCanvas.Tasks.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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


    [Header("Grabbing and Pushing")]

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

    //[SerializeField]
    Vector3 velocity;


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
        Debug.DrawRay(transform.position, transform.forward * grabRange, Color.red);
        CheckForGround();
        if (Input.GetButtonDown("Grab"))
        {
            Debug.Log("Input");
            FaceObject();
            GrabObject();

      //      TryGrabObject();
        }

        if (isGrabbing)

            MoveGrabbedObject();

     
    

        if (Input.GetButtonUp("Grab"))
        {
            ReleaseObject();
}
    }

    private void FixedUpdate()
    {
        velocity = rb.velocity;
        RotatePlayer();
        Vector3 playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

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
        if (isGrounded && Input.GetButton("Jump"))
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
   [System.Serializable]
   public struct GrabDataLog
    {
       public Vector3 playerPosition;
       public Vector3 objPosition;
        public string gameObjectName;
    }

    private void GrabObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, grabRange))
        {
            if (hit.collider.CompareTag("obj")) // Object must have "obj" tag
                Debug.Log("Button pressed");
           

            grabbedRb = hit.collider.GetComponent<Rigidbody>();
            grabbedCollider = hit.collider;

            var data = new GrabDataLog()
            {
                playerPosition = transform.position,
                objPosition = grabbedRb.transform.position,
                gameObjectName = grabbedRb.name
            };
            TelemetryLogger.Log(this, "Grabbed Object", data);
            if (grabbedRb)
            {

               
                grabbedRb.useGravity = false;
                grabbedRb.freezeRotation = true;
                grabbedRb.interpolation = RigidbodyInterpolation.Interpolate; 
                grabbedRb.isKinematic = true; 
                Physics.IgnoreCollision(grabbedCollider, GetComponent<Collider>(), true); 
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

    void OnDrawGizmos()
    {
        // Define the position of the box (down from the player's position)
        Vector3 boxPosition = transform.position + Vector3.down * groundCheckOffset;

        // Visualize the box with Gizmos at the check position
        Gizmos.color = Color.green; // Set the color for the gizmo
        Gizmos.DrawWireCube(boxPosition, groundCheckSize); // Draw a wireframe box

        Vector3 temp = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 grabbedBoxPosition = transform.position + Vector3.forward * 0.5f;
        Gizmos.DrawWireCube(grabbedBoxPosition, temp);

    }
}

