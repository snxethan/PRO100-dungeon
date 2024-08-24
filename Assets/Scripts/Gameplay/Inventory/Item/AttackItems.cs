using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Items/Create new ATTACK Item")]
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
        if(level == -1)
        {
            infiniteDmg = true;
        }
        if (infiniteDmg)
        {
            return "ATK (\u221e HP)";
        }
        int scaledDmgAmount = dmgAmount + (level * 2);
        return $"ATK (-{scaledDmgAmount} HP)";
    }

    public override int GetItemModifier(int level)
    {
        if (level == -1)
        {
            infiniteDmg = true;
        }
        if (infiniteDmg)
        {
            Debug.Log("Item returned infinite damage");
            return -1;
        }
        int scaledDmgAmount = infiniteDmg ? int.MaxValue : dmgAmount + (level * 2); // Scale damage amount with level
        Debug.Log($"Item returned {scaledDmgAmount}");
        return scaledDmgAmount;
    }
}