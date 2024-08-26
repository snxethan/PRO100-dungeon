using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(menuName = "Items/Create new DEF Item")]
public class DefensiveItems : ItemBase
{
    [Header("Damage Reduction")]
    [SerializeField] int Defense; 
    [SerializeField] bool infiniteDefense;
    public DefensiveItems()
    {
        itemType = ItemType.DefenseItem;
    }
    public override string GetItemTypeStr(int level) // Return the item type as a string
    {
        if(level == -1) // If the level is -1, return the item type as infinite
        {
            return "DEF (\u221e DEF)";
        }
        int scaledDefenseAmount = Defense + (level * 2);
        return $"DEF (+{scaledDefenseAmount} DEF)";
    }

    public override int GetItemModifier(int level)
    {
        if (level == -1) // If the level is -1, return -1 to indicate infinite defense
        {
            infiniteDefense = true;
        }
        if (infiniteDefense)
        {
            Debug.Log("Item returned full damage reduction");
            return -1;
        }
        int scaledDmgReduction = Defense + (level * 2); // Scale damage reduction with level
        Debug.Log($"Item returned {scaledDmgReduction}");
        return scaledDmgReduction;
    }
}