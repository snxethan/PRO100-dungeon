using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLayer : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        Debug.Log("Enemy Layer Detected");
        if (UnityEngine.Random.Range(1, 101) <= 10)
        {
            GameController.Instance.StartBattle();
        }
        else
        {
            Debug.Log("No encounter triggered.");
        }
    }
}
