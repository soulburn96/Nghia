using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellMovement : MonoBehaviour {

    public float speed = 30f;
    PlayerController playerController;    
    public string spellElement;
    public int damage = 50;

    void Start ()
    {
        // set default value
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
		transform.rotation = Quaternion.Euler(playerController.currentDirection);
	}
	
	void Update () {

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
	}

	int getDamage(Collider collider) {
		// this method should return different number of damage depending on the elemnt of the collided player
		return 50;
	}
	
    private void OnCollisionEnter(Collision collision)
    {
		Debug.Log(collision.collider.tag + " " + collision.collider.name);
        //if spell collide with player
		if (collision.collider.CompareTag("Player")) {
			int playerHitId = collision.collider.GetComponent<PhotonView>().ownerId;
			PlayerStat enemyStat = collision.collider.GetComponent<PlayerStat>();
			collision.collider.GetComponent<PhotonView>().RPC("receiveDamage", PhotonTargets.Others, DamageCalculate(enemyStat), playerHitId);
			if (playerController.gameObject.GetComponent<PhotonView>().isMine) {
				PhotonNetwork.Destroy(gameObject);
			}
        }
        //if spell collide with wall 
        else if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Limiter")) {
			if (playerController.gameObject.GetComponent<PhotonView>().isMine) {
				PhotonNetwork.Destroy(gameObject);
			}
		}
    }
    // caculate damage base on collided player
    int DamageCalculate(PlayerStat playerStat)
    {
        if (playerStat == null)
        {
            return 0;
        }
        else
        {
            if(spellElement == playerStat.strongerElement)
            {
                
                return this.damage * 2;
            }
            else if (spellElement == playerStat.weakerElement)
            {
                return 0;
            }
            else
            {                
                return this.damage;
            }
        }
    }

}
