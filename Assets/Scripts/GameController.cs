using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerControler;
    //add battle system serializable field when created

    GameState state;

    private void Start()
    {
        playerControler.OnEncountered += StartBattle;
        //battle system on battle over += EndBattle();
    }

    void StartBattle()
    {
        state = GameState.Battle;
        //battle system start
    }

    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        //battle system end
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerControler.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            //battle system update
        }
    }
}
