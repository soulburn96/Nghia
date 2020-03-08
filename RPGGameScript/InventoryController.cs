using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    WeaponController playerWeaponController;
    public Item sword;
    // Start is called before the first frame update
    void Start()
    {
        //creation of the weapons
        playerWeaponController = GetComponent<WeaponController>();
        List<BaseStat> swordStats = new List<BaseStat>();
        swordStats.Add(new BaseStat(10, "Power", "Your Damage"));
        sword = new Item(swordStats, "sword","","");
        playerWeaponController.EquipWeapon(sword);
    }
}
