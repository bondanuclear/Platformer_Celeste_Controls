using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Inputs checking")]
    public Vector2 velocityChecker;
    public float HorizontalInput {get; private set;}
    public float VerticalInput { get; private set; }
    [SerializeField] bool hasJumped;
    public bool JumpFlag {get; private set;}
    public bool DashFlag {get; private set;}
   
    public void TickInput()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");
        JumpFlag = Input.GetKeyDown(KeyCode.C);
        
        if (Input.GetKeyDown(KeyCode.B)) { DashFlag = true; }
        else DashFlag = false;

        
        //Debug.Log("DashFlag " + DashFlag);
    }
    // private void Update() {
    //     TickInput();
    // }
}
