using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Packages.Rider.Editor.UnitTesting;
using UnityEditor.Search;
using UnityEngine;

public class TileCell : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------
    [SerializeField] protected string _name;
    [SerializeField] protected SpriteRenderer _spriteRender;
    [SerializeField] protected GameObject _highlight;
    [SerializeField] protected GameObject _highlightBorder;
    [SerializeField] protected GameObject _arrow;
    [SerializeField] protected bool _isWalkable;

    protected BaseUnit _occupiedUnit;
    
    protected int movementCost = 1;
    protected Vector3 _position;
    
    [SerializeField] protected List<Sprite> _arrows;
    private ArrowTranslator _arrowTranslator;

    // Getters and Setters ---------------------------------------------------------------------------------------------
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

    // Methods ---------------------------------------------------------------------------------------------------------

    private void Start()
    {
        _arrowTranslator = new ArrowTranslator();
    }

    public virtual void Init(int x, int y)
    {
        _position = GridManager.Instance.WorldToCellCenter(transform.position);
    }

    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile)
        {
            unit.OccupiedTile.OccupiedUnit = null;
        }
        _occupiedUnit = unit;
        unit.OccupiedTile = this;
    }
    
    protected virtual void OnMouseEnter()
    {
        _highlight.SetActive(true);
        _highlightBorder.SetActive(true);
        
        UIBattleManager.Instance.ShowTileInfo(this);

        CheckForEnemyTilemapToCreate();
        
        if (CardPlayedManager.Instance.CurrentCard != null)
        {
            if (CardPlayedManager.Instance.CurrentCard.CardType == CardType.MoveCard &&
                CardPlayedManager.Instance.CurrentCard.AvailableTiles.ContainsKey(_position) &&
                !_occupiedUnit)
            {
                BaseMoveCard card = (BaseMoveCard)CardPlayedManager.Instance.CurrentCard;

                card.Path = TilemapsManager.Instance.GetPath(_position, card.AvailableTiles);
        
                UnitsManager.Instance.SelectedHero.Path = card.Path;

                List<TileCell> pathTiles = new List<TileCell>();
                
                foreach (var item in card.Path)
                {
                    var tile = GridManager.Instance.GetTileAtPosition(item.Key);
                    tile.Arrow.transform.rotation = Quaternion.identity;
                    tile.Arrow.SetActive(true);
                    pathTiles.Add(tile);
                }

                pathTiles.Reverse();

                for (int i = 0; i < pathTiles.Count; i++)
                {
                    var previousTile = i > 0 ? pathTiles[i - 1] : UnitsManager.Instance.SelectedHero.OccupiedTile;
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
        
        UIBattleManager.Instance.ShowTileInfo(null);
        
        CheckForEnemyTilemapToDestroy();
        if (CardPlayedManager.Instance.CurrentCard != null && 
            CardPlayedManager.Instance.CurrentCard.AvailableTiles.ContainsKey(_position) &&
            !_occupiedUnit)
        {
            if (CardPlayedManager.Instance.CurrentCard.CardType == CardType.MoveCard)
            {
                BaseMoveCard card = (BaseMoveCard)CardPlayedManager.Instance.CurrentCard;
                
                foreach (var item in card.Path)
                {
                    Debug.Log(item);
                    var tile = GridManager.Instance.GetTileAtPosition(item.Key);
                    tile.Arrow.SetActive(false);
                }
            }
        }
    }
    
    protected virtual void OnMouseDown()
    {
        if (BattleManager.Instance.State != BattleState.HEROES_TURN)
        {
            return;
        }
        
        // If there is a unit on the tile we clicked.
        if (_occupiedUnit != null)
        {
            // If the unit is a hero, set this hero to selected.
            if (_occupiedUnit.Faction == Faction.Hero)
            {
                UnitsManager.Instance.SetSelectedHero((BaseHero)_occupiedUnit);
            }
            // Else, destroy the enemy on the tile we clicked.
            else
            {
                var enemy = (BaseEnemy)_occupiedUnit;

                enemy.IsSelected = !enemy.IsSelected;

                if (UnitsManager.Instance.SelectedHero != null && 
                    CardPlayedManager.Instance.HasAnAttackCardOnIt)
                {
                    // potential stuff to do
                    //enemy.TakeDamage();
                    // or
                    // UnitsManager.INstance.SelectedHero.Attack();
                    
                    if (CardPlayedManager.Instance.CurrentCard.AvailableTiles.ContainsKey(this.Position) && enemy != null)
                    {
                        enemy.IsSelected = false;
                        Destroy(enemy.MovementTilemap.gameObject);
                        Destroy(enemy.gameObject);
                        //UnitsManager.Instance.SetSelectedHero(null);
                        CardPlayedManager.Instance.HandlePlayedCard();
                    }
                }
            }
        }
        
        // If there is no unit on the tile we clicked on.
        else
        {
            //Check if we have a selected hero and if we have played a moveCard.
            if (UnitsManager.Instance.SelectedHero != null  && 
                CardPlayedManager.Instance.HasAMoveCardOnIt)
            {
                // If so, move the hero to the tile where the player clicked if it is in the range of the aoe moveCard
                // and it is walkable.
                if (CardPlayedManager.Instance.CurrentCard.AvailableTiles.ContainsKey(this.Position) && Walkable)
                {
                    UnitsManager.Instance.SelectedHero.HandleBattleMove();
                    SetUnit(UnitsManager.Instance.SelectedHero);
                    //UnitsManager.Instance.SetSelectedHero(null);
                    CardPlayedManager.Instance.HandlePlayedCard();
                }
            }
        }
    }
    
    private void CheckForEnemyTilemapToCreate()
    {
        if (_occupiedUnit != null)
        {
            if (_occupiedUnit.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
            {
                int x = (int)_occupiedUnit.transform.position.x;
                int y = (int)_occupiedUnit.transform.position.y;

                var pos = GridManager.Instance.WorldTilemap.WorldToCell(new Vector3(x, y, 0));
                
                TileCell startingTile = GridManager.Instance.GetTileAtPosition(
                    GridManager.Instance.WorldToCellCenter(pos));
                
                // If the Tilemap doesn't already exist, create and draw it.
                if (!enemy.MovementTilemap)
                {
                    enemy.MovementTilemap = TilemapsManager.Instance.InstantiateTilemap("Enemy Movement");

                    enemy.AvailableTiles = TilemapsManager.Instance.GetAvailableTiles(startingTile.Position,
                        _occupiedUnit.GetComponent<BaseEnemy>().Movement, enemy.gameObject);
                    
                    TilemapsManager.Instance.DrawTilemap(enemy.AvailableTiles,
                        enemy.MovementTilemap, 
                        TilemapsManager.Instance.AttackRuleTile);
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
                if (!enemy.IsSelected)
                {
                    Destroy(enemy.MovementTilemap.gameObject);
                }
            }
        }
    }
}
