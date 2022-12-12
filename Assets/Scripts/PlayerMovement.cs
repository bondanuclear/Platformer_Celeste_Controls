using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerMovementSpeed = 10f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] Vector2 movDir = new Vector2();
    new Rigidbody2D rigidbody2D;
    [SerializeField] bool isGrounded = true;
    [Header("Inputs checking")]
    [SerializeField] float horizontalInput;
    [SerializeField] float verticalInput;
    [SerializeField] bool hasJumped;
    [Header("Ground Touch Check")]
    [SerializeField] Transform raySpawnPoint;
    [SerializeField] float rayLength = 2f;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float jumpFallOff = 8f;
    [SerializeField] float fallMultiplyer = 3f;
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        TickInput();
        CheckGroundTouch();
        FallingGravity();
        //Debug.Log(rigidbody2D.velocity + " rigidbody 2d velocity");
        movDir = new Vector2(horizontalInput, verticalInput);
        
    }

    private void FallingGravity()
    {
        
        if(rigidbody2D.velocity.y < jumpFallOff || !hasJumped && rigidbody2D.velocity.y > 0)
        {
            rigidbody2D.velocity += Vector2.down * 9.8f * Time.deltaTime * fallMultiplyer;
        }
    }

    private void CheckGroundTouch()
    {
        if(Physics2D.Raycast(raySpawnPoint.position, Vector2.down, rayLength, groundLayerMask))
        {
            Debug.Log("Touching the ground ");
            isGrounded = true;
        } else 
        {
            isGrounded = false;
        }
    }

    private void TickInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        hasJumped = Input.GetKey(KeyCode.Space);
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleJumping();
    }

    private void HandleMovement()
    {
        rigidbody2D.velocity = new Vector2(horizontalInput * Time.fixedDeltaTime * playerMovementSpeed, rigidbody2D.velocity.y);
    }

    private void HandleJumping()
    {
        if (hasJumped && isGrounded)
        {
            Debug.Log("Should Jump");
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
            isGrounded = false;
        } 
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(raySpawnPoint.position, Vector3.down * rayLength);
    }
}
