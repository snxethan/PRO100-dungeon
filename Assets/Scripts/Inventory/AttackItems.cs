using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Items/Create new Attack item")]
public class AttackItems : ItemBase
{
    [Header("Damage")]
    [SerializeField] int dmgAmount;
}
