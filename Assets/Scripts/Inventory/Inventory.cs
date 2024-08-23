using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<ItemBase> fixedItems = new List<ItemBase>();
    private List<ItemBase> dynamicItems = new List<ItemBase>();
    private const int MaxSlots = 4;

    private void Awake()
    {
        InitializeInventory();
    }

    public void InitializeInventory(List<ItemBase> fixedItemsList = null)
    {
        fixedItems.Clear();
        dynamicItems.Clear();

        if (fixedItemsList != null)
        {
            foreach (var item in fixedItemsList)
            {
                fixedItems.Add(Instantiate(item));
            }
        }

        if (fixedItems.Count > MaxSlots)
        {
            fixedItems = fixedItems.GetRange(0, MaxSlots);
        }

        for (int i = fixedItems.Count; i < MaxSlots; i++)
        {
            dynamicItems.Add(null);
        }

        Debug.Log($"Inventory initialized with {fixedItems.Count} fixed items and {dynamicItems.Count} dynamic slots.");
    }

    public void AddFixedItem(ItemBase item)
    {
        if (fixedItems.Count < MaxSlots)
        {
            fixedItems.Add(item);
            Debug.Log($"Added fixed item: {item.Name}");
        }
        else
        {
            Debug.LogWarning("Cannot add more fixed items. Inventory is full.");
        }
    }

    public void RemoveItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < fixedItems.Count)
        {
            fixedItems[slotIndex] = null;
        }
        else
        {
            int dynamicIndex = slotIndex - fixedItems.Count;
            if (dynamicIndex >= 0 && dynamicIndex < dynamicItems.Count)
            {
                dynamicItems[dynamicIndex] = null;
            }
        }
    }

    public void ReplaceDynamicItem(int slotIndex, ItemBase newItem)
    {
        int dynamicIndex = slotIndex - fixedItems.Count;
        if (dynamicIndex >= 0 && dynamicIndex < dynamicItems.Count)
        {
            dynamicItems[dynamicIndex] = newItem;
            Debug.Log($"Replaced dynamic item in slot {slotIndex} with {newItem.Name}");
        }
        else if (slotIndex >= 0 && slotIndex < fixedItems.Count)
        {
            fixedItems[slotIndex] = newItem;
            Debug.Log($"Replaced fixed item in slot {slotIndex} with {newItem.Name}");
        }
        else
        {
            Debug.LogError("Invalid slot index.");
        }
    }

    public void CheckAndConvertFixedSlots()
    {
        for (int i = 0; i < fixedItems.Count; i++)
        {
            if (fixedItems[i] != null && fixedItems[i].Uses <= 0)
            {
                fixedItems[i] = null;
                dynamicItems.Add(null);
                Debug.Log($"Converted fixed item slot {i} to dynamic slot.");
            }
        }
    }

    public void PopulateDynamicSlots(List<ItemBase> availableItems, List<ItemBase> attackItems)
    {
        if (availableItems == null || availableItems.Count == 0)
        {
            Debug.LogError("No available items to populate dynamic slots.");
            return;
        }

        bool hasAttackItem = false;

        for (int i = 0; i < dynamicItems.Count; i++)
        {
            if (dynamicItems[i] == null)
            {
                ItemBase selectedItem = availableItems[Random.Range(0, availableItems.Count)];
                dynamicItems[i] = selectedItem;

                if (attackItems.Contains(selectedItem))
                {
                    hasAttackItem = true;
                }
            }
        }

        if (!hasAttackItem && attackItems.Count > 0)
        {
            dynamicItems[0] = attackItems[Random.Range(0, attackItems.Count)];
        }

        Debug.Log("Dynamic slots populated with random items.");
    }

    public List<ItemBase> GetItems()
    {
        List<ItemBase> allItems = new List<ItemBase>(fixedItems);
        allItems.AddRange(dynamicItems);
        allItems.RemoveAll(item => item == null); // Remove null items

        return allItems;
    }

    public void ClearInventory()
    {
        fixedItems.Clear();
        dynamicItems.Clear();
        Debug.Log("Inventory cleared.");
    }
}