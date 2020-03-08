using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class Item
{

    //finding the names of the items
    public List<BaseStat> Stats { get; set; }
    public string ObjectSlug { get; set; }
    public string ItemName { get; set; }

    public string Description { get; set; }

    [JsonConstructor]
    public Item(List<BaseStat> _Stats, string _ObjectSlug, string _ItemName, string _Description)
    {
        this.Stats = _Stats;
        this.ObjectSlug = _ObjectSlug;
     
        this.ItemName = _ItemName;
        this.Description = _Description;
    }
}
