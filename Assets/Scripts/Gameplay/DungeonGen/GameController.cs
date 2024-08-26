using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerControler;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    
    GameState state;
    

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerControler.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
    }
    

    private void StartBattle()
    {
        state = GameState.Battle;
        playerControler.TriggerBattle();
    }

    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        battleSystem.EndBattle(won);
        playerControler.StartTimer(4);
    }
    private void Update()
    {
        switch(state)
        {
            case GameState.FreeRoam:
                playerControler.HandleUpdate();
                break;
            case GameState.Battle:
                battleSystem.HandleUpdate();
                break;
        }
    }
}
