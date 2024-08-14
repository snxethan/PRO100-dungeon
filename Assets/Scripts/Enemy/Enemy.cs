using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public EnemyBase Base { get; set; }
    public int Level { get; set; }
    public int HP { get; set; }

    private Inventory inventory;
    private GameObject enemyObject;

    public Enemy(EnemyBase eBase, int eLevel)
    {
        Base = eBase;
        Level = eLevel;
        HP = MaxHP;
        enemyObject = new GameObject(Base.Name); // Create a new GameObject with the name of the Enemy
        inventory = enemyObject.AddComponent<Inventory>(); // Add Inventory component to this GameObject

        PopulateInventory();
    }

    private void PopulateInventory()
    {
        int itemCount = 0;

        while (itemCount < Base.RandomItemCount)
        {
            var item = GetRandomItemBasedOnType();
            // Implement your logic to add the item to the inventory using your existing system
            itemCount++;
        }
    }

    private ItemBase GetRandomItemBasedOnType()
    {
        List<ItemBase> allItems = new List<ItemBase>();

        //edit this to add more cases for other types
        switch (Base.Type)
        {
            case EnemyType.Yokai:
                allItems.AddRange(Base.AttackItems); // Yokai prefer attack items
                break;
            case EnemyType.Possessed:
                allItems.AddRange(Base.DefensiveItems); // Possessed prefer defensive items
                break;
            // Add other cases for other types as needed
            default:
                allItems.AddRange(Base.RecoveryItems); // Default or no type prefers recovery items
                break;
        }

        // Pick a random item from the selected list
        return allItems[UnityEngine.Random.Range(0, allItems.Count)];
    }


    public int Attack
    {
        get { return Mathf.FloorToInt((Base.BaseAttack * Level) / 100f + 5); }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((Base.BaseDefense * Level) / 100f + 5); }

    }

    public int MaxHP
    {
        get { return Mathf.FloorToInt((Base.MaxHP * Level) / 100f + 10); }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f + 5); }

    }

    public Inventory Inventory
    {
        get { return inventory; }
    }

}
