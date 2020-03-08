using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] ParticleSystem dashParticle;
    public CharacterController controller;
    private CharacterStats characterStats;

    public float speed = 10f;
    public float gravity = -9.81f;
    public float dashdistance = 3f;
    public Animator animator;
    public bool isAttacking = false;
    bool canDash = true;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.4f;
    
    Vector3 velocity;
    Vector3 move;
    bool isGrounded;
    public int time = 0;
    void Start()
    {
        characterStats = GetComponent<Player>().characterStats;
    }
    public void updateStats()
    {

        speed = characterStats.GetStat(BaseStat.BaseStatType.AttackSpeed).GetCalculatedStatValue();
    }
    void FixedUpdate()
    {
        HandleMovementInput();
        HandleRotationInput();
        HandleDash();
    }
    void HandleMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal")* speed;
        float verticalInput = Input.GetAxis("Vertical")* speed;
        move = new Vector3(horizontalInput, 0, verticalInput);
        if (move != Vector3.zero && !isAttacking)
        {
            transform.Translate(move * Time.deltaTime, Space.World);
            animator.SetInteger("condition", 1);
        }
        else
        {
            animator.SetInteger("condition", 0);
        }
    }
    void HandleRotationInput()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }
    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            StartCoroutine(Dash());
        }
    }
    IEnumerator Dash()
    {

            float dashDistance = 10f;
            transform.position += transform.forward * dashDistance;

            dashParticle.Play();
      
        yield return new WaitForSeconds(2f);
    }
}
