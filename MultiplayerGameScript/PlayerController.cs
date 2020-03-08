using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // Use this for initialization
    Animator anim;
    Vector3 up = Vector3.zero;
    Vector3 right = new Vector3(0, 90, 0);
    Vector3 down = new Vector3(0, 180, 0);
    Vector3 left = new Vector3(0, 270, 0);    
    Vector3 nextPos; // tile direction base on player position
    Vector3 destination;// player's next position on the map
    public Vector3 currentDirection; // current player's direction 
    public GameObject spellFire; // spell's elements
    public GameObject spellWater;
    public GameObject spellLighting;
    public GameObject spellEarth;
    public PlayerStat playerStat;// 
    SpellMovement spellMovement;
    bool canAttack = true; //check if player can attack
    bool canRotate = true; //check if player can rotate
    bool canMove = true; //check if player can move
    bool rotating = false; //check if player is rotating
    public bool isDead = false; //check if player is dead
    public float stepDuration ; // player speed base on duration of each step taken
    public Coroutine playerMovement; // coroutine handle movement

    void Start()
    {  
        // setup default value
        nextPos = Vector3.forward;
		currentDirection = transform.rotation.eulerAngles;
        destination = transform.position;
        anim = GetComponent<Animator>();
        stepDuration = playerStat.speed;
    }

    // Update is called once per frame
    void Update()
    {
		if (!this.GetComponent<PhotonView>().isMine) return;
        Control();
		CastSpell();
    }
    //Handle player control
    void Control()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (anim.GetBool("Attacking") || isDead) //check attacking or dead
            {
                return;
            }
            else //if not then handle movement
            {
                if (playerMovement == null && canMove &&!rotating)
                {
                    playerMovement = StartCoroutine(Move(newPosition(currentDirection)));
                }
            }
        }
        if (canRotate && !rotating)
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentDirection += down; // set value of player direction to new direction
                if (currentDirection.y >= 360)
                {
                    currentDirection.y -= 360;
                }
                // value of new rotation in quaternion and handle Rotation
                Quaternion targetAngle = Quaternion.Euler(transform.rotation.x, transform.rotation.y+currentDirection.y, transform.rotation.z);
                StartCoroutine(Rotate(targetAngle,0.3f));

            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentDirection += right;
                if (currentDirection.y >= 360)
                {
                    currentDirection.y -= 360;
                }
                Quaternion targetAngle = Quaternion.Euler(transform.rotation.x, transform.rotation.y + currentDirection.y, transform.rotation.z);
                StartCoroutine(Rotate(targetAngle, 0.3f));

            }
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentDirection += left;
                if (currentDirection.y >= 360)
                {
                    currentDirection.y -= 360;
                }
                Quaternion targetAngle = Quaternion.Euler(transform.rotation.x, transform.rotation.y + currentDirection.y, transform.rotation.z);
                StartCoroutine(Rotate(targetAngle, 0.3f));
            }
        }
    }
    //Handle movement to given direction
    IEnumerator Move(Vector3 direction)
    {
        if (CanMove())//check if player can move
        {
            anim.SetBool("Walking", true);//set animation to walking
            anim.SetInteger("Condition", 1);
			playerStat.regenHealth();//each step regen player's HP
            //setup position for movement
            Vector3 startPosition = transform.position;
            Vector3 destinationPosition = transform.position + direction;
            float elapsedTime = 0.0f;
            //while moving can't attack or rotate
            canAttack = false;
            canRotate = false;
            //lerping movement overtime
            while (elapsedTime < 1.0f)
            {
                transform.position = Vector3.Lerp(startPosition, destinationPosition, elapsedTime);
                elapsedTime += Time.deltaTime / stepDuration;
                yield return new WaitForEndOfFrame();
            }
            //set value to default
            transform.position = destinationPosition;
            CancelAnimation();
            canAttack = true;
            canRotate = true;
            playerMovement = null;
        }
    }
    
    public Vector3 newPosition(Vector3 direction) // return next position base on direction
    {
        Vector3 newPos = new Vector3(0, 0, 0);
        if (direction == up)
        {
            newPos = Vector3.forward;
        }
        if (direction == right)
        {
            newPos = Vector3.right;
        }
        if (direction == left)
        {
            newPos = Vector3.left;
        }
        if (direction == down)
        {
            newPos = Vector3.back;
        }
        return newPos;
    }
    //handle Attack 
    void CastSpell() 
    {
        if (canAttack)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {    
                //Instantiate spell prefab in network
				PhotonNetwork.Instantiate(spellFire.name, transform.position + spellOffset(currentDirection), spellFire.transform.rotation, 0);
                //Handle Animation and cooldown for spell
                StartCoroutine(CorountineCastSpell(playerStat.spellCooldown));           
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
				PhotonNetwork.Instantiate(spellWater.name, transform.position + spellOffset(currentDirection), spellFire.transform.rotation, 0);
				StartCoroutine(CorountineCastSpell(playerStat.spellCooldown));
			}
			if (Input.GetKeyDown(KeyCode.O))
			{
				PhotonNetwork.Instantiate(spellLighting.name, transform.position + spellOffset(currentDirection), spellFire.transform.rotation, 0);
				StartCoroutine(CorountineCastSpell(playerStat.spellCooldown));
			}
			if (Input.GetKeyDown(KeyCode.P))
			{
				PhotonNetwork.Instantiate(spellEarth.name, transform.position + spellOffset(currentDirection), spellFire.transform.rotation, 0);
				StartCoroutine(CorountineCastSpell(playerStat.spellCooldown));
            }
        }
    }
    // Spell position for instantite base on player
	Vector3 spellOffset(Vector3 currentDirection) {
		return new Vector3(newPosition(currentDirection).x, newPosition(currentDirection).y + 0.5f , newPosition(currentDirection).z);
	}
    //Set animation to default value
	void CancelAnimation()
    {
        anim.SetBool("Running", false);
        anim.SetBool("Walking", false);
        anim.SetBool("Attacking", false);
        anim.SetInteger("Condition", 0);
    }
    //Handle attack animation
    IEnumerator CorountineCastSpell(int cooldownTime) 
    {
        canAttack = false;
        anim.SetInteger("Condition", 3);
        anim.SetBool("Attacking", true);
        yield return new WaitForSeconds(1.2f);
        CancelAnimation();
        yield return new WaitForSeconds(cooldownTime);
        canAttack = true;
    }
    //Check if any obstacle in front of player
    bool CanMove()
    {
        Ray myRay = new Ray(transform.position + new Vector3(0, 0.25f, 0), transform.forward);
        RaycastHit hit;
        Debug.DrawRay(myRay.origin, myRay.direction, Color.red);

        if(Physics.Raycast(myRay,out hit, 1f))
        {
            if (hit.collider.tag == "Wall")
            {
                return false;
            }
        }
        return true;
    }
    //Handle rotation
    IEnumerator Rotate(Quaternion targetRotation, float duration)
    {
        if (rotating)
        {
            yield break;
        }
        rotating = true;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsedTime / duration);
            yield return null;
        }
        rotating = false;

    }
}
