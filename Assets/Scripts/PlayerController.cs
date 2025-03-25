using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]
    [Header("Movement Settings")]
    public float maxSpeed = 5;
    public float accelerateTime = 0.2f;
    public float decelerateTime = 0.2f;
    float acceleration;
    float deceleration;
    public float rotateSpeed;
    Quaternion currentRotation;
    Vector3 playerInput;

    [SerializeField]
    [Header("Jump Settings")]
    float gravity;
    float initialJumpSpeed;
    public float apexHeight = 3f;
    public float apexTime = 0.5f;
    public float maxVelocity = 15f;

    [SerializeField]
    [Header("Ground Check Settings")]
    bool isGrounded = false;
    public float groundCheckOffset = 0.5f;
    public Vector3 groundCheckSize = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;

    [SerializeField]
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
    public GameObject grabby;
    private Collider grabbedCollider;

    [SerializeField]
    [Header("Controls")]
    public string horizontalControl;
    public string verticalControl;
    public string jumpButton;
    public string leftArm;
    public string rightArm;
    public string slapL;
    public string slapR;
    public string lookHorizontal;
    public string lookVertical;

    [SerializeField]
    [Header("Health/Slapping Variables")]
    public Rigidbody[] bodyParts;  // All the body part rigidbodies for the ragdoll
    public Collider[] colliders;  // Colliders for ragdoll body parts
    public Animator animator;     // Animator of the character
    public float resetTime = 10f; // Time to reset the character after ragdolling
    private Vector2 lookInput;
    float rotationSpeed = 5f;
    private bool isRagdoll = false; // Flag to check if the character is in ragdoll mode
    public float hitCount = 0;     // Counter for how many hits the character has taken
    public float maxHitCount = 10;
    public GameObject leftSlapCollider;
    public GameObject rightSlapCollider;
    public Animator ani;
    public bool isLeftSlapping;
    public bool isRightSlapping;
    private float slapTimer = 0f;
    private bool isSlapping = false;
    public AudioManager audioManager;
    // Start is called before the first frame update
    float timer;

    PlayerController otherPlayer;
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        gravity = -2 * apexHeight / (Mathf.Pow(apexTime, 2));
        initialJumpSpeed = 2 * apexHeight / apexTime;
        acceleration = maxSpeed / accelerateTime;
        deceleration = maxSpeed / decelerateTime;
        leftSlapCollider.SetActive(false);
        rightSlapCollider.SetActive(false);
    }

    // Update is called once per frame
    private void Awake()
    {
       audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Update()
    {
        Debug.Log(Input.GetButton(rightArm));
        CheckForGround();
        Attack();
        SlappedOut();
        if (grabbedRb != null) //THROWING WHEN GRABBED.
        {
            if (Input.GetButtonDown(rightArm))
            {
                Debug.Log("Pressed");
                PushObject();
            }
        }
        Debug.DrawRay(transform.position, transform.forward * grabRange, Color.red);
        if (Input.GetButtonDown(leftArm))
        {
            Debug.Log("Input");

            GrabObject();
           
            //      TryGrabObject();
        }

        if (isGrabbing) MoveGrabbedObject();

        if (Input.GetButtonUp(leftArm) || otherPlayer.hitCount < 10) 
        {
            ReleaseObject();
        }
   
       

        //OnDrawGizmos();
        if (hitCount > maxHitCount)
        {
            float timer = +Time.deltaTime;
            if (timer > 10)
            {
                hitCount = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        if (hitCount != maxHitCount)
        {
            Rotate();
            if (hitCount <= maxHitCount)
            {
                velocity = rb.velocity;
                if (lookInput.sqrMagnitude <= 2.0f)
                {
                    RotatePlayer();
                }
                playerInput = new Vector3(Input.GetAxisRaw(horizontalControl), 0, Input.GetAxisRaw(verticalControl));

                MovementUpdate(playerInput);
                lookInput = new Vector2(Input.GetAxis(lookHorizontal), Input.GetAxis(lookVertical));

                // Apply dead zone to prevent stick drift
                if (lookInput.magnitude < 0.2f)
                    lookInput = Vector2.zero;

                JumpUpdate();


                rb.velocity = new Vector3(velocity.x, velocity.y, velocity.z);
            }
        }
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
            audioManager.PlaySFX(audioManager.Jumping);
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
            if (hit.collider.CompareTag("Downed") || hit.collider.CompareTag("Grabbable"))
            {// Object must have "obj" tag
                Debug.Log("Button pressed");


                grabbedRb = hit.collider.GetComponent<Rigidbody>();
                grabbedCollider = hit.collider; // Store object collider
                otherPlayer = grabbedRb.GetComponent<PlayerController>();
                if (grabbedRb)
                {

                    //  hit.transform.SetParent(grabby.transform, true);
                    grabbedRb.useGravity = false;
                    grabbedRb.freezeRotation = true;
                    grabbedRb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth Movement
                    grabbedRb.isKinematic = true; // Prevents physics interactions while held
                    Physics.IgnoreCollision(grabbedCollider, GetComponent<Collider>(), true); // Prevents pushing player
                    isGrabbing = true;

                   
                }


                if (Input.GetButtonUp(leftArm))
                {
                    grabbedObject.GetComponent<Rigidbody>().interpolation = objInterpolation;
                    grabbedObject = null;
                }
            }
        }
    }



  void SlappedOut()
    {
       
        
        if(hitCount>=maxHitCount && timer<10)
        {
            gameObject.tag = "Downed";
            Debug.Log(timer);
            timer += Time.deltaTime;
            playerInput = transform.position;

        }
        if (timer >=10 )
        {
            timer = 0;
            hitCount = 0;
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
    
    private void PushObject()
    {
        float verticalBoost = 4f;
        float throwRange = 4f;
          audioManager.PlaySFX(audioManager.Grab);
            grabbedRb.isKinematic = false; // Reactivate physics for push
        Vector3 throwDirection = transform.forward * throwRange;
        throwDirection.y = 0; // Ensure knockback is mostly horizontal
        Vector3 finalForce = (throwDirection * throwRange) + (Vector3.up * verticalBoost);
        grabbedRb.AddForce(finalForce, ForceMode.Impulse);
     //   grabbedRb.AddForce(transform.forward , ForceMode.Impulse);
            ReleaseObject(); // Let go after pushing
        
    } 

    private void ReleaseObject()
    {
        if (grabbedRb )
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

    void Rotate()
    {
        if (lookInput.sqrMagnitude > 0.01f) // Ensure there's input
        {
            Vector3 lookDirection = new Vector3(lookInput.x, 0, lookInput.y).normalized;

            if (lookDirection != Vector3.zero) // Prevent rotation errors
            {
                currentRotation  = Quaternion.LookRotation(lookDirection, Vector3.up);
                // rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime));
               // currentRotation = Quaternion.Slerp(transform.rotation, currentRotation,( rotationSpeed *3) * Time.deltaTime);
               transform.rotation = Quaternion.Slerp(transform.rotation, currentRotation, (rotationSpeed) * Time.deltaTime);

            }
        }
    }


    

    void Attack()
    {
        if (Input.GetButtonDown(slapL) )
        {
            ani.SetBool("leftArm", true);
            leftSlapCollider.SetActive(true);
            isSlapping = true;
            slapTimer = 0f; // Reset timer
        }

        if (Input.GetButtonDown(slapR))
        {
            ani.SetBool("rightArm", true);
            rightSlapCollider.SetActive(true);
            isSlapping = true;
            slapTimer = 0f; // Reset timer
        }

        if (isSlapping)
        {
            slapTimer += Time.deltaTime;

            if (slapTimer >= 0.2f) // Stop animation after 1 second
            {
                ani.SetBool("leftArm", false);
                ani.SetBool("rightArm", false);
                leftSlapCollider.SetActive(false);
                rightSlapCollider.SetActive(false);
                isSlapping = false; // Allow next slap
            }
        }
    }
}


