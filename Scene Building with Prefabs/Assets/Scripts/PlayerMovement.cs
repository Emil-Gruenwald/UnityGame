using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpcooldown;
    // public float dashForce;
    // public float dashcooldown;
    public float airMultiplier;
    public int coyoteJump;
    bool readyToJump;
    bool readyToDash;
    bool Dash;

    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;


    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    public Transform orientation;

    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitSlope;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    public MovementState state;

    public enum MovementState
    {
        walking,
        sprinting,
        air
    }

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        readyToDash = true;
        Dash = false;
        coyoteJump = 0;
    }
    private void Update()
    {

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        MyInput();
        SpeedControl();
        StateHandler();

        if (grounded) 
        {
            rb.drag = groundDrag;
            coyoteJump = 0;
        }
        else 
        {
            rb.drag = 0;
            coyoteJump ++;
        }

        if (Dash) 
        {
            rb.AddForce(orientation.forward * 10, ForceMode.Impulse);
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(jumpKey) && readyToJump && (grounded || coyoteJump <= 20))
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpcooldown);
        }

        if (Input.GetKey(KeyCode.E) && readyToDash) 
        {
            readyToDash = false;

            initiateDash();

            Invoke(nameof(ResetDash), 2);
        }
    }
    private void StateHandler()
    {
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }
    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            
        rb.useGravity = !OnSlope();
    }
    private void SpeedControl()
    {
        if (OnSlope()&& !exitSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
       
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
        
    }
    private void Jump()
    {
        
        exitSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
        exitSlope = false;
    }
    private void initiateDash() 
    {
        Dash = true;
        Invoke(nameof(cancelDash), 0.3f);
        // rb.AddForce(orientation.forward * 100, ForceMode.Impulse);
    }
    private void cancelDash() 
    {
        Dash = false;
    }
    private void ResetDash()
    {
        readyToDash = true;
    }
    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;   
    }
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}