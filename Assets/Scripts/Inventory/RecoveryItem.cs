using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new recovery item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;

    [Header("Stamina")]
    [SerializeField] int stamAmount;
    [SerializeField] bool restoreMaxStam;

    /*
     * this is for when we get Status Conditions
    [Header("Status Conditions)]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllStatus;
    */
}
  