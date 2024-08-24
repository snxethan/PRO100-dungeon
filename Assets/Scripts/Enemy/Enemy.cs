using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy
{
    public EnemyBase Base { get; private set; }
    public int Level { get; private set; }
    public int HP { get; private set; }
    public Inventory Inventory { get; private set; }
    public string Name { get; private set; }

    private int currentAttack;
    private int currentDefense;
    private int currentMaxHP;
    private int currentSpeed;
    private float experience;

    public Enemy(EnemyBase eBase, int eLevel)
    {
        if (eBase == null)
        {
            throw new ArgumentNullException(nameof(eBase), "EnemyBase cannot be null");
        }

        Base = eBase;
        Name = eBase.Name;
        SetLevel(eLevel);
        HP = currentMaxHP;

        InitializeInventory(eBase);
    }

    private void InitializeInventory(EnemyBase eBase)
    {
        GameObject inventoryObject = new GameObject("Inventory");
        Inventory = inventoryObject.AddComponent<Inventory>();

        List<ItemBase> initialItems = new List<ItemBase>();

        // Ensure each enemy gets one of each item type
        ItemBase healingItem = GetRandomItemOfType(ItemType.RecoveryItem);
        ItemBase defensiveItem = GetRandomItemOfType(ItemType.DefenseItem);
        ItemBase attackItem = GetRandomItemOfType(ItemType.AttackItem);

        initialItems.Add(healingItem);
        initialItems.Add(defensiveItem);
        initialItems.Add(attackItem);

        // Determine the fourth item based on the enemy's preferred item type
        ItemBase preferredItem = GetRandomItemOfType(eBase.PreferredItemType);
        initialItems.Add(preferredItem);

        Inventory.InitializeInventory(initialItems);
    }

    private ItemBase GetRandomItemOfType(ItemType itemType)
    {
        // Assuming you have a method to get all items of a specific type
        var itemsOfType = Resources.LoadAll<ItemBase>("Items").Where(item => item.ItemType == itemType).ToList();
        return itemsOfType[Random.Range(0, itemsOfType.Count)];
    }

    private void SetLevel(int eLevel)
    {
        Level = Mathf.Max(1, eLevel);
        currentAttack = Base.BaseAttack + (Level - 1) * 5;
        currentDefense = Base.BaseDefense + (Level - 1) * 5;
        currentMaxHP = Base.MaxHP + (Level - 1) * 5;
        currentSpeed = Base.Speed + (Level - 1) * 5;
    }

    public int Attack => currentAttack;
    public int Defense { get => currentDefense; private set => currentDefense = value; }
    public int MaxHP => currentMaxHP;
    public int Speed => currentSpeed;

    public bool TakeDamage(ItemBase item, PlayerController player)
    {
        Debug.Log($"Enemy is taking damage from {player.Name} using {item.Name}");
        float modifiers = Random.Range(0.85f, 1.15f);
        int itemDamage = item.GetItemModifier(player.Level);
        if(itemDamage == -1)
        {
            Debug.Log("Item returned infinite damage");
            return SetHP(0);
        }
        int baseDamage = player.Attack;
        int totalDamage = Mathf.FloorToInt((itemDamage + baseDamage) * modifiers);

        return SetHP(HP - totalDamage);
    }

    public void Heal(int amount)
    {
        if(amount == -1)
        {
            HP = currentMaxHP;
            return;
        }
        float modifiers = Random.Range(0.85f, 1.15f);
        int totalHeal = Mathf.FloorToInt(amount * modifiers);
        HP = Mathf.Min(currentMaxHP, HP + totalHeal);
    }

    public bool SetHP(int newHP)
    {
        HP = Mathf.Clamp(newHP, 0, currentMaxHP);
        if (HP <= 0)
        {
            HP = 0;
            Debug.Log("Enemy has died.");
            return true;
        }

        return false;
    }

    public void LevelUp()
    {
        SetLevel(Level + 1);
        HP = currentMaxHP;
    }

    public List<ItemBase> GetItems()
    {
        return Inventory.GetItems();
    }

    public ItemBase GetRandomItem()
    {
        int r = Random.Range(0, GetItems().Count);
        return GetItems()[r];
    }

    public int AddDefense(int defense)
    {
        if (defense == -1)
        {
            Debug.Log("Item returned full defense");
            Defense = 9999;
            return Defense;
        }
        float modifiers = Random.Range(0.85f, 1.15f);
        int totalDefense = Mathf.FloorToInt(defense * modifiers);
        Defense += totalDefense;
        return Defense;
    }
    
    public void UseItem(int slotIndex)
    {
        var item = Inventory.GetItems()[slotIndex];
        if (item != null)
        {
            bool isUsedUp = item.UseItem();
            if (isUsedUp)
            {
                Inventory.RemoveItem(slotIndex);
            }
        }
    }

    public bool GainExperience(float exp)
    {
        experience += exp;
        if (experience >= 1)
        {
            LevelUp();
            experience = 0;
            return true;
        }
        return false;
    }
}