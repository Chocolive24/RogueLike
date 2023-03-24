using System;
using System.Collections;
using System.Collections.Generic;
using Packages.Rider.Editor.UnitTesting;
using UnityEditor.Search;
using UnityEngine;

public abstract class TileCell : MonoBehaviour
{
    // Attributes ------------------------------------------------------------------------------------------------------
    [SerializeField] private string _name;
    [SerializeField] protected SpriteRenderer _spriteRender;
    [SerializeField] protected GameObject _highlight;
    [SerializeField] private bool _isWalkable;

    private BaseUnit _occupiedUnit;
    
    protected int movementCost = 1;
    protected Vector2 _position;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public string Name => _name;
    public BaseUnit OccupiedUnit { get => _occupiedUnit; set => _occupiedUnit = value; }
    public bool Walkable => _isWalkable && _occupiedUnit == null;
    public Vector2 Position { get => _position; set => _position = value; }

    // Methods ---------------------------------------------------------------------------------------------------------
    public virtual void Init(int x, int y)
    {
        _position = new Vector2(x, y);
    }

    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile)
        {
            unit.OccupiedTile.OccupiedUnit = null;
        }
        unit.transform.position = transform.position;
        _occupiedUnit = unit;
        unit.OccupiedTile = this;
    }
    
    void OnMouseEnter()
    {
        _highlight.SetActive(true);
        
        UIBattleManager.Instance.ShowTileInfo(this);

        CheckForEnemyTilemapToCreate();
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
        
        UIBattleManager.Instance.ShowTileInfo(null);
        
        CheckForEnemyTilemapToDestroy();
    }
    
    private void OnMouseDown()
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
                
                TileCell startingTile = GridManager.Instance.GetTileAtPosition(new Vector2(x, y));
                
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
