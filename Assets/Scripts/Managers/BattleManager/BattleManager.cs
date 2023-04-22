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
    [SerializeField] private int _rewardsNbr = 3;
    
    private bool _isPlayerTurn;

    private List<BaseCard> _cardRewards = new List<BaseCard>();
    
    private Vector3 _heroSpawnPos;

    // References ------------------------------------------------------------------------------------------------------

    #region Managers

    private GameManager _gameManager;
    private GridManager _gridManager;
    private UnitsManager _unitsManager;
    private UIBattleManager _uiBattleManager;
    private CardsManager _cardsManager;

    #endregion
    
    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action<BattleManager> OnBattleStart;
    public static event Action<BattleManager> OnPlayerTurnStart;
    public static event Action<BattleManager> OnPlayerTurnEnd;
    public static event Action<BattleManager> OnEnemyTurnStart;
    public static event Action<BattleManager> OnEnemyTurnEnd;
    public static event Action<BattleManager> OnBattleEnd;

    // State Pattern ---------------------------------------------------------------------------------------------------
    
    #region Battle States

    private HeroesTurnBattleState _heroesTurnBattleState;
    private EnemiesTurnBattleState _enemiesTurnBattleState;
    private VictoryBattleState _victoryBattleState;
    private DefeatBattleState _defeatBattleState;
    private EndBattleState _endBattleState;

    #endregion
    
    // State Machine
    private StateMachine _stateMachine;
    private float _endTurnTime = 0.5f;

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
        
        UnitsManager.OnEnemiesTurnEnd += SetTurnToPlayerTurn;
        DoorTileCell.OnDoorTileEnter += GetSpawnTile;
    }

    

    private void SetTurnToPlayerTurn()
    {
        OnPlayerTurnStart?.Invoke(this);
        _isPlayerTurn = true;
    }

    private void GetSpawnTile(DoorTileCell doorTile)
    {
        _heroSpawnPos = doorTile.GetNextRoomSpawnPos();
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
        _cardsManager = CardsManager.Instance;
    }

    private void CreateStatePattern()
    {
        _heroesTurnBattleState = new HeroesTurnBattleState(this, _uiBattleManager);
        _enemiesTurnBattleState = new EnemiesTurnBattleState();
        _victoryBattleState = new VictoryBattleState(this);
        _defeatBattleState = new DefeatBattleState();
        _endBattleState = new EndBattleState(this);
        
        _stateMachine = new StateMachine();

        _stateMachine.AddTransition(_heroesTurnBattleState, _enemiesTurnBattleState,
            () => !_isPlayerTurn);
        _stateMachine.AddTransition(_enemiesTurnBattleState, _heroesTurnBattleState,
            () => _isPlayerTurn);
        _stateMachine.AddTransition(_heroesTurnBattleState, _victoryBattleState, 
            () => _unitsManager.Enemies.Count == 0);
        _stateMachine.AddTransition(_enemiesTurnBattleState, _victoryBattleState, 
            () => _unitsManager.Enemies.Count == 0);
        _stateMachine.AddTransition(_victoryBattleState, _endBattleState, 
            () => _cardRewards.Count < _rewardsNbr);
    }
    
    public void StartBattle()
    {
        //_gridManager.GenerateGrid();
        _unitsManager.SpawnHeroes(_heroSpawnPos);
        //_unitsManager.HandleSpawnEnemies();

        _isPlayerTurn = true;
        
        _uiBattleManager.BattlePanel.SetActive(true);
        
        _stateMachine.SetState(_heroesTurnBattleState);
    }

    public void EndBattle()
    {
        //_gridManager.DestroyGrid();
        
        _uiBattleManager.BattlePanel.SetActive(false);
        
        OnBattleEnd?.Invoke(this);
    }
    
    public void EnterHeroesTurn()
    {
        _isPlayerTurn = true;
        
        OnPlayerTurnStart?.Invoke(this);
        
        _uiBattleManager.EndTurnButton.interactable = true;
        _uiBattleManager.SetCurrentTurnText("Player Turn");
    }

    public void ExitHeroesTurn()
    {
        _uiBattleManager.EndTurnButton.interactable = false;
        
        OnPlayerTurnEnd?.Invoke(this);
    }

    public void StartEnemiesTurn()
    {
        OnEnemyTurnStart?.Invoke(this);
        _unitsManager.CurrentEnemyPlaying = _unitsManager.Enemies[0];
        _uiBattleManager.SetCurrentTurnText("Enemies Turn");
    }
    
    public void ExitEnemiesTurn()
    {
        _unitsManager.CurrentEnemyPlaying = null;
    }

    private IEnumerator EndTurnCo()
    {
        yield return new WaitForSeconds(_endTurnTime);
    }

    public void EnterVictory()
    {
        _cardRewards = _cardsManager.CreateCardRewards(_rewardsNbr);

        foreach (var card in _cardRewards)
        {
            card.OnCollected += HandleChosenReward;
        }
        
        //_uiBattleManager.VictoryPanel.SetActive(true);
    }

    private void HandleChosenReward(BaseCard obj)
    {
        _cardRewards.Remove(obj);
    }

    public void ExitVictory()
    {
        foreach (var card in _cardRewards)
        {
            Destroy(card.gameObject);
        }
        
        _cardRewards.Clear();

        _gameManager.IsInBattleState = false;
    }
}
