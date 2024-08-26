using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<ItemBase> items = new List<ItemBase>(); // Initialize the list
    public const int MaxSlots = 4;

    #region Initialization & Setup
    private void Awake()
    {
        InitializeInventory("null");
    }

    /// <summary>
    /// This method initializes the inventory with the given items.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="initialItems"></param>
    public void InitializeInventory( String name, List<ItemBase> initialItems = null)
    {
        items.Clear(); // Clear the list

        if (initialItems != null) // If the list is not null
        {
            foreach (var item in initialItems) // For each item in the list
            {
                if (item != null)
                {
                    items.Add(item);
                }
            }
        }

        if (items.Count > MaxSlots) // If the list is greater than the max slots
        {
            items = items.GetRange(0, MaxSlots); // Get the range of the list
        }

        for (int i = items.Count; i < MaxSlots; i++) // For each item in the list
        {
            items.Add(null); // Add null to the list
        }

        Debug.Log($"{name} 'sInventory initialized with {items.Count} items: {string.Join(", ", items.Select(item => item?.name ?? "Empty"))}");
    }

    #endregion
    
    #region Adding, Removing, Replacing, Getting, Clearing, Checking
    public void AddItem(ItemBase item) // Add item to the inventory
    {
        if(item == null)
        {
            Debug.LogWarning("Cannot add null item to inventory."); 
            return;
        }
        if (!IsFull()) // If the inventory is not full
        {
            Debug.Log($"Added {item} to inventory.");
            items.Add(item);
        }
        else
        {
            Debug.LogWarning("Inventory is full. Cannot add more items.");
        }
    }

    public void RemoveItem(int slotIndex) // Remove item from the inventory
    {
        if (slotIndex >= 0 && slotIndex < items.Count)
        {
            items[slotIndex] = null;
        }
    }

    public void ReplaceItem(int slotIndex, ItemBase newItem) // Replace item in the inventory
    {
        if (slotIndex >= 0 && slotIndex < items.Count)
        {
            items[slotIndex] = newItem;
        }
    }

    public List<ItemBase> GetItems() // Get the items in the inventory
    {
        return new List<ItemBase>(items);
    }

    public void ClearInventory() // Clear the inventory
    {
        items.Clear();
        Debug.Log("Inventory cleared.");
    }
    
    #region Check Logic
    public bool IsFull() // Check if the inventory is full
    {
        return items.Count >= MaxSlots && items.All(item => item != null);
    }

    public bool IsEmpty() // Check if the inventory is empty
    {
        return items.All(item => item == null);
    }
    #endregion
    
    #endregion
}