using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileCell : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------
    [SerializeField] protected string _name;
    [SerializeField] protected bool _isWalkable;

    protected BaseUnit _occupiedUnit;
    
    protected int movementCost = 1;
    protected Vector3 _position;

    protected int _g;
    protected int _h;

    protected Vector3 _previousTilePos;
    
    // References ------------------------------------------------------------------------------------------------------
    [SerializeField] protected SpriteRenderer _spriteRender;
    
    #region GameObjects

    
    [SerializeField] protected GameObject _highlight;
    [SerializeField] protected GameObject _highlightBorder;
    [SerializeField] protected GameObject _arrow;

    #endregion
    
    [SerializeField] protected List<Sprite> _arrows;
    private ArrowTranslator _arrowTranslator;

    #region Managers

    private GameManager _gameManager;
    private GridManager _gridManager;
    private CardPlayedManager _cardPlayedManager;
    private UnitsManager _unitsManager;
    private UIBattleManager _uiBattleManager;
    private TilemapsManager _tilemapsManager;

    #endregion
    
    // Getters and Setters ---------------------------------------------------------------------------------------------

    #region Getters and Setters

    public string Name => _name;
    public BaseUnit OccupiedUnit { get => _occupiedUnit; set => _occupiedUnit = value; }
    public bool Walkable => _isWalkable && _occupiedUnit == null;
    public Vector3 Position { get => _position; set => _position = value; }

    public GameObject Arrow => _arrow;
    
    public List<Sprite> Arrows
    {
        get => _arrows;
        set => _arrows = value;
    }

    public int G
    {
        get => _g;
        set => _g = value;
    }

    public int H
    {
        get => _h;
        set => _h = value;
    }

    public int F => _g + _h;

    public Vector3 PreviousTilePos
    {
        get => _previousTilePos;
        set => _previousTilePos = value;
    }

    #endregion
    

    // Methods ---------------------------------------------------------------------------------------------------------

    private void Start()
    {
        _arrowTranslator = new ArrowTranslator();
        
        ReferenceManagers();
        _position = _gridManager.WorldToCellCenter(transform.position);
    }

    private void ReferenceManagers()
    {
        _gameManager = GameManager.Instance;
        _gridManager = GridManager.Instance;
        _unitsManager = UnitsManager.Instance;
        _cardPlayedManager = CardPlayedManager.Instance;
        _uiBattleManager = UIBattleManager.Instance;
        _tilemapsManager = TilemapsManager.Instance;
    }

    public virtual void Init(int x, int y)
    {
        
    }

    public void SetUnit(BaseUnit unit)
    {
        // if (unit.OccupiedTile)
        // {
        //     unit.OccupiedTile.OccupiedUnit = null;
        // }
        // _occupiedUnit = unit;
        // unit.OccupiedTile = this;

        // if (unit.OccupiedTiles.Count > 0)
        // {
        //     foreach (var tile in unit.OccupiedTiles)
        //     {
        //         tile.OccupiedUnit = null;
        //     }
        //     
        //     unit.OccupiedTiles.Clear();
        // }

        _occupiedUnit = unit;
        //unit.OccupiedTiles.Add(this);
    }
    
    protected virtual void OnMouseEnter()
    {
        _highlight.SetActive(true);
        _highlightBorder.SetActive(true);
        
        _uiBattleManager.ShowTileInfo(this);

        CheckForEnemyTilemapToCreate();
        
        if (_cardPlayedManager.CurrentCard != null)
        {
            if (_cardPlayedManager.CurrentCard.CardType == CardType.MoveCard &&
                _cardPlayedManager.CurrentCard.AvailableTiles.ContainsKey(_position) &&
                !_occupiedUnit)
            {
                BaseMoveCard card = (BaseMoveCard)_cardPlayedManager.CurrentCard;

                card.Path = _tilemapsManager.FindPathWithinRange(_position, card.AvailableTiles);
        
                _unitsManager.HeroPlayer.Path = card.Path.Keys.ToList();

                List<TileCell> pathTiles = new List<TileCell>();
                
                foreach (var item in card.Path)
                {
                    var tile = _gridManager.GetTileAtPosition(item.Key);
                    tile.Arrow.transform.rotation = Quaternion.identity;
                    tile.Arrow.SetActive(true);
                    pathTiles.Add(tile);
                }

                pathTiles.Reverse();

                for (int i = 0; i < pathTiles.Count; i++)
                {
                    var previousTile = i > 0 ? pathTiles[i - 1] : _unitsManager.HeroPlayer.GetOccupiedTiles().First();
                    var futureTile = i < pathTiles.Count - 1 ? pathTiles[i + 1] : null;

                    _arrowTranslator.DrawArrowPath(previousTile, pathTiles[i], futureTile);
                }
            }
        }
    }

    protected virtual void OnMouseExit()
    {
        _highlight.SetActive(false);
        _highlightBorder.SetActive(false);
        
        _uiBattleManager.ShowTileInfo(null);
        
        CheckForEnemyTilemapToDestroy();
        if (_cardPlayedManager.CurrentCard != null && 
            _cardPlayedManager.CurrentCard.AvailableTiles.ContainsKey(_position) &&
            !_occupiedUnit)
        {
            if (_cardPlayedManager.CurrentCard.CardType == CardType.MoveCard)
            {
                BaseMoveCard card = (BaseMoveCard)_cardPlayedManager.CurrentCard;
                
                foreach (var item in card.Path)
                {
                    var tile = _gridManager.GetTileAtPosition(item.Key);
                    tile.Arrow.SetActive(false);
                }
            }
        }
    }
    
    protected virtual void OnMouseDown()
    {
        if (!_gameManager.IsInBattleState)
        {
            return;
        }
        
        // If there is a unit on the tile we clicked.
        if (_occupiedUnit != null)
        {
            // // If the unit is a hero, set this hero to selected.
            // if (_occupiedUnit.Faction == Faction.Hero)
            // {
            //     _unitsManager.SetSelectedHero((BaseHero)_occupiedUnit);
            // }
            // Else, destroy the enemy on the tile we clicked.
            // else
            // {
                var enemy = (BaseEnemy)_occupiedUnit;

                enemy.IsSelected = !enemy.IsSelected;

                if (_unitsManager.HeroPlayer != null && 
                    _cardPlayedManager.HasAnAttackCardOnIt)
                {
                    // potential stuff to do
                    //enemy.TakeDamage();
                    // or
                    // UnitsManager.INstance.SelectedHero.Attack();
                    
                    if (_cardPlayedManager.CurrentCard.AvailableTiles.ContainsKey(this.Position) && enemy != null)
                    {
                        BaseAttackCard attackCard = (BaseAttackCard)_cardPlayedManager.CurrentCard;
                        enemy.IsSelected = false;
                        enemy.TakeDamage(attackCard.Damage);
                        //UnitsManager.Instance.SetSelectedHero(null);
                        _cardPlayedManager.HandlePlayedCard();
                    }
                }
            //}
        }
        
        // If there is no unit on the tile we clicked on.
        else
        {
            //Check if we have a selected hero and if we have played a moveCard.
            if (_unitsManager.HeroPlayer != null  && 
                _cardPlayedManager.HasAMoveCardOnIt)
            {
                // If so, move the hero to the tile where the player clicked if it is in the range of the aoe moveCard
                // and it is walkable.
                if (_cardPlayedManager.CurrentCard.AvailableTiles.ContainsKey(this.Position) && Walkable)
                {
                    _unitsManager.HeroPlayer.FindAvailablePathToTarget(transform.position, 0,
                        false, false, false);
                    //UnitsManager.Instance.SetSelectedHero(null);
                    _cardPlayedManager.HandlePlayedCard();
                }
            }
        }
    }
    
    private void CheckForEnemyTilemapToCreate()
    {
        if (_occupiedUnit != null && _unitsManager.HeroPlayer.CanPlay)
        {
            if (_occupiedUnit.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
            {
                int x = (int)_occupiedUnit.transform.position.x;
                int y = (int)_occupiedUnit.transform.position.y;

                var pos = _gridManager.CurrentRoomTilemap.WorldToCell(new Vector3(x, y, 0));
                
                TileCell startingTile = _gridManager.GetTileAtPosition(
                    _gridManager.WorldToCellCenter(pos));
                
                // If the Tilemap doesn't already exist, create and draw it.
                if (!enemy.MovementTilemap)
                {
                    enemy.MovementTilemap = _tilemapsManager.InstantiateTilemap("Enemy Movement");

                    enemy.AvailableTiles = enemy.GetAvailableTilesInRange(startingTile.Position,
                        _occupiedUnit.GetComponent<BaseEnemy>().Movement.Value, false, false);
                    
                    _tilemapsManager.DrawTilemap(enemy.AvailableTiles,
                        enemy.MovementTilemap, 
                        _tilemapsManager.AttackRuleTile);
                }

                if (!enemy.AttackTilemap)
                {
                    enemy.AttackTilemap = _tilemapsManager.InstantiateTilemap("Enemy Attack");
                    enemy.DrawAttackTiles();
                }
            }
        }
    }
    
    private void CheckForEnemyTilemapToDestroy()
    {
        if (_occupiedUnit != null)
        {
            if (_occupiedUnit.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
            {
                if (!enemy.IsSelected && enemy.MovementTilemap)
                {
                    Destroy(enemy.MovementTilemap.gameObject);
                    Destroy(enemy.AttackTilemap.gameObject);
                }
            }
        }
    }
}