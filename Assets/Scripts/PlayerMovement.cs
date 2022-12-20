using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    enum State {
        Walking,
        Dashing
    }
    [SerializeField] float playerMovementSpeed = 10f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] Vector2 movDir = new Vector2();
    new Rigidbody2D rigidbody2D;
    State state;
    [SerializeField] bool isGrounded = true;
    [Header("Inputs checking")]
    [SerializeField] Vector2 velocityChecker;
    [SerializeField] float horizontalInput;
    [SerializeField] float verticalInput;
    [SerializeField] bool hasJumped;
    
    [Header("Ground Touch Check")]
    [SerializeField] Transform raySpawnPoint;
    [SerializeField] float rayLength = 2f;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float jumpFallOff = 8f;
    [SerializeField] float fallMultiplyer = 3f;
    [Header("Dash settings")]
    [SerializeField] float dashPower = 10f;
    [SerializeField] float smoothDashPower = 250f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] bool canDash = true;
    bool hasUsedDash = false;
    float timeSinceLastDash = Mathf.Infinity;
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        state = State.Walking;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case State.Walking:
                TickInput();
                CheckGroundTouch();
                CheckSpriteRotation();
                HandleJumping();
            
                break;
            case State.Dashing:
                float downMultiplyer = 3f;
                smoothDashPower -= smoothDashPower * downMultiplyer * Time.deltaTime;
                float minimumDashSpeed = 6f;
                if (smoothDashPower <= minimumDashSpeed)
                {
                    state = State.Walking;
                }
                break;
        }
        //Debug.Log(rigidbody2D.velocity + " rigidbody 2d velocity");
        movDir = new Vector2(horizontalInput, verticalInput);
    }
    private void FixedUpdate() {
        
        switch(state)
        {
            case State.Walking: 
                HandleMovement();  
                break;
            case State.Dashing:
                HandleSmoothDash();
                break;
        }
        FallingGravity();
        timeSinceLastDash += Time.deltaTime;
    }
    private void TickInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.B) && canDash)
        {
           
            smoothDashPower = 10f;
            state = State.Dashing;
        }
        velocityChecker = rigidbody2D.velocity;
        hasJumped = Input.GetKeyDown(KeyCode.Space);
        canDash = (isGrounded || (!isGrounded && !hasUsedDash)) && (timeSinceLastDash > dashCooldown);
        
        
    }
    private void CheckSpriteRotation()
    {
        if(movDir.x > 0) transform.localScale = Vector3.one;
        else if (movDir.x < 0) transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z); 
    }

    private void FallingGravity()
    {
        if (rigidbody2D.velocity.y < jumpFallOff || !Input.GetKey(KeyCode.C) && rigidbody2D.velocity.y > 0)
        {
            
            rigidbody2D.velocity += Vector2.down * 9.8f * Time.fixedDeltaTime * fallMultiplyer;
        }
        
    }

    private void CheckGroundTouch()
    {
        if(Physics2D.Raycast(raySpawnPoint.position, Vector2.down, rayLength, groundLayerMask))
        {
            //Debug.Log("Touching the ground ");
            isGrounded = true;
            hasUsedDash = false;
        } else 
        {
            isGrounded = false;
        }
    }

    


    private void HandleMovement()
    {
        rigidbody2D.velocity = new Vector2(horizontalInput * Time.fixedDeltaTime * playerMovementSpeed, rigidbody2D.velocity.y);
        
    }

    private void HandleJumping()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            if (isGrounded)
            {
                //Debug.Log("Should Jump");
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
                isGrounded = false;
            }
        }
       
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(raySpawnPoint.position, Vector3.down * rayLength);
    }
    private void HandleSmoothDash()
    {   

        if(canDash)
        {
            rigidbody2D.velocity = movDir.normalized * smoothDashPower;
            hasUsedDash = true;
            timeSinceLastDash = 0;
        }
        
    }


    // deprecated
    private void HandleDash()
    {
        Debug.Log($"Should dash in {movDir} direction");
        // teleporting
        rigidbody2D.MovePosition((Vector2)transform.position + movDir.normalized * dashPower);
    }
}
