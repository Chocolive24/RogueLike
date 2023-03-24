using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public enum GameState
{
    EXPLORING,
    BATTLE,
    VICTORY,
    DEFEAT
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private GameState _state;
    public GameState State => _state;

    public static event Action<GameState> OnGameStateChange;

    [SerializeField] private TextMeshProUGUI _currentState;

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
        //UpdateGameState(GameState.GENERATE_GRID);
        UpdateGameState(GameState.EXPLORING);

        StartCoroutine(TestCo());
        
        
    }

    private IEnumerator TestCo()
    {
        yield return new WaitForSeconds(2f);
        UpdateGameState(GameState.BATTLE);
    }
    
    // Update is called once per frame
    void Update()
    {
        _currentState.text = _state.ToString();
    }

    public void UpdateGameState(GameState newState)
    {
        _state = newState;

        switch (newState)
        {
            // case GameState.GENERATE_GRID:
            //     Debug.Log("1");
            //     GridManager.Instance.GenerateGrid();
            //     break;
            // case GameState.SPAWN_HEROES:
            //     Debug.Log("2");
            //     UnitsManager.Instance.SpawnHeroes();
            //     break;
            // case GameState.SPAWN_ENEMIES:
            //     Debug.Log("3");
            //     UnitsManager.Instance.SpawnEnemies();
            //     break;
            case GameState.EXPLORING:
                break;
            case GameState.BATTLE:
                BattleManager.Instance.StartBattle();
                break;
            // case GameState.HeroesTurn:
            //     HandleHeroesTurn();
            //     break;
            // case GameState.EnemiesTurn:
            //     break;
            case GameState.VICTORY:
                break;
            case GameState.DEFEAT:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChange?.Invoke(newState);
    }
}
