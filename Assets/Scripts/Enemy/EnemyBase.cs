using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/Create New Enemy")]
public class EnemyBase : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string enemyName;
    [SerializeField] private string description;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite hurtSprite;
    [SerializeField] private EnemyType type;

    [Header("Items")]
    [SerializeField] private List<ItemBase> fixedItems;
    [SerializeField] private List<ItemBase> attackItems;
    [SerializeField] private List<ItemBase> defensiveItems;
    [SerializeField] private List<ItemBase> recoveryItems;

    [Header("Stats")]
    [SerializeField] private int maxHP;
    [SerializeField] private int baseDefense;
    [SerializeField] private int baseAttack;
    [SerializeField] private int speed;
    [SerializeField] private int level;

    public string Name => enemyName;
    public string Description => description;
    public Sprite Sprite => sprite;
    public Sprite HurtSprite => hurtSprite;
    public EnemyType Type => type;

    public List<ItemBase> FixedItems => fixedItems;
    public List<ItemBase> AttackItems => attackItems;
    public List<ItemBase> DefensiveItems => defensiveItems;
    public List<ItemBase> RecoveryItems => recoveryItems;

    public int MaxHP => maxHP;
    public int BaseDefense => baseDefense;
    public int BaseAttack => baseAttack;
    public int Speed => speed;
    public int Level => level;

    public void SetLevel(int newLevel)
    {
        level = Mathf.Max(1, newLevel);
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