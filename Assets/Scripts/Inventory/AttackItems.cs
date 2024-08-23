using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Items/Create new Attack item")]
public class AttackItems : ItemBase
{
    [Header("Damage")]
    [SerializeField] int dmgAmount;
    [SerializeField] private bool infiniteDmg;
    public AttackItems()
    {
        itemType = ItemType.AttackItem;
    }
    public override string GetItemTypeStr(int level)
    {
        if (infiniteDmg)
        {
            return "ATK (\u221e HP)";
        }
        int scaledDmgAmount = dmgAmount + (level * 2);
        return $"ATK (-{scaledDmgAmount} HP)";
    }

    public override int GetItemModifier(int level)
    {
        int scaledDmgAmount = infiniteDmg ? int.MaxValue : dmgAmount + (level * 2); // Scale damage amount with level
        Debug.Log($"Item returned {scaledDmgAmount}");
        return scaledDmgAmount;
    }
}