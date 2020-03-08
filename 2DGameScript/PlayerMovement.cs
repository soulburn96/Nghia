using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;

    float horizontalMove = 0f;

    public float runSpeed = 40f;

    bool jump = false;

    // Update is called once per frame
    void Update()
    {
        if (!controller.gameOver)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
                animator.SetBool("IsJumping", true);
            }
        }
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }
    private void FixedUpdate()
    {
        
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);

        jump = false;
    }
}
