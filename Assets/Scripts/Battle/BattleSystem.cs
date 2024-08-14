using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHUD playerBattleHud;


    private void Start()
    {
        SetupBattle();
    }

    private void SetupBattle()
    {
        enemyUnit.Setup();
        playerBattleHud.SetData(enemyUnit);
    }
}
