using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new recovery item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;
    public RecoveryItem()
    {
        itemType = ItemType.RecoveryItem;
    }

    public override string GetItemTypeStr(int level)
    {
        if (restoreMaxHP)
        {
            return $"HEAL (\u221e HP)";
        }
        int scaledHpAmount = hpAmount + (level * 2);
        return $"HEAL (+{scaledHpAmount} HP)";
    }

    public override int GetItemModifier(int level)
    {
        int scaledHpAmount = hpAmount + (level * 2); // Scale HP amount with level
        Debug.Log($"Item returned {scaledHpAmount}");
        return scaledHpAmount;
    }
}