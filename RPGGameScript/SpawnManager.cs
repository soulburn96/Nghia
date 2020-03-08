  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] monsters; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnMonster(1,20);
        }
    }
    void SpawnMonster(int level,int number)
    {
        for (int i = 0; i < number; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(300, 460), 1, Random.Range(530, 720));
            Instantiate(monsters[level - 1], spawnPosition, monsters[level - 1].transform.rotation);
        }
    }
}
