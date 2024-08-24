using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<ItemBase> items = new List<ItemBase>(); // Initialize the list
    public const int MaxSlots = 4;

    private void Awake()
    {
        InitializeInventory();
    }

    public void InitializeInventory(List<ItemBase> initialItems = null)
    {
        items.Clear();

        if (initialItems != null)
        {
            foreach (var item in initialItems)
            {
                if (item != null)
                {
                    items.Add(item);
                }
            }
        }

        if (items.Count > MaxSlots)
        {
            items = items.GetRange(0, MaxSlots);
        }

        for (int i = items.Count; i < MaxSlots; i++)
        {
            items.Add(null);
        }

        Debug.Log($"Inventory initialized with {items.Count} items: {string.Join(", ", items.Select(item => item?.name ?? "Empty"))}");
    }

    public void AddItem(ItemBase item)
    {
        if(item == null)
        {
            Debug.LogWarning("Cannot add null item to inventory.");
            return;
        }
        if (!IsFull())
        {
            Debug.Log($"Added {item} to inventory.");
            items.Add(item);
        }
        else
        {
            Debug.LogWarning("Inventory is full. Cannot add more items.");
        }
    }

    public void RemoveItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < items.Count)
        {
            items[slotIndex] = null;
        }
    }

    public void ReplaceItem(int slotIndex, ItemBase newItem)
    {
        if (slotIndex >= 0 && slotIndex < items.Count)
        {
            items[slotIndex] = newItem;
        }
    }

    public List<ItemBase> GetItems()
    {
        return new List<ItemBase>(items);
    }

    public void ClearInventory()
    {
        items.Clear();
        Debug.Log("Inventory cleared.");
    }
    
    public bool IsFull()
    {
        return items.Count >= MaxSlots && items.All(item => item != null);
    }

    public bool IsEmpty()
    {
        return items.All(item => item == null);
    }
}