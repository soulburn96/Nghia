using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cactus : MonoBehaviour, IEnemy
{
    public float currentHealth;
    public float maxHealth;
    private NavMeshAgent agent;
    private CharacterStats characterStats;
    GameObject playerHolder;
    Player player;
    public HealthBar healthBar;
    public DamagePopup damagePopup;
    bool canAttack = true;
    public int strength=35;
    void Awake()
    {
        characterStats = new CharacterStats(strength, 10, 2);
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        currentHealth = maxHealth;
        playerHolder = GameObject.Find("Player");
        player = playerHolder.GetComponent<Player>();
        //player = GameObject.Find("Player");
        healthBar.SetMaxHealth((int)maxHealth);
    }

    void FixedUpdate()
    {
        if(player.death != true)
        {
            ChasePlayer(player);
            FaceTarget();
        }
    }

    public void PerformAttack()
    {       
        player.TakeDamage(characterStats.GetStat(BaseStat.BaseStatType.Power).GetCalculatedStatValue());
        float dashDistance = 3f;
        player.transform.position += transform.forward * -1 * dashDistance;
        StartCoroutine(AttackCooldown());
    }
    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(1);
        canAttack = true;
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        damagePopup.Create(transform.position, amount);
        healthBar.SetHealth((int)currentHealth);

        float dashDistance = 10f;
        gameObject.transform.position -= transform.forward * -1 * dashDistance;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ChasePlayer(Player player)
    {
        agent.SetDestination(player.transform.position);
        this.player = player;
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= agent.stoppingDistance +2f && canAttack && !player.invulnerable) 
        {
            PerformAttack();
        }
    }
    void Die()
    {
        Destroy(gameObject);
    }
    void FaceTarget()
    {
        Vector3 direction = (transform.position - player.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
