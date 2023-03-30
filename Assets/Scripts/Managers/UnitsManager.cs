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

    // Attributes ------------------------------------------------------------------------------------------------------
    private List<ScriptableUnit> _units;

    private List<BaseHero> _heroes;
    private List<BaseEnemy> _enemies;
    
    private BaseHero _selectedHero;
    
    // References ------------------------------------------------------------------------------------------------------
    private GridManager _gridManager;
    private UIBattleManager _uiBattleManager;

    // Getters and Setters ---------------------------------------------------------------------------------------------
    public BaseHero SelectedHero { get => _selectedHero; set => _selectedHero = value; }

    public List<BaseHero> Heroes => _heroes;
    public List<BaseEnemy> Enemies => _enemies;

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
        _enemies = new List<BaseEnemy>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _gridManager = GridManager.Instance;
        _uiBattleManager = UIBattleManager.Instance;
        
        // Instantiate the hero.
        var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
        var spawnedHero = Instantiate(randomPrefab, new Vector3(7.5f, -10, 0), Quaternion.identity);
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
            var rndSpawnedTile = _gridManager.GetHeroSpawnTile();
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
    }
    
    public void SpawnEnemies()
    {
        var enemyCount = 1;

        for (int i = 0; i < enemyCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = _gridManager.GetEnemySpawnTile();
            spawnedEnemy.transform.position = randomSpawnTile.transform.position;
            randomSpawnTile.SetUnit(spawnedEnemy);
            _enemies.Add(spawnedEnemy);
        }
    }
    
    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction).OrderBy
            (o => Random.value).First().BaseUnitPrefab;
    }

    public void SetSelectedHero(BaseHero hero)
    {
        _selectedHero = hero;
        _uiBattleManager.ShowSelectedHero(hero);
    }
}
