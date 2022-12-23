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
    [SerializeField] State state;
    [SerializeField] bool isGrounded = true;
    [Header("Inputs checking")]
    PlayerInput playerInput;
    AnimatorController animatorController;
    // [SerializeField] Vector2 velocityChecker;
    // [SerializeField] float horizontalInput;
    // [SerializeField] float verticalInput;
    // [SerializeField] bool hasJumped;
    
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
    [Header("Koyote Jump settings")]
    // need to check if we free fall or jumping
    [SerializeField] float timeToCoyoteJump = 0.18f;
    [SerializeField] float coyoteCounter;
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        state = State.Walking;
        playerInput = GetComponent<PlayerInput>();
        animatorController = GetComponent<AnimatorController>();

    }

    public void HandleMovementChecks()
    {
        switch (state)
        {
            case State.Walking:
                HandleBooleans();
                CheckGroundTouch();
                CheckSpriteRotation();
                CheckCoyoteJump();
                if (playerInput.JumpFlag)
                {
                    Debug.Log("Should jump");
                    HandleJumping();
                }
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
        if(MathF.Abs(rigidbody2D.velocity.x) > 0.1f) animatorController.StartWalking();
        else animatorController.StopWalking();
        CheckLeftWallTouch();
        //Debug.Log(rigidbody2D.velocity + " rigidbody 2d velocity");
        movDir = new Vector2(playerInput.HorizontalInput, playerInput.VerticalInput);
        timeSinceLastDash += Time.deltaTime;
    }
    public void HandlePhysicsMovement()
    {
        switch (state)
        {
            case State.Walking:
                HandleMovement();
                break;
            case State.Dashing:
                HandleSmoothDash();
                break;
        }
        FallingGravity();
    }

    private void HandleBooleans()
    {
       
        if (playerInput.DashFlag && canDash)
        {
           
            smoothDashPower = 10f;
            state = State.Dashing;
        }
        playerInput.velocityChecker = rigidbody2D.velocity;
        
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
        bool touchingGround = Physics2D.Raycast(raySpawnPoint.position, Vector2.down, rayLength, groundLayerMask);
        if(touchingGround)
        {
            //Debug.Log("Touching the ground ");
            isGrounded = true;
            hasUsedDash = false;
        } 
        else 
        {
            isGrounded = false;
             
        }
        
    }
    private void CheckCoyoteJump()
    {
        if(isGrounded)
        {
            coyoteCounter = timeToCoyoteJump;
        } else if(!isGrounded)
        {
            coyoteCounter -= Time.deltaTime;
        }
        if(!Input.GetKey(KeyCode.C) && rigidbody2D.velocity.y > 0)
        {
            coyoteCounter = 0;
        }
    }
    private void HandleMovement()
    {
        rigidbody2D.velocity = new Vector2(playerInput.HorizontalInput * Time.fixedDeltaTime * playerMovementSpeed, rigidbody2D.velocity.y); 
    }

    private void HandleJumping()
    {
        
        if (coyoteCounter > 0)
        {
            
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
            //isGrounded = false;
        }
        
       
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(raySpawnPoint.position, Vector3.down * rayLength);
        Gizmos.DrawRay(raySpawnPoint.position, Vector3.left * distanceToWall);
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

    #region WallSliding 
    [SerializeField] float distanceToWall = 1f;
    [SerializeField] bool _touchingLeftWall;
    [SerializeField] bool _isClimbingUp = false;
    [SerializeField] bool _isSlidingDown = false;
    [SerializeField] LayerMask wallLayerMask;
    private void CheckLeftWallTouch()
    {
        bool leftWallTouched = Physics2D.Raycast(raySpawnPoint.position, Vector3.left, distanceToWall,
       wallLayerMask);
        if(leftWallTouched)
        {
            if(movDir.x < 0 && movDir.y > 0) {
                Debug.Log("i see.. you want to climb up");
                _isClimbingUp = true;
                _isSlidingDown = false;
            }
            else if(movDir.x < 0)
            {
                _isSlidingDown = true;
                _isClimbingUp = false;
                Debug.Log("weeee sliding doooown");
            }
            else if(movDir.x == 0)
            {
                _isClimbingUp = false;
                _isSlidingDown = false;
            }
            
            Debug.Log("Touching left wall");
        }
        else
        {
            _isClimbingUp = false;
            _isSlidingDown = false;
        }
     
    }
    #endregion
}
