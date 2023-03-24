using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum BattleState
{
    GENERATE_GRID = 0,
    SPAWN_HEROES,
    SPAWN_ENEMIES,
    HEROES_TURN,
    ENEMIES_TURN,
    VICTORY,
    DEFEAT,
}

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance;
    public static BattleManager Instance { get { return _instance; } }

    private BattleState _state;
    
    public static event Action<BattleState> OnBattleStateChange; 

    public BattleState State => _state;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateBattleState(BattleState newState)
    {
        _state = newState;

        switch (newState)
        {
            case BattleState.GENERATE_GRID:
                GridManager.Instance.GenerateGrid();
                break;
            case BattleState.SPAWN_HEROES:
                UnitsManager.Instance.SpawnHeroes();
                break;
            case BattleState.SPAWN_ENEMIES:
                UnitsManager.Instance.SpawnEnemies();
                break;
            case BattleState.HEROES_TURN:
                HandleHeroesTurn();
                break;
            case BattleState.ENEMIES_TURN:
                HandleEnemiesTurn();
                break;
            case BattleState.VICTORY:
                break;
            case BattleState.DEFEAT:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        
        OnBattleStateChange?.Invoke(newState);
    }

    
    public void StartBattle()
    {
        UpdateBattleState(BattleState.GENERATE_GRID);
        UpdateBattleState(BattleState.HEROES_TURN);

        Debug.Log(State);
    }
    
    private void HandleHeroesTurn()
    {
        foreach (var hero in UnitsManager.Instance.Heroes)
        {
            hero.CurrentMana = hero.MaxMana;
        }
    }
    
    private void HandleEnemiesTurn()
    {
        StartCoroutine(TmpEnemyCo());
    }

    private IEnumerator TmpEnemyCo()
    {
        yield return new WaitForSeconds(1f);
        
        UpdateBattleState(BattleState.HEROES_TURN);
    }
}
