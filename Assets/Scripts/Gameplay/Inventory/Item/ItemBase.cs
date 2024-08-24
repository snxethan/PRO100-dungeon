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
    [SerializeField] private string itemName; // Name of the item
    [SerializeField] private string description; // Description of the item
    [SerializeField] private Sprite icon; // Icon of the item
    [SerializeField] private int defaultUses; // Default number of uses of the item
    [SerializeField] public bool unlimetedUse; // If the item has unlimited uses
    [SerializeField] public ItemType itemType; // Type of the item
    
    private int currentUses;

    public string Name => itemName;
    public string Description => description;
    public Sprite Icon => icon;
    public int Uses => currentUses;
    public bool UnlimetedUse => unlimetedUse;
    public ItemType ItemType => itemType;

    private void Awake()
    {
        ResetUses();
    }

    public void ResetUses()
    {
        currentUses = defaultUses;
    }

    public void SetUses(int newUses)
    {
        currentUses = newUses;
    }

    public bool UseItem()
    {
        Debug.Log($"Using {Name}");
        if (!UnlimetedUse)
        {
            currentUses--;
        }
        return currentUses <= 0 && !UnlimetedUse;
    }

    public virtual string GetItemTypeStr(int level)
    {
        return "Item Type";
    }

    public string GetItemDetails()
    {
        if (UnlimetedUse)
        {
            return $"\u221e USES";
        }
        return $"{currentUses} USES";
    }

    public virtual int GetItemModifier(int level)
    {
        return 0;
    }
}