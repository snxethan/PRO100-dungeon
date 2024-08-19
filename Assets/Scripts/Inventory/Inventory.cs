using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<ItemBase> fixedItems = new List<ItemBase>(); // Fixed items that cannot be replaced

    private List<ItemSlot> slots = new List<ItemSlot>(); // The complete inventory with both fixed and dynamic items
    private const int MaxSlots = 4; // Total number of inventory slots

    public List<ItemSlot> Slots => slots; // Public getter for all inventory slots

    private void Awake()
    {
        InitializeInventory(); // Initialize the inventory on start
    }

    public void InitializeInventory(List<ItemBase> fixedItemsList = null)
    {
        // Handle cases where the inventory might be initialized multiple times
        slots.Clear();

        // If a new fixedItemsList is provided, use it
        if (fixedItemsList != null)
        {
            fixedItems = fixedItemsList;
        }

        // Add fixed items to slots, respecting the MaxSlots limit
        for (int i = 0; i < MaxSlots; i++)
        {
            if (i < fixedItems.Count && fixedItems[i] != null)
            {
                slots.Add(new ItemSlot(fixedItems[i], 1));
            }
            else
            {
                slots.Add(new ItemSlot(null, 0)); // Add an empty slot for dynamic items
            }
        }

        Debug.Log($"Inventory initialized with {slots.Count} slots. Fixed Items: {fixedItems.Count}");
    }

    public void AddFixedItem(ItemBase item)
    {
        if (fixedItems.Count < MaxSlots)
        {
            fixedItems.Add(item);

            // Replace the first empty slot with the new fixed item
            for (int i = 0; i < MaxSlots; i++)
            {
                if (slots[i].Item == null)
                {
                    slots[i] = new ItemSlot(item, 1);
                    break;
                }
            }
        }
        else
        {
            Debug.LogWarning("Cannot add more fixed items. Inventory is full.");
        }
    }

    public void ReplaceDynamicItem(int slotIndex, ItemBase newItem)
    {
        if (slotIndex >= fixedItems.Count && slotIndex < MaxSlots)
        {
            slots[slotIndex] = new ItemSlot(newItem, 1);
        }
        else
        {
            Debug.LogError("Invalid slot index or trying to replace a fixed item.");
        }
    }

    public List<ItemSlot> GetAllItems()
    {
        return new List<ItemSlot>(slots); // Return a copy of all items in the inventory
    }

    public List<ItemBase> GetItemNames()
    {
        List<ItemBase> itemNames = new List<ItemBase>();
        foreach (var slot in slots)
        {
            if (slot.Item != null)
            {
                itemNames.Add(slot.Item);
            }
        }
        return itemNames;
    }

    public void ClearInventory()
    {
        slots.Clear();
        fixedItems.Clear();
    }
}
