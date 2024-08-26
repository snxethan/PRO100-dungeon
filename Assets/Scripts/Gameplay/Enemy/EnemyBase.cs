using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/Create New Enemy")]
public class EnemyBase : ScriptableObject
{
    #region Fields and Properties
    [Header("General")]
    [SerializeField] private string enemyName;
    [SerializeField] private string description;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite hurtSprite;
    [SerializeField] private EnemyType type;

    [Header("Stats")]
    [SerializeField] private int baseAttack;
    [SerializeField] private int baseDefense;
    [SerializeField] private int maxHP;
    [SerializeField] private int speed;

    [Header("Items")]
    [SerializeField] private List<ItemBase> items;
    [SerializeField] private ItemType preferredItemType; // Add this property

    // Properties to access private fields
    public string Name => enemyName;
    public string Description => description;
    public Sprite Sprite => sprite;
    public Sprite HurtSprite => hurtSprite;
    public EnemyType Type => type;
    public int BaseAttack => baseAttack;
    public int BaseDefense => baseDefense;
    public int MaxHP => maxHP;
    public int Speed => speed;
    public List<ItemBase> Items => items;
    public ItemType PreferredItemType => preferredItemType; // Add this property
    #endregion
}

public enum EnemyType //the type of enemy that the player will face, this will determine the enemy's behavior
{
    Yokai,
    Demon,
    Gremlin,
    Possessed
}