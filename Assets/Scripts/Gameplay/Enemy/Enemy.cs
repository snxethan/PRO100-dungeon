using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy
{
    
    #region Fields & Properties
    
    private int currentAttack;
    private int currentDefense;
    private int currentMaxHP;
    private int currentSpeed;
    private float experience;
    
    
    //getters for the enemy stats
    public EnemyBase Base { get; private set; } //the base enemy
    public int Level { get; private set; } 
    public int HP { get; private set; }
    public Inventory Inventory { get; private set; } //the inventory of the enemy
    public int Attack => currentAttack;
    public int Defense { get => currentDefense; private set => currentDefense = value; }
    public int MaxHP => currentMaxHP;
    public int Speed => currentSpeed;
    public string Name { get; private set; }
    #endregion

    #region Constructor & Initialization
    /// <summary>
    /// The enemy constructor,
    /// this is a 'enemy', that will be used in the battle system to fight against the player
    /// </summary>
    /// <param name="eBase">The base enemy, which is defined in resources/enemy</param>
    /// <param name="eLevel">the level of the enemy, usually grabbed from the player</param>
    /// <exception cref="ArgumentNullException">an Exception to prevent a null enemy</exception>
    public Enemy(EnemyBase eBase, int eLevel)
    {
        if (eBase == null)
        {
            throw new ArgumentNullException(nameof(eBase), "EnemyBase cannot be null");
        }

        //initialize the enemy stats
        Base = eBase;
        Name = eBase.Name;
        SetLevel(eLevel);
        HP = currentMaxHP;

        InitializeInventory(eBase); //initialize the inventory of the enemy
    }

    /// <summary>
    /// Initialize the inventory of the enemy
    /// </summary>
    /// <param name="eBase">The Base Enemy</param>
    private void InitializeInventory(EnemyBase eBase)
    {
        // Create an inventory object and attach it to the enemy
        GameObject inventoryObject = new GameObject("Inventory");
        Inventory = inventoryObject.AddComponent<Inventory>();

        List<ItemBase> initialItems = new List<ItemBase>();

        // Ensure each enemy gets one of each type of item
        ItemBase healingItem = GetRandomItemOfType(ItemType.RecoveryItem);
        ItemBase defensiveItem = GetRandomItemOfType(ItemType.DefenseItem);
        ItemBase attackItem = GetRandomItemOfType(ItemType.AttackItem);

        initialItems.Add(healingItem);
        initialItems.Add(defensiveItem);
        initialItems.Add(attackItem);

        // Determine the fourth item based on the enemy's type
        ItemType preferredItemType = GetPreferredItemType(eBase.Type);
        ItemBase preferredItem = GetRandomItemOfType(preferredItemType);
        initialItems.Add(preferredItem);

        Inventory.InitializeInventory(Name, initialItems);
    }
    #endregion
    
    #region Item Logic
    /// <summary>
    /// Returns the preferred item type for the enemy based on its type
    /// </summary>
    /// <param name="enemyType">The type of enemy</param>
    /// <returns></returns>
    private ItemType GetPreferredItemType(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Gremlin:
                return ItemType.RecoveryItem;
            case EnemyType.Demon:
                return ItemType.AttackItem;
            case EnemyType.Possessed:
                return ItemType.DefenseItem;
            default:
                // randomize the preferred item type if the enemy type is null OR Yokai:
                int randomIndex = Random.Range(0, 3);
                return (ItemType)randomIndex;
        }
    }
    private ItemBase GetRandomItemOfType(ItemType itemType)
    {
        // Load all items of the specified type
        var itemsOfType = Resources.LoadAll<ItemBase>("Items").Where(item => item.ItemType == itemType).ToList();
        return itemsOfType[Random.Range(0, itemsOfType.Count)];
    }
    public List<ItemBase> GetItems()
    {
        return Inventory.GetItems(); //return the items of the enemy
    }
    
    #endregion

    #region Battle Logic

    #region Level Logic
    
    /// <summary>
    /// increases the level of the enemy,
    /// as well as increase the stats by level.
    /// </summary>
    /// <param name="eLevel">The Level you want to set</param>
    private void SetLevel(int eLevel)
    {
        // Ensure the level is at least 1
        Level = Mathf.Max(1, eLevel);
        currentAttack = Base.BaseAttack + (Level - 1) * 5;
        currentDefense = Base.BaseDefense + (Level - 1) * 5;
        currentMaxHP = Base.MaxHP + (Level - 1) * 5;
        currentSpeed = Base.Speed + (Level - 1) * 5;
    }
    
    /// <summary>
    /// levels up the enemy and heals the enemy to full HP
    /// </summary>
    public void LevelUp()
    {
        SetLevel(Level + 1);
        HP = currentMaxHP;
    }

    /// <summary>
    /// Gain experience for the enemy and level up if the float is greater than 1
    /// </summary>
    /// <param name="exp"></param>
    /// <returns></returns>
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
    #endregion

    #region Health, Damage & Defense Logic

    /// <summary>
    /// Set the HP of the enemy
    /// </summary>
    /// <param name="newHP">The new HP you want to set</param>
    /// <returns></returns>
    public bool SetHP(int newHP)
    {
        // Ensure HP is within the bounds of 0 and the current max HP
        HP = Mathf.Clamp(newHP, 0, currentMaxHP);
        if (HP <= 0) //if the enemy has no HP left, and dies
        {
            HP = 0;
            Debug.Log($"{Name} has died.");
            return true; 
        }

        return false;
    }
    
    /// <summary>
    /// heal the enemy by a certain amount
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(int amount)
    {
        if(amount == -1) //if the item returns infinite healing
        {
            HP = currentMaxHP; 
            return;
        }
        float modifiers = Random.Range(0.85f, 1.15f);
        int totalHeal = Mathf.FloorToInt(amount * modifiers);
        HP = Mathf.Min(currentMaxHP, HP + totalHeal);
    }

    /// <summary>
    /// Take damage from the player
    /// </summary>
    /// <param name="item">The item the player is using to inflict damage</param>
    /// <param name="player">the player object</param>
    /// <returns></returns>
    public bool TakeDamage(ItemBase item, PlayerController player)
    {
        Debug.Log($"{Name} is taking damage from {player.Name} using {item.Name}");
    
        // Calculate the damage the player will deal to the enemy
        float modifiers = Random.Range(0.85f, 1.15f); //randomize the damage
        int itemDamage = item.GetItemModifier(player.Level); //get the item damage
        if(itemDamage == -1) //if the item returns infinite damage
        {
            Debug.Log($"{item} returned infinite damage");
            return SetHP(0); //kill the enemy
        }
        int baseDamage = player.Attack; //get the player's attack
        int totalDamage = Mathf.FloorToInt((itemDamage + baseDamage) * modifiers); //calculate the total damage

        // Subtract the enemy's defense from the total damage
        totalDamage = Mathf.Max(0, totalDamage - Defense); //subtract the defense from the total damage

        return SetHP(HP - totalDamage); //set the HP of the enemy
    }
    
    /// <summary>
    /// Add defense to the enemy
    /// </summary>
    /// <param name="defense">The int defense to add</param>
    /// <returns></returns>
    public int AddDefense(int defense)
    {
        if (defense == -1) //if the item returns infinite defense
        {
            Debug.Log("Item returned full defense");
            Defense = 9999;
            return Defense;
        }
        float modifiers = Random.Range(0.85f, 1.15f); //randomize the defense
        int totalDefense = Mathf.FloorToInt(defense * modifiers); //calculate the total defense
        Defense += totalDefense; //add the defense to the enemy
        return Defense;
    }
    #endregion

    #endregion












    


    


}