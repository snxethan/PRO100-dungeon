using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public EnemyBase Base { get; set; } // Base stats of the enemy
    public int Level { get; set; } // Level of the enemy
    public int HP { get; set; } // Current HP of the enemy

    private Inventory inventory; // Inventory of the enemy
    private GameObject enemyObject; // Enemy game object

    public Enemy(EnemyBase eBase, int eLevel) // Constructor for the enemy
    {
        Base = eBase; // Set the base stats of the enemy
        Level = eLevel; // Set the level of the enemy
        HP = MaxHP; // Set the current HP of the enemy to the maximum HP
        enemyObject = new GameObject(Base.Name); // Create a new game object for the enemy
        inventory = enemyObject.AddComponent<Inventory>(); // Add an inventory component to the enemy game object

        PopulateInventory(); // Populate the inventory of the enemy
    }

    private void PopulateInventory() // Method to populate the inventory of the enemy
    {
        List<ItemBase> selectedItems = new List<ItemBase>(); // List to store selected items

        // Ensure at least one item from each category 
        selectedItems.Add(GetRandomItem(Base.AttackItems)); // Add a random attack item
        selectedItems.Add(GetRandomItem(Base.DefensiveItems)); // Add a random defensive item
        selectedItems.Add(GetRandomItem(Base.RecoveryItems)); // Add a random recovery item

        // Determine remaining items to add
        int remainingItems = Base.RandomItemCount - selectedItems.Count; // Calculate the remaining items to add

        while (remainingItems > 0) // Loop to add remaining items
        {
            var item = GetRandomItemBasedOnType(); // Get a random item based on the enemy type
            selectedItems.Add(item); // Add the item to the selected items list
            remainingItems--; // Decrement the remaining items count
        }

        // Add all selected items to inventory
        foreach (var item in selectedItems) // Loop through selected items
        {
            inventory.AddItem(item); // Add the item to the inventory
        }
    }

    private ItemBase GetRandomItemBasedOnType()
    {
        List<ItemBase> weightedItems = new List<ItemBase>(); // List to store weighted items

        switch (Base.Type) // Switch statement based on enemy type
        {
            case EnemyType.Yokai: 
                AddItemsWithWeight(weightedItems, Base.AttackItems, 70); // Add attack items with a weight of 70
                AddItemsWithWeight(weightedItems, Base.DefensiveItems, 15); // Add defensive items with a weight of 15
                AddItemsWithWeight(weightedItems, Base.RecoveryItems, 15); // Add recovery items with a weight of 15
                break;
            case EnemyType.Possessed: 
                AddItemsWithWeight(weightedItems, Base.DefensiveItems, 70); // Add defensive items with a weight of 70
                AddItemsWithWeight(weightedItems, Base.AttackItems, 15); // Add attack items with a weight of 15
                AddItemsWithWeight(weightedItems, Base.RecoveryItems, 15); // Add recovery items with a weight of 15
                break;
            default:
                AddItemsWithWeight(weightedItems, Base.RecoveryItems, 70); // Add recovery items with a weight of 70
                AddItemsWithWeight(weightedItems, Base.AttackItems, 15); // Add attack items with a weight of 15
                AddItemsWithWeight(weightedItems, Base.DefensiveItems, 15); // Add defensive items with a weight of 15
                break;
        }

        return weightedItems[UnityEngine.Random.Range(0, weightedItems.Count)]; // Return a random item from the weighted items list
    }

    private void AddItemsWithWeight(List<ItemBase> weightedItems, List<ItemBase> items, int weight)
    {
        foreach (var item in items) // Loop through items
        {
            for (int i = 0; i < weight; i++) // Loop to add items with weight
            {
                weightedItems.Add(item); // Add the item to the weighted items list
            }
        }
    }

    private ItemBase GetRandomItem(List<ItemBase> items) // Method to get a random item from a list
    {
        if (items == null || items.Count == 0) return null; // Return null if the list is empty
        return items[UnityEngine.Random.Range(0, items.Count)]; // Return a random item from the list
    }

    public int Attack => Mathf.FloorToInt((Base.BaseAttack * Level) / 100f + 5); // Calculate the attack value
    public int Defense => Mathf.FloorToInt((Base.BaseDefense * Level) / 100f + 5); // Calculate the defense value
    public int MaxHP => Mathf.FloorToInt((Base.MaxHP * Level) / 100f + 10); // Calculate the maximum HP value
    public int Speed => Mathf.FloorToInt((Base.Speed * Level) / 100f + 5); // Calculate the speed value
    public Inventory Inventory => inventory; // Getter for the inventory
}
