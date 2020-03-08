using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    // Start is called before the first frame update
    public int hp = 100;
    public  int damage = 10;
    public int speed = 12;
    PlayerMovement playerMovement;
    void Start()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHp(int value)
    {
        hp = hp + value;
    }
    public void UpdateDamage(int value)
    {
        damage = damage + value;
    }
    public void UpdateSpeed(int value)
    {
        speed = speed + value;       
    }
}
