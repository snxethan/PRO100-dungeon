using UnityEngine;

public enum ItemType
{
    AttackItem,
    DefenseItem,
    RecoveryItem
}
public class ItemBase : ScriptableObject
{
    [Header("Item Data")]
    [SerializeField] string itemName; // Name of the item
    [SerializeField] string description; // Description of the item
    [SerializeField] Sprite icon; // Icon of the item
    [SerializeField] int uses; // Number of uses of the item
    [SerializeField] bool unlimetedUse; // If the item has unlimited uses
    [SerializeField] public ItemType itemType; // Type of the item

    public string Name => itemName;
    public string Description => description;
    public Sprite Icon => icon;
    public int Uses => uses;
    public bool UnlimetedUse => unlimetedUse;
    public ItemType ItemType => itemType;

    public void SetUses(int newUses)
    {
        uses = newUses;
    }

    public virtual string GetItemTypeStr(int level)
    {
        return "Item Type";
    }

    public string GetItemDetails(int level)
    {
        if (UnlimetedUse)
        {
            return $"\u221e USES";
        }
        return $"{Uses} USES, Modifier: {GetItemModifier(level)}";
    }

    public bool UseItem()
    {
        Debug.Log($"Using {Name}");
        if (!UnlimetedUse)
        {
            uses--;
        }
        return uses <= 0;
    }

    public virtual int GetItemModifier(int level)
    {
        return 0;
    }
}