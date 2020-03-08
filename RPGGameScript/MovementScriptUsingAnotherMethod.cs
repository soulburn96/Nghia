using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NghiaScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] ParticleSystem dashParticle =null;
    float speed = 10f;
    public Animator anim;
    public WeaponController wpController;
    public bool isAttacking = false;
    bool canDash = true;
    void Start()
    {
        wpController = this.GetComponent<WeaponController>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        HandleRotationInput();
        HandleDash();
    }
    void HandleMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
        if (movement != Vector3.zero&&!isAttacking)
        {
                transform.Translate(movement * speed * Time.deltaTime, Space.World);
                anim.SetInteger("condition", 1);
        }
        else
        {
            anim.SetInteger("condition", 0);
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
        if (Input.GetKeyDown(KeyCode.Space)&&!isAttacking)
        {
            StartCoroutine(Dash());
        }
    }
    IEnumerator Dash()
    {
        if (canDash)
        {
            float dashDistance = 15f;
            transform.position += transform.forward * dashDistance;

            dashParticle.Play();
        }
        canDash = false;
        yield return new WaitForSeconds(2f);
        canDash = true;
        
    }
}
