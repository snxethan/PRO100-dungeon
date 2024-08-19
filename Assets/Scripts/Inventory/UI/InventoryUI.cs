using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList; // Parent object containing item slots
    [SerializeField] ItemSlotUI itemSlotPrefab; // Prefab for displaying items in the inventory

    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    int selectedItem = 0;
    List<ItemSlotUI> slotUIList;

    Inventory inventory;

    private void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Inventory not found in the scene.");
            return;
        }
    }

    private void Start()
    {
        UpdateItemList();
    }

    void UpdateItemList()
    {
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.Slots)
        {
            var slotUIObj = Instantiate(itemSlotPrefab, itemList.transform);
            slotUIObj.SetData(itemSlot);
            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }

    public void HandleUpdate(Action onBack)
    {
        int prevSelection = selectedItem;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItem;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Slots.Count - 1);

        if (prevSelection != selectedItem)
            UpdateItemSelection();

        if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }

    void UpdateItemSelection()
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = Color.red; // Highlight selected item
            else
                slotUIList[i].NameText.color = Color.black;
        }

        var slot = inventory.Slots[selectedItem].Item;
        itemIcon.sprite = slot.Icon;
        itemDescription.text = slot.Description;
    }
}
