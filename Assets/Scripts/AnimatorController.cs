using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    Animator playerAnimator;
    // Start is called before the first frame update
    void Awake()
    {
        playerAnimator = GetComponent<Animator>();
    }

    public void StartWalking()
    {
        playerAnimator.SetBool("isWalking", true);
    }
    public void StopWalking()
    {
        playerAnimator.SetBool("isWalking", false);
    }
    private void Update() {
        
    }
}
