using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject playerHand;
    public GameObject EquippedWeapon;
    IWeapon equippedWeapon;
    public CharacterStats characterStats;
    public Animator animator;
    public PlayerMovement movement;
    private BoxCollider box;
    void Start()
    {
        characterStats = GetComponent<Player>().characterStats;
        movement = GetComponent<PlayerMovement>();

        if(animator != null)
        {
            //animator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        }
        //movement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }
    public void EquipWeapon(Item itemToEquip)
    {
        /*EquippedWeapon = (GameObject)Instantiate(Resources.Load<GameObject>("Weapon/" + itemToEquip.ObjectSlug),
                playerHand.transform.position, playerHand.transform.rotation, playerHand.transform);
        */
        equippedWeapon = EquippedWeapon.GetComponent<IWeapon>();
        box = EquippedWeapon.GetComponent<BoxCollider>();
        Debug.Log(equippedWeapon);
        equippedWeapon.Stats = itemToEquip.Stats;
        EquippedWeapon.transform.SetParent(playerHand.transform);
        characterStats.AddStatBonus(itemToEquip.Stats);
    }

    public void PerformWeaponAttack()
    { 
        /*if(animator != null)
        {
            animator.SetTrigger("Attacking");
        }*/
        equippedWeapon.PerformAttack(CalculateDamage());
    }
    private int CalculateDamage()
    {        
        int damageToDeal = (characterStats.GetStat(BaseStat.BaseStatType.Power).GetCalculatedStatValue())
            + Random.Range(2, 8);
        Debug.Log("Damage dealt: " + damageToDeal);
        return damageToDeal;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            box.enabled = true;
            StartCoroutine(Attacking());

        }
    }
    IEnumerator Attacking()
    {
        PerformWeaponAttack();
        if (animator != null)
        {
            animator.SetInteger("condition", 2);
            movement.isAttacking = true;            
            yield return new WaitForSeconds(1f);
            movement.isAttacking = false;
            box.enabled = false;
            animator.SetInteger("condition", 0);
        }
    }
}
