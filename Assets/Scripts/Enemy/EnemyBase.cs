using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/Create new Enemy")]
public class EnemyBase : ScriptableObject
{
    [SerializeField] private string name; // Name of the enemy

    [TextArea]
    [SerializeField] private string description; // Description of the enemy

    [SerializeField] private Sprite sprite; // Sprite of the enemy
    [SerializeField] private Sprite hurtSprite; // Sprite of the enemy when hurt

    [SerializeField] private EnemyType type; // Type of the enemy

    // Inventory logic
    [SerializeField] private List<ItemBase> attackItems;  // Items for attack
    [SerializeField] private List<ItemBase> defensiveItems;  // Items for defense
    [SerializeField] private List<ItemBase> recoveryItems;  // Items for recovery

    [SerializeField] private int randomItemCount = 0; // Total number of random items to add to the inventory

    // Public getters
    public List<ItemBase> AttackItems => attackItems; // Getter for attack items
    public List<ItemBase> DefensiveItems => defensiveItems; // Getter for defense items
    public List<ItemBase> RecoveryItems => recoveryItems; // Getter for recovery items
    public int RandomItemCount => randomItemCount; // Getter for random item count

    // Base stats
    [SerializeField] private int maxHP; // Maximum HP of the enemy
    [SerializeField] private int baseDefense; // Base defense of the enemy
    [SerializeField] private int baseAttack; // Base attack of the enemy
    [SerializeField] private int speed; // Speed of the enemy
    [SerializeField] private int level; // Level of the enemy

    public string Name => name; // Getter for the name
    public string Description => description; // Getter for the description
    public Sprite Sprite => sprite; // Getter for the sprite
    public Sprite HurtSprite => hurtSprite; // Getter for the hurt sprite
    public EnemyType Type => type; // Getter for the type
    public int MaxHP => maxHP; // Getter for the maximum HP
    public int BaseDefense => baseDefense; // Getter for the base defense
    public int BaseAttack => baseAttack; // Getter for the base attack
    public int Speed => speed; // Getter for the speed
    public int Level => level; // Getter for the level
}

public enum EnemyType
{
    None, // Default value
    Yokai,
    Demon,
    Gremlin,
    Possessed
}