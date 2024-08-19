using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/Create New Enemy")]
public class EnemyBase : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string enemyName; // Name of the enemy
    [SerializeField] private string description; // Description of the enemy
    [SerializeField] private Sprite sprite; // Sprite of the enemy
    [SerializeField] private Sprite hurtSprite; // Sprite of the enemy when hurt
    [SerializeField] private EnemyType type = EnemyType.Yokai; // Type of the enemy

    [Header("Items")]
    [SerializeField] private List<ItemBase> fixedItems; // Fixed items the enemy will always have
    [SerializeField] private List<ItemBase> attackItems; // List of attack items
    [SerializeField] private List<ItemBase> defensiveItems; // List of defensive items
    [SerializeField] private List<ItemBase> recoveryItems; // List of recovery items

    [Header("Stats")]
    [SerializeField] private int maxHP; // Maximum HP of the enemy
    [SerializeField] private int baseDefense; // Base defense of the enemy
    [SerializeField] private int baseAttack; // Base attack of the enemy
    [SerializeField] private int speed; // Speed of the enemy
    [SerializeField] private int level = 1; // Level of the enemy

    public string Name => enemyName; // Name of the enemy
    public string Description => description; // Description of the enemy
    public Sprite Sprite => sprite; // Sprite of the enemy
    public Sprite HurtSprite => hurtSprite; // Sprite of the enemy when hurt
    public EnemyType Type => type; // Type of the enemy

    public List<ItemBase> FixedItems => fixedItems; // List of fixed items
    public List<ItemBase> AttackItems => attackItems; // List of attack items
    public List<ItemBase> DefensiveItems => defensiveItems; // List of defensive items
    public List<ItemBase> RecoveryItems => recoveryItems; // List of recovery items

    public int MaxHP => maxHP; // Maximum HP of the enemy
    public int BaseDefense => baseDefense; // Base defense of the enemy
    public int BaseAttack => baseAttack; // Base attack of the enemy
    public int Speed => speed; // Speed of the enemy
    public int Level => level; // Level of the enemy

    public void SetLevel(int newLevel)
    {
        level = Mathf.Max(1, newLevel); // Ensure level is at least 1
    }

    public void SetBaseStats(int newHP, int newAttack, int newDefense, int newSpeed)
    {
        maxHP = newHP;
        baseAttack = newAttack;
        baseDefense = newDefense;
        speed = newSpeed;
    }
}


public enum EnemyType
{
    Yokai,
    Demon,
    Gremlin,
    Possessed
}
