using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CharacterStats characterStats;
    public int currentHealth;
    public int maxHealth ;
    public bool death = false;
    public int level;
    public int statCounter;
    public bool invulnerable = false;
    CharacterStats statPoints;
    PlayerMovement playerMovement;
    void Awake()
    {
        statCounter = 1;
        level = 0;
        invulnerable = false;
        this.currentHealth = this.maxHealth;
        characterStats = new CharacterStats(10, 10, 10);
        playerMovement = GetComponent<PlayerMovement>();
        UIManager.PlayerLevelChanged(level);
    }
    public void LevelUp()
    {
        statCounter++;
        level++;
        UIManager.HealthChanged(currentHealth,maxHealth);
        UIManager.PlayerLevelChanged(level);
        UIManager.PlayerStatCounter();
    }
    public void AddPower()
    {
        if(statCounter > 0)
        {
            statPoints = new CharacterStats(10, 0, 0);
            characterStats.AddStatBonus(statPoints.stats);
            UIManager.StatChange();
            playerMovement.updateStats();
            statCounter--;
            UIManager.PlayerStatCounter();

        }
    }
    public void AddToughness()
    {
        
        if (statCounter > 0)
        {
            statPoints = new CharacterStats(0, 2, 0);
            characterStats.AddStatBonus(statPoints.stats);
            UIManager.StatChange();
            playerMovement.updateStats();
            statCounter--;
            UIManager.PlayerStatCounter();

        }
    }
    public void AddSpeed()
    {
        if (statCounter > 0)
        {
            statPoints = new CharacterStats(0, 0, 5);
            characterStats.AddStatBonus(statPoints.stats);
            UIManager.StatChange();
            playerMovement.updateStats();
            statCounter--;
            UIManager.PlayerStatCounter();
        }
    }
    public void TakeDamage(int amount)
    {
        amount = amount-characterStats.GetStat(BaseStat.BaseStatType.Toughness).GetCalculatedStatValue();
        currentHealth -= amount;
        UIManager.HealthChanged(this.currentHealth, this.maxHealth);
        counter();
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    IEnumerator counter()
    {
        invulnerable = true;
        yield return new WaitForSeconds(0.5F);
        invulnerable = false;
    }
    public void Die()
    {
        death = true;
        UIManager.Instance.Defeat();
        Debug.Log("Player dead. Reset health.");
        //Destroy(gameObject);
    }
}
