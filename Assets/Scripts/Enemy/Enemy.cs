using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy
{
    public EnemyBase Base { get; private set; } // EnemyBase is a ScriptableObject
    public int Level { get; private set; } // Level of the enemy
    public int HP { get; private set; } // Current HP of the enemy
    public Inventory Inventory { get; private set; } // Inventory of the enemy
    public string Name { get; private set; } // Name of the enemy

    private int currentAttack; // Current attack of the enemy
    private int currentDefense; // Current defense of the enemy
    private int currentMaxHP; // Current max HP of the enemy
    private int currentSpeed; // Current speed of the enemy
    private float experience; // Experience of the enemy
    public bool isAlive => HP > 0; // Check if the enemy is alive

    public Enemy(EnemyBase eBase, int eLevel)
    {
        if (eBase == null) // Check if the EnemyBase is null
        {
            throw new ArgumentNullException(nameof(eBase), "EnemyBase cannot be null.");
        }

        Base = eBase; // Set the base of the enemy
        Name = eBase.Name; // Set the name of the enemy
        SetLevel(eLevel); // Set the level of the enemy
        HP = currentMaxHP; // Set the HP of the enemy to max HP

        InitializeInventory(eBase); // Initialize the inventory of the enemy
    }

    private void InitializeInventory(EnemyBase eBase)
    {
        GameObject inventoryObject = new GameObject("Inventory");
        Inventory = inventoryObject.AddComponent<Inventory>();
        Inventory.InitializeInventory(eBase.FixedItems ?? new List<ItemBase>());

        List<ItemBase> availableItems = new List<ItemBase>();
        availableItems.AddRange(Resources.LoadAll<ItemBase>("Items"));

        List<ItemBase> attackItems = availableItems.Where(i => i.ItemType == ItemType.AttackItem).ToList();
        List<ItemBase> defenseItems = availableItems.Where(i => i.ItemType == ItemType.DefenseItem).ToList();
        List<ItemBase> recoveryItems = availableItems.Where(i => i.ItemType == ItemType.RecoveryItem).ToList();

        if (attackItems.Count > 0)
        {
            Inventory.AddFixedItem(attackItems[Random.Range(0, attackItems.Count)]);
        }

        if (defenseItems.Count > 0)
        {
            Inventory.AddFixedItem(defenseItems[Random.Range(0, defenseItems.Count)]);
        }

        if (recoveryItems.Count > 0)
        {
            Inventory.AddFixedItem(recoveryItems[Random.Range(0, recoveryItems.Count)]);
        }

        if (availableItems.Count > 0)
        {
            Inventory.AddFixedItem(availableItems[Random.Range(0, availableItems.Count)]);
        }

        Inventory.PopulateDynamicSlots(availableItems, eBase.AttackItems);
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
        if(!isAlive) return false;
        Debug.Log($"Enemy is taking damage from {player.Name} using {item.Name}");
        float modifiers = Random.Range(0.85f, 1.15f); // Random factor between 0.85 and 1.15
        int itemDamage = item.GetItemModifier(player.Level);
        int baseDamage = player.Attack;
        int totalDamage = Mathf.FloorToInt((itemDamage + baseDamage) * modifiers);

        return SetHP(HP - totalDamage); // Subtract damage from current HP
    }

    public void Heal(int amount)
    {
        if (!isAlive) return;
        float modifiers = Random.Range(0.85f, 1.15f); // Random factor between 0.85 and 1.15
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
            return isAlive;
        }

        return isAlive;
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
        float modifiers = Random.Range(0.85f, 1.15f); // Random factor between 0.85 and 1.15
        int totalDefense = Mathf.FloorToInt(defense * modifiers);
        currentDefense += totalDefense;
        return currentDefense;
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
            Inventory.CheckAndConvertFixedSlots();
        }
    }
    public bool GainExperience(float exp) // Gain experience with float values
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