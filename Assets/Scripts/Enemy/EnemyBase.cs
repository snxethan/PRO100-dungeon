using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/Create new Enemy")]
public class EnemyBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite Sprite;

    [SerializeField] EnemyType Type;

    //base stats
    [SerializeField] int maxHP;


}

public enum EnemyType
{
    None,
    Yokai,
    Gremlin,
    Demon,
    Oni,
    Possessed
}