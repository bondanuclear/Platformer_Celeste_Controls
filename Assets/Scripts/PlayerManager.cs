using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerInput), typeof(PlayerMovement))]
public class PlayerManager : MonoBehaviour
{
    PlayerInput playerInput;
    PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerInput();
        playerMovement.HandleMovementChecks();
    }
    private void FixedUpdate() {
        playerMovement.HandlePhysicsMovement();
    }
    private void HandlePlayerInput()
    {
        playerInput.TickInput();
    }

}
