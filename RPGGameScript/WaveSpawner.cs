using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState
    {
        SPAWNING, WAITING, COUNTING, STOPPING
    };
    public static int EnemiesAlive = 0;
    public Wave[] waves;
    
    public Transform spawnPoint;
    public bool cleared = false;
    private float countdown = 2f;
    private float waveTimer = 5;

    public Text waveText;
    public Text waveCountdownText;
    public SpawnState state;
    private int waveIndex = 0;
    void Start()
    {
        state = SpawnState.COUNTING;
        waveIndex = 0;
        countdown = 2f;
        waveTimer = 5f;
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        waveIndex = 0;
        state = SpawnState.COUNTING;
        countdown = 2f;
        waveTimer = 5f;
    }
    public void Restart()
    {
        waveIndex = 0;
        state = SpawnState.COUNTING;
        countdown = 2f;
        waveTimer = 5f;
    }
    void Update()
    {
        if (state == SpawnState.STOPPING)
        {
            if (!EnemyIsAlive())
            {
                if (waveIndex == waves.Length - 1)
                {
                    UIManager.Instance.Victory();
                }
                else
                {   
                   
                    waveText.text = "Wave " + (1 + waveIndex) + " begins in ";
                    waveCountdownText.text = Mathf.RoundToInt(waveTimer).ToString();                 
                    UIManager.Instance.WaitForWave(true);
                    if (waveTimer <= 0f)
                    {
                        UIManager.Instance.WaitForWave(false);
                        NextWave();
                        waveTimer = 5f;
                    }
                    else
                    {
                        waveTimer -= Time.deltaTime;
                        waveTimer = Mathf.Clamp(waveTimer, 0f, Mathf.Infinity);
                    }
                }      
            }
        }
        if (countdown <= 0f)
        {
            if(state != SpawnState.STOPPING)
            {
                if (state != SpawnState.SPAWNING)
                {                   
                    StartCoroutine(SpawnWave());
                    return;
                }
            }
        }
        else
        {
            countdown -= Time.deltaTime;

            countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);
        }
        // Debug.Log(string.Format("{0:00.00}", countdown));
    }
    
    bool EnemyIsAlive()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        UIManager.WaveChanged(enemies.Length);
        if (GameObject.FindGameObjectWithTag("Enemy") == null)
        {
            return false;
        }
        return true;
    }
    public void NextWave()
    {
        UIManager.Instance.LevelUp();
        waveIndex++;
        state = SpawnState.COUNTING;  
    }
    IEnumerator SpawnWave()
    {
        Wave wave = waves[waveIndex];
        Debug.Log("Wave is :"+ waveIndex);
        EnemiesAlive = wave.count;
        state = SpawnState.SPAWNING;
        for (int i = 0; i < wave.count; i++)
        {
            UIManager.WaveChanged(EnemiesAlive);
            SpawnEnemy(wave.enemy);
            yield return new WaitForSeconds(1f);
        }      
        state = SpawnState.STOPPING;
    }

    void SpawnEnemy(GameObject enemy)
    {
        if (spawnPoint != null)
        {
            
        }
        else
        {
            Vector3 spawnPosition = new Vector3(Random.Range(300, 460), 8, Random.Range(530, 720));
            Instantiate(enemy, spawnPosition, enemy.transform.rotation);
        }
    }

}