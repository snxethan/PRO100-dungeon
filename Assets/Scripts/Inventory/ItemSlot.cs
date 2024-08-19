using UnityEngine;

[System.Serializable]
public class ItemSlot
{
    public ItemBase Item { get; private set; }
    public int Quantity { get; private set; }

    public ItemSlot(ItemBase item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }

    public void SetItem(ItemBase item)
    {
        Item = item;
    }

    public void SetQuantity(int quantity)
    {
        Quantity = quantity;
    }
}