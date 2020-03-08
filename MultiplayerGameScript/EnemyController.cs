using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    // Use this for initialization
    int hp = 100;
    Rigidbody rgbody;
	void Start () {
        rgbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        IsDead();
	}
    private void OnCollisionEnter(Collision collision)
    {
        
    }
    void IsDead()
    {
        if (hp <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
