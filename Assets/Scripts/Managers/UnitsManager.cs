using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitsManager : MonoBehaviour
{
    private static UnitsManager _instance;
    public static UnitsManager Instance { get { return _instance; } }

    private BaseHero _selectedHero;
    
    // TODO make a second list to store enemies. for now they are all in the same.
    private List<ScriptableUnit> _units;

    private List<BaseHero> _heroes;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public BaseHero SelectedHero { get => _selectedHero; set => _selectedHero = value; }

    public List<BaseHero> Heroes => _heroes;

    // -----------------------------------------------------------------------------------------------------------------
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

        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        
        _heroes = new List<BaseHero>();
        
        BattleManager.OnBattleStateChange += BattleManagerOnOnBattleStateChange;
    }

    private void BattleManagerOnOnBattleStateChange(BattleState battleState)
    {
        if (battleState == BattleState.HEROES_TURN)
        {
            SetSelectedHero(_heroes[0]);
        }
        else if (battleState == BattleState.ENEMIES_TURN)
        {
            SetSelectedHero(null);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Instantiate the hero.
        var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
        var spawnedHero = Instantiate(randomPrefab);
        _heroes.Add(spawnedHero);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnHeroes()
    {
        foreach (var hero in _heroes)
        {
            var rndSpawnedTile = GridManager.Instance.GetHeroSpawnTile();
            hero.transform.position = rndSpawnedTile.transform.position;
            rndSpawnedTile.SetUnit(hero);
        }
        
        //_heroes = new List<BaseHero>();
        
        // var heroCount = 1;
        //
        // for (int i = 0; i < heroCount; i++)
        // {
        //     var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
        //     var spawnedHero = Instantiate(randomPrefab);
        //     var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();
        //     
        //     randomSpawnTile.SetUnit(spawnedHero);
        //     
        //     _heroes.Add(spawnedHero);
        // }
        
        //GameManager.Instance.UpdateGameState(GameState.SPAWN_ENEMIES);
        BattleManager.Instance.UpdateBattleState(BattleState.SPAWN_ENEMIES);
    }
    
    public void SpawnEnemies()
    {
        var enemyCount = 1;

        for (int i = 0; i < enemyCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();
            spawnedEnemy.transform.position = randomSpawnTile.transform.position;
            randomSpawnTile.SetUnit(spawnedEnemy);
        }
        
        //GameManager.Instance.UpdateGameState(GameState.BATTLE);
        BattleManager.Instance.UpdateBattleState(BattleState.HEROES_TURN);
    }

    
    
    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction).OrderBy
            (o => Random.value).First().BaseUnitPrefab;
    }

    public void SetSelectedHero(BaseHero hero)
    {
        _selectedHero = hero;
        UIBattleManager.Instance.ShowSelectedHero(hero);
    }
}
