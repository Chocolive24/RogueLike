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
    private List<ScriptableUnit> _enemiesData;

    private List<BaseHero> _heroes;
    private List<BaseEnemy> _enemies;
    
    private BaseEnemy _currentEnemyPlaying;

    private int _enemyCount = 0;

    // References ------------------------------------------------------------------------------------------------------
    private GridManager _gridManager;
    private UIBattleManager _uiBattleManager;

    [SerializeField] private AI_TypeSO _aiTypeSO;
    private float _betweenTurnsTime = 0.5f;

    // Events ----------------------------------------------------------------------------------------------------------
    public static event Action OnEnemiesTurnEnd;

    public static event Action<UnitsManager> OnHeroSpawn;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public BaseHero HeroPlayer => _heroes[0];

    public BaseEnemy CurrentEnemyPlaying { get => _currentEnemyPlaying; set => _currentEnemyPlaying = value; }

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
        _enemiesData = Resources.LoadAll<ScriptableUnit>("Units/Enemies").ToList();
        
        _heroes = new List<BaseHero>();
        _enemies = new List<BaseEnemy>();

        DoorTileCell.OnDoorTileEnter += SpawnRoomEnemies;

        //Room.OnRoomEnter += SpawnEntities;
    }

    private void SpawnRoomEnemies(DoorTileCell doorTile)
    {
        RoomData roomToSpawnEnemy = doorTile.GetRoomNeighbour();

        if (roomToSpawnEnemy != null)
        {
            int remainginWeight = roomToSpawnEnemy.EnemySpawnWeight;
            
            while (remainginWeight > 0)
            {
                BaseEnemy enemy = GetRandomEnemyUnderWeight(remainginWeight);
            
                var spawnedEnemy = Instantiate(enemy);
            
                var randomSpawnPos = roomToSpawnEnemy.GetARandomTilePosition();
                spawnedEnemy.transform.position = _gridManager.WorldToCellCenter(randomSpawnPos);
            
                spawnedEnemy.PreviousOccupiedTiles = spawnedEnemy.GetOccupiedTiles();
            
                foreach (var tile in spawnedEnemy.GetOccupiedTiles())
                {
                    tile.SetUnit(spawnedEnemy);
                }
            
                _enemies.Add(spawnedEnemy);
            
                spawnedEnemy.OnTurnFinished += SetNextEnemyTurn;
                spawnedEnemy.OnDeath += HandleEnemyDeath;
            
                remainginWeight -= spawnedEnemy.Weight;
            }
        }
    }

    private BaseEnemy GetRandomEnemyUnderWeight(int weight)
    {
        return (BaseEnemy)(_enemiesData.Where(enemyUnit => (
            (BaseEnemy)enemyUnit.BaseUnitPrefab).Weight <= weight).OrderBy(
            o => Random.value).First()).BaseUnitPrefab;
    }
    
    // private void SpawnEntities(Room room)
    // {
    //     SpawnHeroes();
    //     HandleSpawnEnemies();
    // }


    // Start is called before the first frame update
    void Start()
    {
        _gridManager = GridManager.Instance;
        _uiBattleManager = UIBattleManager.Instance;
        
        // Instantiate the hero.
        var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
        var rndSpawnedTile = _gridManager.GetHeroSpawnTile();

        var spawnedHero = Instantiate(randomPrefab, rndSpawnedTile.transform.position, Quaternion.identity);
        
        spawnedHero.OnDeath += HandleHeroDeath;
        
        spawnedHero.PreviousOccupiedTiles = spawnedHero.GetOccupiedTiles();
        
        foreach (var tile in spawnedHero.GetOccupiedTiles())
        {
            tile.SetUnit(spawnedHero);
        }
        
        _heroes.Add(spawnedHero);
        
        OnHeroSpawn?.Invoke(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnHeroes(Vector3 spawnPos)
    {
        foreach (var hero in _heroes)
        {
            hero.transform.position = spawnPos;

            foreach (var tile in hero.PreviousOccupiedTiles)
            {
                tile.OccupiedUnit = null;
            }
            
            hero.PreviousOccupiedTiles = hero.GetOccupiedTiles();

            foreach (var tile in hero.GetOccupiedTiles())
            {
                tile.SetUnit(hero);
            }
        }
    }

    private void HandleHeroDeath(BaseUnit obj)
    {
        // TODO
    }

    public void HandleSpawnEnemies()
    {
        switch (_aiTypeSO.Type)
        {
            case EnemyType.GOBLIN:
                SpawnEnemy(3, EnemyType.GOBLIN);
                break;
            case EnemyType.ARCHER:
                SpawnEnemy(3, EnemyType.ARCHER);
                break;
            case EnemyType.TANK:
                SpawnEnemy(2, EnemyType.TANK);
                break;
            case EnemyType.SPAWNER:
                SpawnEnemy(1, EnemyType.SPAWNER);
                break;
            case EnemyType.MIX:
                SpawnEnemy(1, EnemyType.MIX);
                break;
        }
        
        // var enemyCount = 3;
        //
        // for (int i = 0; i < enemyCount; i++)
        // {
        //     var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
        //     var spawnedEnemy = Instantiate(randomPrefab);
        //     var randomSpawnTile = _gridManager.GetEnemySpawnTile();
        //     spawnedEnemy.transform.position = randomSpawnTile.transform.position;
        //     randomSpawnTile.SetUnit(spawnedEnemy);
        //     _enemies.Add(spawnedEnemy);
        //     
        //     spawnedEnemy.OnTurnFinished += SetNextEnemyTurn;
        // }
    }
    
    public void SpawnEnemy(int enemyNbr, EnemyType enemyType)
    {
        for (int i = 0; i < enemyNbr; i++)
        {
            BaseEnemy enemyData;
            
            if (enemyType == EnemyType.MIX)
            {
                // BaseEnemy enemyData2;
                //
                // BaseEnemy enemyData3;
                //
                // enemyData = GetAnEnemyByType(EnemyType.GOBLIN);
                //
                // var spawnedEnemy = Instantiate(enemyData);
                //
                // spawnedEnemy.transform.position = _gridManager.WorldToCellCenter(new Vector3(0, 6.5f, 0));
                //
                // spawnedEnemy.PreviousOccupiedTiles = spawnedEnemy.GetOccupiedTiles();
                //
                // foreach (var tile in spawnedEnemy.GetOccupiedTiles())
                // {
                //     tile.SetUnit(spawnedEnemy);
                // }
                //
                // //randomSpawnTile.SetUnit(spawnedEnemy);
                // _enemies.Add(spawnedEnemy);
                //
                // spawnedEnemy.OnTurnFinished += SetNextEnemyTurn;
                // spawnedEnemy.OnDeath += HandleEnemyDeath;
                //
                // // ----------------------------------------------------------------------------------------
                //
                // // enemyData = GetAnEnemyByType(EnemyType.GOBLIN);
                // //
                // // var spawnedEnemy = Instantiate(enemyData);
                // //
                // // spawnedEnemy.transform.position = _gridManager.WorldToCellCenter(new Vector3(0, 1.5f, 0));
                // //
                // // spawnedEnemy.PreviousOccupiedTiles = spawnedEnemy.GetOccupiedTiles();
                // //
                // // foreach (var tile in spawnedEnemy.GetOccupiedTiles())
                // // {
                // //     tile.SetUnit(spawnedEnemy);
                // // }
                // //
                // // //randomSpawnTile.SetUnit(spawnedEnemy);
                // // _enemies.Add(spawnedEnemy);
                // //
                // // spawnedEnemy.OnTurnFinished += SetNextEnemyTurn;
                // // spawnedEnemy.OnDeath += HandleEnemyDeath;
                //
                // // ----------------------------------------------------------------------------------------
                //
                // enemyData2 = GetAnEnemyByType(EnemyType.TANK);
                //
                // var spawnedEnemy2 = Instantiate(enemyData2);
                //
                // spawnedEnemy2.transform.position = _gridManager.WorldToCellCenter(new Vector3(3, 6.5f, 0));
                //
                // spawnedEnemy2.PreviousOccupiedTiles = spawnedEnemy2.GetOccupiedTiles();
                //
                // foreach (var tile in spawnedEnemy2.GetOccupiedTiles())
                // {
                //     tile.SetUnit(spawnedEnemy2);
                // }
                //
                // //randomSpawnTile.SetUnit(spawnedEnemy);
                // _enemies.Add(spawnedEnemy2);
                //
                // spawnedEnemy2.OnTurnFinished += SetNextEnemyTurn;
                // spawnedEnemy2.OnDeath += HandleEnemyDeath;

                do
                {
                    enemyData = GetRandomUnit<BaseEnemy>(Faction.Enemy);
                } while (enemyData.UnitName == "Minion");
            }
            else
            {
                enemyData = GetAnEnemyByType(enemyType);
            }
            
            var spawnedEnemy = Instantiate(enemyData);
            
            var randomSpawnTile = _gridManager.GetEnemySpawnTile();
            spawnedEnemy.transform.position = randomSpawnTile.transform.position;
            
            spawnedEnemy.PreviousOccupiedTiles = spawnedEnemy.GetOccupiedTiles();
            
            foreach (var tile in spawnedEnemy.GetOccupiedTiles())
            {
                tile.SetUnit(spawnedEnemy);
            }
            
            //randomSpawnTile.SetUnit(spawnedEnemy);
            _enemies.Add(spawnedEnemy);
            
            spawnedEnemy.OnTurnFinished += SetNextEnemyTurn;
            spawnedEnemy.OnDeath += HandleEnemyDeath;
        }
    }

    

    public void SetNextEnemyTurn()
    {
        StartCoroutine(WaitBeforeNextActionCo());
    }

    private void HandleEnemyDeath(BaseUnit obj)
    {
        _enemies.Remove((BaseEnemy)obj);
    }
    
    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction).OrderBy
            (o => Random.value).First().BaseUnitPrefab;
    }

    private BaseEnemy GetAnEnemyByType(EnemyType type)
    {
        return (BaseEnemy)_enemiesData.Find(
            x => x.BaseUnitPrefab.GetComponent<BaseEnemy>().Type == type).BaseUnitPrefab;
    }
    
    private IEnumerator WaitBeforeNextActionCo()
    {
        yield return new WaitForSeconds(_betweenTurnsTime);
        
        if (_enemyCount < _enemies.Count - 1)
        {
            _enemyCount++;
            _currentEnemyPlaying = _enemies[_enemyCount];
            //_currentEnemyPlaying.BehaviorTree.SetupTree();
        }
        else
        {
            OnEnemiesTurnEnd?.Invoke();
            _enemyCount = 0;
        }
    }
    
    // public void SetSelectedHero(BaseHero hero)
    // {
    //     _selectedHero = hero;
    //     _uiBattleManager.ShowSelectedHero(hero);
    // }
}
