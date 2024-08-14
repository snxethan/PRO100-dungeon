using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemSlot> slots = new List<ItemSlot>(); // Initialize the list to avoid null reference issues

    public List<ItemSlot> Slots => slots;

    // Static method to get the player's inventory (if needed)
    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    // Method to populate the inventory with preset and random items
    public void PopulateInventory(List<ItemBase> presetItems, int randomItemCount)
    {
        foreach (var item in presetItems)
        {
            slots.Add(new ItemSlot(item, 1)); // Add preset items to the inventory
        }

        for (int i = 0; i < randomItemCount; i++)
        {
            var randomItem = GetRandomItem(); // Fetch a random item
            if (randomItem != null) // Check to ensure a valid item was returned
            {
                slots.Add(new ItemSlot(randomItem, 1)); // Add random items to the inventory
            }
        }
    }

    // Example method to fetch a random item (you should implement logic to select an item based on your needs)
    private ItemBase GetRandomItem()
    {
        // Load all items from the specified Resources path
        ItemBase[] allItems = Resources.LoadAll<ItemBase>("Items");

        // Pick a random item from the list
        if (allItems.Length > 0)
        {
            return allItems[UnityEngine.Random.Range(0, allItems.Length)];
        }
        return null; // Return null if no items are available
    }
}

// Serializable class to represent an item slot in the inventory
[Serializable]
public class ItemSlot
{
    [SerializeField] private ItemBase item;
    [SerializeField] private int count;

    public ItemSlot(ItemBase item, int count)
    {
        this.item = item;
        this.count = count;
    }

    public ItemBase Item => item;
    public int Count => count;
}
