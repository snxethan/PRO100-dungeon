using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Items/Create new Defensive item")]
public class DefensiveItems : ItemBase
{
    [Header("Damage Reduction")]
    [SerializeField] int dmgReduction;
    [SerializeField] bool fullReduction;
}
