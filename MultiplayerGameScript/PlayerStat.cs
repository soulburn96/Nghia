using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    // Start is called before the first frame update   
    public Text hpIndicator;
    public int currentHealth;
    public string elementName;
    public int maxHp;
    public float speed;
    public int spellCooldown;
    public int hpRegen;
    public string strongerElement;
    public string weakerElement;
    Animator anim;
    void Awake  ()
    {
        //set value to default
        currentHealth = maxHp;
		hpIndicator = GameObject.Find("HpIndicator").GetComponent<Text>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //set current HP to indicator on screen
		if(gameObject.GetPhotonView().isMine) {
			hpIndicator.text = currentHealth.ToString();
		}
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            Die();
            anim.SetInteger("condition", 4);
        }
        Debug.Log(currentHealth);
    }

    public void Die()
    {
        Destroy(gameObject);
    }

	public void regenHealth() {
		if (currentHealth < maxHp) {
			currentHealth += hpRegen;
		}
	}
}
