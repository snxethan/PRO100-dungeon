using UnityEngine;

public enum ItemType
{
    AttackItem,
    DefenseItem,
    RecoveryItem
}

public class ItemBase : ScriptableObject
{
    
    #region Fields & Properties
    [Header("Item Data")]
    [SerializeField] private string itemName; // Name of the item
    [SerializeField] private string description; // Description of the item
    [SerializeField] private Sprite icon; // Icon of the item
    [SerializeField] private int defaultUses; // Default number of uses of the item
    [SerializeField] public bool unlimetedUse; // If the item has unlimited uses
    [SerializeField] public ItemType itemType; // Type of the item
    
    private int currentUses; // Current number of uses of the item

    public string Name => itemName;
    public string Description => description; 
    public Sprite Icon => icon;
    public int Uses => currentUses;
    public bool UnlimetedUse => unlimetedUse;
    public ItemType ItemType => itemType;
    #endregion

    private void Awake() // Initialize the item uses 
    {
        ResetUses();
    }

    #region Use Logic
    public void ResetUses() // Reset the item uses to the default value
    {
        SetUses(defaultUses);
    }

    /// <summary>
    /// set the uses of the item
    /// </summary>
    /// <param name="newUses"></param>
    public void SetUses(int newUses)
    {
        if (newUses == -1)
        {
            Debug.Log("Item has infinite uses");
            unlimetedUse = true;
        }
        else
        {
            currentUses = newUses;
        }
    }

    /// <summary>
    /// use the item and return if the item is out of uses
    /// </summary>
    /// <returns>if the item is out of uses or not</returns>
    public bool UseItem()
    {
        Debug.Log($"Using {Name}");
        if (!UnlimetedUse)
        {
            currentUses--;
        }
        return currentUses <= 0 && !UnlimetedUse;
    }
    #endregion

    public virtual string GetItemTypeStr(int level) // Get the item type
    {
        return "Item Type";
    }

    public string GetItemDetails() // Get the item details
    {
        if (UnlimetedUse)
        {
            return $"\u221e USES"; // Unicode for infinity symbol
        }
        return $"{currentUses} USES";
    }

    public virtual int GetItemModifier(int level) // Get the item modifier
    {
        return 0;
    }
}