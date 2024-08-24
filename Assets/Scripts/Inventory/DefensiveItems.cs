using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Items/Create new DEF Item")]
public class DefensiveItems : ItemBase
{
    [Header("Damage Reduction")]
    [SerializeField] int dmgReduction;
    [SerializeField] bool fullReduction;
    public DefensiveItems()
    {
        itemType = ItemType.DefenseItem;
    }
    public override string GetItemTypeStr(int level)
    {
        int scaledDefenseAmount = dmgReduction + (level * 2);
        return $"DEF (+{scaledDefenseAmount} DEF)";
    }

    public override int GetItemModifier(int level)
    {
        if (fullReduction)
        {
            Debug.Log("Item returned full damage reduction");
            return -1;
        }
        int scaledDmgReduction = dmgReduction + (level * 2); // Scale damage reduction with level
        Debug.Log($"Item returned {scaledDmgReduction}");
        return scaledDmgReduction;
    }
}