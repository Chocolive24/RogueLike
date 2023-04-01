using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    // Singleton -------------------------------------------------------------------------------------------------------
    private static BattleManager _instance;
    public static BattleManager Instance { get { return _instance; } }
    
    // Attributes ------------------------------------------------------------------------------------------------------
    private bool _isPlayerTurn;
    
    // References ------------------------------------------------------------------------------------------------------

    #region Managers

    private GameManager _gameManager;
    private GridManager _gridManager;
    private UnitsManager _unitsManager;
    private UIBattleManager _uiBattleManager;

    #endregion
    
    // State Pattern ---------------------------------------------------------------------------------------------------
    
    #region Battle States

    private HeroesTurnBattleState _heroesTurnBattleState;
    private EnemiesTurnBattleState _enemiesTurnBattleState;
    private VictoryBattleState _victoryBattleState;
    private DefeatBattleState _defeatBattleState;

    #endregion
    
    // State Machine
    private StateMachine _stateMachine;
    
    // Public Methods --------------------------------------------------------------------------------------------------

    #region Getters and Setters
    
    public bool IsPlayerTurn { get => _isPlayerTurn; set => _isPlayerTurn = value; }
    
    public StateMachine StateMachine => _stateMachine;

    #endregion
    
    // Methods ---------------------------------------------------------------------------------------------------------
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
        
        UnitsManager.OnEnemiesTurnEnd += UnitsManagerOnOnEnemiesTurnEnd;
    }

    private void UnitsManagerOnOnEnemiesTurnEnd()
    {
        _isPlayerTurn = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        ReferenceManagers();

        CreateStatePattern();
    }

    private void ReferenceManagers()
    {
        _gameManager = GameManager.Instance;
        _gridManager = GridManager.Instance;
        _unitsManager = UnitsManager.Instance;
        _uiBattleManager = UIBattleManager.Instance;
    }

    private void CreateStatePattern()
    {
        _heroesTurnBattleState = new HeroesTurnBattleState(this, _uiBattleManager);
        _enemiesTurnBattleState = new EnemiesTurnBattleState();
        _victoryBattleState = new VictoryBattleState(_gameManager);
        _defeatBattleState = new DefeatBattleState();
        
        _stateMachine = new StateMachine();

        _stateMachine.AddTransition(_heroesTurnBattleState, _enemiesTurnBattleState,
            () => !_isPlayerTurn);
        _stateMachine.AddTransition(_enemiesTurnBattleState, _heroesTurnBattleState,
            () => _isPlayerTurn);
        _stateMachine.AddTransition(_heroesTurnBattleState, _victoryBattleState, 
            () => _unitsManager.Enemies.Count == 0);
        _stateMachine.AddTransition(_enemiesTurnBattleState, _victoryBattleState, 
            () => _unitsManager.Enemies.Count == 0);
    }
    
    public void StartBattle()
    {
        _gridManager.GenerateGrid();
        _unitsManager.SpawnHeroes();
        _unitsManager.SpawnEnemies();

        _isPlayerTurn = true;
        
        _uiBattleManager.BattlePanel.SetActive(true);
        
        _stateMachine.SetState(_heroesTurnBattleState);
    }

    public void EndBattle()
    {
        _uiBattleManager.BattlePanel.SetActive(false);
    }
    
    public void EnterHeroesTurn()
    {
        _isPlayerTurn = true;
        
        _uiBattleManager.EndTurnButton.interactable = true;
        
        _unitsManager.SetSelectedHero(_unitsManager.Heroes[0]);
        
        foreach (var hero in _unitsManager.Heroes)
        {
            hero.CurrentMana = hero.MaxMana;
        }
    }

    public void ExitHeroesTurn()
    {
        _uiBattleManager.EndTurnButton.interactable = false;
        
        _unitsManager.SetSelectedHero(null);
    }

    public void StartEnemiesTurn()
    {
        _unitsManager.CurrentEnemyPlaying = _unitsManager.Enemies[0];
    }
    
    public void HandleEnemiesTurn()
    {
        Debug.Log(_unitsManager.CurrentEnemyPlaying.Path.Count);
        if (_unitsManager.CurrentEnemyPlaying.Path.Count == 0)
        {
            Debug.Log("FIND PATH");
            _unitsManager.CurrentEnemyPlaying.FindAvailablePathToTarget(_unitsManager.Heroes[0].transform.position);
        }
        
        
        //StartCoroutine(TmpEnemyCo());
    }

    public IEnumerator TmpEnemyCo()
    {
        yield return new WaitForSeconds(1f);

        _isPlayerTurn = true;
    }
}
