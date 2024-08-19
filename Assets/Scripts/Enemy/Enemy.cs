using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy
{
    public EnemyBase Base { get; private set; } // Base stats of the enemy
    public int Level { get; private set; } // Level of the enemy
    public int HP { get; private set; } // Current HP of the enemy
    public Inventory Inventory { get; private set; } // Inventory of the enemy
    public String Name { get; private set; }

    private int currentAttack;
    private int currentDefense;
    private int currentMaxHP;
    private int currentSpeed;

    private const int MaxSlots = 4; // Cap slots at 4

    public Enemy(EnemyBase eBase, int eLevel)
    {
        if (eBase == null)
        {
            throw new System.ArgumentNullException(nameof(eBase), "EnemyBase cannot be null.");
        }

        Name = eBase.Name;
        Base = eBase;
        InitializeStats(eLevel);
        HP = currentMaxHP; // Set the current HP of the enemy to the maximum HP

        try
        {
            InitializeInventory(eBase); // Initialize and populate the inventory
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error initializing enemy inventory: {ex.Message}");
        }
    }

    private void InitializeInventory(EnemyBase eBase)
    {
        Inventory = new Inventory();

        // Add fixed items if they exist
        if (eBase.FixedItems != null && eBase.FixedItems.Count > 0)
        {
            Inventory.InitializeInventory(eBase.FixedItems);
        }
        else
        {
            // Initialize with empty slots or a fallback item
            Inventory.InitializeInventory(new List<ItemBase>());
        }
    }

    private List<ItemBase> GetRandomizedItems(EnemyBase eBase)
    {
        List<ItemBase> items = new List<ItemBase>();
        List<ItemBase> preferredItems = GetPreferredItems(eBase);

        // Ensure at least one attack item
        AddRandomItemFromCategory(eBase.AttackItems, items);

        // Fill remaining slots with preferred items or any available items
        while (items.Count < MaxSlots)
        {
            if (preferredItems.Count > 0 && Random.value < 0.75f) // 75% chance to get a preferred item
            {
                AddRandomItemFromCategory(preferredItems, items);
            }
            else
            {
                AddRandomItemFromCategory(GetAllItemCategories(eBase), items);
            }

            if (items.Count >= 4) break; // Ensure no more than 4 items
        }

        return items;
    }

    private List<ItemBase> GetPreferredItems(EnemyBase eBase)
    {
        List<ItemBase> preferredItems = new List<ItemBase>();

        // Add items based on enemy type preferences
        switch (eBase.Type)
        {
            case EnemyType.Demon:
                preferredItems.AddRange(eBase.AttackItems);
                break;
            case EnemyType.Gremlin:
                preferredItems.AddRange(eBase.RecoveryItems);
                break;
            case EnemyType.Possessed:
                preferredItems.AddRange(eBase.DefensiveItems);
                break;
            default: // Yokai or others: no preference, add all types
                preferredItems.AddRange(eBase.AttackItems);
                preferredItems.AddRange(eBase.DefensiveItems);
                preferredItems.AddRange(eBase.RecoveryItems);
                break;
        }

        return preferredItems;
    }

    private List<ItemBase> GetAllItemCategories(EnemyBase eBase)
    {
        List<ItemBase> allItems = new List<ItemBase>();
        allItems.AddRange(eBase.AttackItems);
        allItems.AddRange(eBase.DefensiveItems);
        allItems.AddRange(eBase.RecoveryItems);
        return allItems;
    }

    private void AddRandomItemFromCategory(List<ItemBase> items, List<ItemBase> targetList)
    {
        if (items != null && items.Count > 0)
        {
            var randomItem = items[Random.Range(0, items.Count)];
            targetList.Add(randomItem);
        }
    }

    public int Attack => currentAttack;
    public int Defense => currentDefense;
    public int MaxHP => currentMaxHP;
    public int Speed => currentSpeed;

    private void InitializeStats(int eLevel) // Initialize the stats of the enemy, by 5 points per level
    {
        Level = eLevel;
        currentAttack = Base.BaseAttack + (Level - 1) * 5;
        currentDefense = Base.BaseDefense + (Level - 1) * 5;
        currentMaxHP = Base.MaxHP + (Level - 1) * 5;
        currentSpeed = Base.Speed + (Level - 1) * 5;
    }

    public bool TakeDamage(int damage)  // Take damage and return true if enemy is defeated
    {
        HP -= damage;

        if (HP <= 0)
        {
            HP = 0;
            return true;
        }

        return false;
    }

    public void Heal(int amount) // Heal the enemy by the specified amount
    {
        HP += amount;
        if (HP > currentMaxHP) HP = currentMaxHP;
    }

    public void SetLevel(int newLevel) // Set the enemy's level and update stats
    {
        if (newLevel < 1)
        {
            Debug.LogError("Level must be at least 1.");
            return;
        }

        Level = newLevel; // Set the new level
        InitializeStats(Level); // Update stats based on the new level
        HP = currentMaxHP; // Adjust HP when level changes
    }

    public void LevelUp() // Level up the enemy, increasing stats
    {
        SetLevel(Level + 1);
    }
}
