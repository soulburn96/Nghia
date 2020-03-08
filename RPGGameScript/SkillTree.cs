using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillTree : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerStat playerStat;
    public Text hpText;
    public Text damageText;
    public Text speedText;
        
    void Start()
    {
        playerStat = GameObject.Find("Player").GetComponent<PlayerStat>();
    }

    // Update is called once per frame
    void Update()
    {
        hpText.text = "Hp: " + playerStat.hp;
        damageText.text = "Damage: " + playerStat.damage;
        speedText.text = "Speed: " + playerStat.speed;
    }

}
