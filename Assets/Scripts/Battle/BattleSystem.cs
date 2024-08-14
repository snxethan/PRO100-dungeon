using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleUnit playerUnit;



    private void Start()
    {
        SetupBattle();
    }

    private void SetupBattle()
    {
        playerUnit.Setup(true);
        enemyUnit.Setup(false);
    }

    
}
