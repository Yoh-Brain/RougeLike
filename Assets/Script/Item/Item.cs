using UnityEngine;

public class Item 
{

    public string itemName;
    public int healAmount;

    public Item(string name, int heal)
    {
        itemName = name;
        healAmount = heal;
    }
}