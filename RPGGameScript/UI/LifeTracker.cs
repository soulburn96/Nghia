using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeTracker : MonoBehaviour
{
    // Start is called before the first frame update
        public Image healthFill;
        public Text health;
        public Text wavesText;
    void Start()
    {
        Inits();   
    }
    void OnDestroy()
    {
        UIManager.OnPlayerHealthChanged -= UpdateHealth;
        UIManager.OnWaveChanged -= UpdateWave;
    }
    void Inits()
    {
        UIManager.OnPlayerHealthChanged += UpdateHealth;
        UIManager.OnWaveChanged += UpdateWave;
    }
    void UpdateWave(int waves)
    {
        wavesText.text = "Enemies Alive : " + waves.ToString();
    }
    void UpdateHealth(int currentHealth, int maxHealth)
    {
        health.text = currentHealth.ToString();
        healthFill.fillAmount = (float)currentHealth / (float)maxHealth;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
