using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    [SerializeField] private Text health, level, statCounter;
    [SerializeField] private Player player;

    // Stats
    public Text playerPower;
    public Text playerDefense;
    public Text playerSpeed;
    private CharacterStats characterStats;

    void Start()
    {
        Init();
    }
    void Init()
    {
        UIManager.OnPlayerHealthChanged += UpdateHealth;
        UIManager.OnPlayerLevelChanged += UpdateLevel;
        UIManager.OnStatsChanged += UpdateStats;
        UIManager.OnPlayerStatPointsChanged += UpdateStatCounter;
        UpdateStats();
    }

    void OnDestroy()
    {
        UIManager.OnPlayerHealthChanged -= UpdateHealth;
        UIManager.OnPlayerLevelChanged -= UpdateLevel;
        UIManager.OnStatsChanged -= UpdateStats;
        UIManager.OnPlayerStatPointsChanged -= UpdateStatCounter;
    }
    void OnLevelWasLoaded(int level)
    {
        Init();
    }
    void UpdateHealth(int currentHealth, int maxHealth)
    {
        health.text = currentHealth.ToString();
        //this.healthFill.fillAmount = (float)currentHealth / (float)maxHealth;
    }

    void UpdateLevel(int levels)
    {
        level.text = levels.ToString();
        //this.levelFill.fillAmount = (float)player.PlayerLevel.CurrentExperience / (float)player.PlayerLevel.RequiredExperience;
    }

    void InitializeStats()
    {
        
    }
    void UpdateStatCounter()
    {
        statCounter.text = "Stat points : " + player.statCounter.ToString();
    }
    void UpdateStats()
    {    
        playerPower.text = "Power: " + player.characterStats.stats[0].GetCalculatedStatValue().ToString();
        playerDefense.text = "Defense: " + player.characterStats.stats[1].GetCalculatedStatValue().ToString();
        playerSpeed.text = "Speed: " + player.characterStats.stats[2].GetCalculatedStatValue().ToString();
    }
}
