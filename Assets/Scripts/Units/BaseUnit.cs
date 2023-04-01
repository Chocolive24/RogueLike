using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------

    #region Tweakable Values

    [SerializeField] private string _unitName;
    [SerializeField] private Faction _faction;
    [SerializeField] protected int _hp;
    [SerializeField] protected int _attack;
    [SerializeField] protected int _shield;
    [SerializeField] protected int _movement;
    
    #endregion
    
    public string UnitName => _unitName;
    
    private TileCell _occupiedTile;

    [SerializeField] protected float _speed = 8f;
    
    protected Vector3? _targetPos = null;
    protected Dictionary<Vector3, int> _availableTiles;
    protected List<Vector3> _path;
    protected int _currentTargetIndex = 0;

    protected List<Vector3> _avalaiblePath;
    
    // References ------------------------------------------------------------------------------------------------------
    protected GameManager _gameManager;
    protected GridManager _gridManager;
    protected TilemapsManager _tilemapsManager;
    protected UnitsManager _unitsManager;

    // Events ----------------------------------------------------------------------------------------------------------
    public event Action OnTurnFinished; 
    
    // Getters and Setters ---------------------------------------------------------------------------------------------

    #region Getters and Setters

    public TileCell OccupiedTile { get => _occupiedTile; set => _occupiedTile = value; }
    
    public Faction Faction => _faction;
    
    
    public int Movement
    {
        get => _movement;
        set => _movement = value;
    }

    public Vector3? TargetPos
    {
        get => _targetPos;
        set => _targetPos = value;
    }

    public List<Vector3> Path
    {
        get => _path;
        set => _path = value;
    }

    #endregion

    // Methods ---------------------------------------------------------------------------------------------------------
    protected virtual void Start()
    {
        _availableTiles = new Dictionary<Vector3, int>();
        _path = new List<Vector3>();
        _avalaiblePath = new List<Vector3>();
        
        _gameManager = GameManager.Instance;
        _gridManager = GridManager.Instance;
        _tilemapsManager = TilemapsManager.Instance;
        _unitsManager = UnitsManager.Instance;
    }

    protected virtual void Update()
    {
        MoveOnGrid();
    }

    public virtual void FindAvailablePathToTarget(Vector3 targetPos)
    {
        _path = _tilemapsManager.FindPath(transform.position, targetPos);
        
        // If the enemy is already near the player, there would be no path, so we don't need to move the enemy.
        if (_path.Count > 0)
        {
            _avalaiblePath = _availableTiles.Keys.Intersect(_path).ToList();
        
            _targetPos = _avalaiblePath.First();
        }
        else
        {
            _targetPos = transform.position;
        }
    }
    
    protected virtual void MoveOnGrid()
    {
        if (_targetPos.HasValue)
        {
            // Move the player to the target position
            transform.position = Vector3.MoveTowards(transform.position, _targetPos.Value,
                _speed * Time.deltaTime);

            // The distance between the player and the target point would never be exactly equal to 0.
            // So we check with an Epsilon value if the player as reached the target position.
            // Then we set his position to the target position in order to be precise.
            if (Vector3.Distance(transform.position, _targetPos.Value) <= 0.01f)
            {
                transform.position = _targetPos.Value;
                _targetPos = null;

                if (_gameManager.IsInBattleState)
                {
                    FollowThePath();
                }
            }
        }
    }

    protected virtual void FollowThePath()
    {
        if (_currentTargetIndex < _avalaiblePath.Count) 
        {
            _targetPos = _gridManager.WorldToCellCenter(_avalaiblePath[_currentTargetIndex]);
            _currentTargetIndex++;
        }
        else
        { 
            OnTurnFinished?.Invoke();
            _gridManager.GetTileAtPosition(transform.position).SetUnit(this);
            _targetPos = null;
            _currentTargetIndex = 0;
            _path.Clear();
            _avalaiblePath.Clear();
        }
    }

    public virtual void DisplayStats()
    {
        Debug.Log(_hp + " " +  _attack + " " + _shield);
    }
    
    public void Kill()
    {
        Destroy(this.gameObject);
    }
}
