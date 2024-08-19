using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
       
        
        if (Random.Range(1, 101) <= 10)
        {
            Debug.Log("Enemy Encounter");
            //call game controller when made to start battle
        }
    }
}
