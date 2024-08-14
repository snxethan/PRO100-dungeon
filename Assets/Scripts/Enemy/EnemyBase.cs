using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/Create new Enemy")]
public class EnemyBase : ScriptableObject
{
    [SerializeField] private string name;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite hurtSprite;

    [SerializeField] private EnemyType baseEnemyType = EnemyType.Yokai;
    [SerializeField] private EnemyType type;
    

    //base stats
    [SerializeField] private int maxHP;
    [SerializeField] private int baseDefense;
    [SerializeField] private int baseAttack;
    [SerializeField] private int speed;


    //getters
    public string Name
    {
        get { return this.name; }
    }


    public string Description 
        {
        get { return this.description; }
    }

    public Sprite Sprite
    {
        get { return this.sprite; }
    }

    public Sprite HurtSprite
    {
        get { return this.hurtSprite; }
    }

    public EnemyType Type
    {
        get { return this.type; }
    }

    public int MaxHP
    {
        get { return this.maxHP; }
    }
    public int BaseDefense
    {
        get { return this.baseDefense; }
    }
    public int BaseAttack
    {
        get { return this.baseAttack; }
    }
    public int Speed
    {
        get { return this.speed; }
    }



}

public enum EnemyType
{
    None,
    Yokai,
    Demon,
    Gremlin,
    Possessed
}
