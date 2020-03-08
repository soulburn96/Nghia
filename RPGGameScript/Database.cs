using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class Database : MonoBehaviour
{
    public static Database Instance { get; set; }
    private List<Item> Items { get; set; }
    // Start is called before the first frame update
    void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
        
            Instance = this;
            BuildDatabase();
            Debug.Log("Database initialized");    
        }
    }

    private void BuildDatabase() 
    {
        
        Items = JsonConvert.DeserializeObject<List<Item>>(Resources.Load<TextAsset>("Collections/Weapons/Weapons").ToString());
        /*bug.Log(Items[0].ItemName
                    + Items[0].Stats[0].StatName 
                    + " level is" 
                    + Items[0].Stats[0].GetCalculatedStatValue());
        */
    }
    public Item GetItem(string itemSlug)
    {
        //comparing the slug from the item to find the correct one
        foreach(Item item in Items)
        {
            if(item.ObjectSlug == itemSlug)
            {
                return item;
            }
        }
        Debug.LogWarning("Couldn't find itemslug" + itemSlug);
        return null;
    }
}
