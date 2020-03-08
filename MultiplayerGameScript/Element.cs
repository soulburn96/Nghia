using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Element
{
    // Start is called before the first frame update
    string elementName ;
    int maxHp ;
    float speed ;
    
    int spellCooldown ;
    int hpRegen ;
    string strongerElement;
    string weakerElement;
    public void SetFire()
    {
        this.elementName = "Fire";
        this.maxHp = 150;
        this.speed = 0.25f;
        
        this.spellCooldown = 4;
        this.hpRegen = 1;
        this.strongerElement = "Water";
        this.weakerElement = "Earth";
    }
    public void SetWater()
    {
        this.elementName = "Water";
        this.maxHp = 150;
        this.speed = 0.33f;
        
        this.spellCooldown = 4;
        this.hpRegen = 1;
        this.strongerElement = "Lightning";
        this.weakerElement = "Fire";
    }
    public void SetLightning()
    {
        this.elementName = "Lightning";
        this.maxHp = 100;
        this.speed = 0.2f;
        
        this.spellCooldown = 3;
        this.hpRegen = 2;
        this.strongerElement = "Earth";
        this.weakerElement = "Water";
    }
    public void SetEarth()
    {
        this.elementName = "Earth";
        this.maxHp = 200;
        this.speed = 0.5f;
        
        this.spellCooldown = 5;
        this.hpRegen = 0;
        this.strongerElement = "Fire";
        this.weakerElement = "Lightning";
    }
    public string GetName()
    {
        return this.elementName;
    }
    public int GetHp()
    {
        return this.maxHp;
    }
    public float GetSpeed()
    {
        return this.speed;
    }
    
    public int GetSpellCooldown()
    {
        return this.spellCooldown;
    }
    public int GetHpRegen()
    {
        return this.hpRegen;
    }
    public string GetStrongerElement()
    {
        return this.strongerElement;
    }
    public string GetWeakerElement()
    {
        return this.weakerElement;
    }
}
