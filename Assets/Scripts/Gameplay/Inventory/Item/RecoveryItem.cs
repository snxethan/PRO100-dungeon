using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new HEAL Item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;
    public RecoveryItem()
    {
        itemType = ItemType.RecoveryItem;
    }

    public override string GetItemTypeStr(int level) // Get the item type as a string
    {
        if(level == -1) // If the level is -1, set as infinite hp
        {
            restoreMaxHP = true;
        }
        if (restoreMaxHP) // If the item restores max HP
        {
            return $"HEAL (\u221e HP)";
        }
        int scaledHpAmount = hpAmount + (level * 2);
        return $"HEAL (+{scaledHpAmount} HP)";
    }

    public override int GetItemModifier(int level) // Get the item modifier
    {
        if (level == -1) // If the level is -1, set as infinite hp
        {
            restoreMaxHP = true;
        }
        if (restoreMaxHP) 
        {
            Debug.Log("Item returned max HP");
            return -1; 
        }
        int scaledHpAmount = hpAmount + (level * 2); // Scale HP amount with level
        Debug.Log($"Item returned {scaledHpAmount}");
        return scaledHpAmount;
    }
}