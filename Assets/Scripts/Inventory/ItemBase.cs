using UnityEngine;

public class ItemBase : ScriptableObject
{
    [Header("Item Data")]
    [SerializeField] string itemName; // Name of the item
    [SerializeField] string description; // Description of the item
    [SerializeField] Sprite icon; // Icon of the item
    [SerializeField] int uses; // Number of uses of the item
    [SerializeField] bool unlimetedUse; // If the item has unlimited uses

    public string Name => itemName;
    public string Description => description;
    public Sprite Icon => icon;
    public int Uses => uses;
    public bool UnlimetedUse => unlimetedUse;
}