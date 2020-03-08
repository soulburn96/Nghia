using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    Player playerHolder;
    public GameObject restartUI;
    public GameObject victoryUI;
    public GameObject statsUI;
    public GameObject waitUI;
    public static UIManager Instance { get; set; }

    public delegate void PlayerHealthEventHandler(int currentHealth, int maxHealth);
    public static event PlayerHealthEventHandler OnPlayerHealthChanged;

    public delegate void StatsEventHandler();
    public static event StatsEventHandler OnStatsChanged;

    public delegate void WaveEventHandler(int waves);
    public static event WaveEventHandler OnWaveChanged;

    public delegate void PlayerLevelEventHandler(int level);
    public static event PlayerLevelEventHandler OnPlayerLevelChanged;

    public delegate void PlayerEventStatCounter();
    public static event PlayerEventStatCounter OnPlayerStatPointsChanged;
    
    public static bool playerState = false;
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        player = GameObject.Find("Player");
        playerHolder = player.GetComponent<Player>();
    }
    public void Restart()
    {
        foreach (GameObject o in UnityEngine.Object.FindObjectsOfType<GameObject>())
        {
            Destroy(o);
        }
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }
    public void Victory()
    {
        victoryUI.SetActive(true);
    }
    public void Defeat()
    {
        restartUI.SetActive(true);
        Time.timeScale = 0;  
    }
    public void WaitForWave(bool active)
    {     
        if (active == true)
        {
            statsUI.SetActive(true);
            waitUI.SetActive(true);
        }
        if (active == false)
        {
            statsUI.SetActive(false);
            waitUI.SetActive(false);
        }
    }
    public void LevelUp()
    {
        playerHolder.LevelUp();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("tab"))
        {
            if (statsUI.activeSelf != false)
            {
                statsUI.SetActive(false);
            }
            else
            {
                statsUI.SetActive(true);
            }
        }
    }

    public static void HealthChanged(int currentHealth, int maxHealth)
    {
        if (OnPlayerHealthChanged != null)
        {
            OnPlayerHealthChanged(currentHealth, maxHealth);
        }
    }
    public static void PlayerLevelChanged(int level)
    {
        if (OnPlayerLevelChanged != null)
        {
            OnPlayerLevelChanged(level);
        }
    }
    public static void StatChange()
    {
        if (OnStatsChanged != null)
        {
            OnStatsChanged();
        }
    }

    public static void PlayerStatCounter()
    {
        if (OnPlayerStatPointsChanged != null)
        {
            OnPlayerStatPointsChanged();
        }
    }
    public static void WaveChanged(int waves)
    {
        if (OnWaveChanged != null)
        {
            OnWaveChanged(waves);
        }
    }
}
