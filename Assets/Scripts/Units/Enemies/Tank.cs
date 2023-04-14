using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Tank : BaseEnemy
{
    private int _nbrOfMovement = 0;
    
    public override bool IsPositionAvailable(Vector3 position, bool countHeroes, bool countEnemies, bool countWalls)
    {
        bool isPosAvalaible = false;

        if (position == _unitsManager.HeroPlayer.transform.position)
        {
            return true;
        }
        
        var possibleAvalaibleTiles = GetTilesInOccupiedRange(position);
    
        if (possibleAvalaibleTiles != null)
        {
            foreach (var tile in possibleAvalaibleTiles)
            {
                if (base.IsPositionAvailable(tile.transform.position, true, false, 
                        false) || GetOccupiedTiles().Contains(tile))
                {
                    isPosAvalaible = true;
                }
                else
                {
                    isPosAvalaible = false;
                    break;
                }
            }
        }
        
        return isPosAvalaible;
    }

    public override void FindAvailablePathToTarget(Vector3 targetPos, int minimumPathCount, 
        bool countHeroes, bool countEnemies, bool countWalls)
    {
        if (GetTilesInAttackRange(transform.position, _attackRange).ContainsKey(
                _unitsManager.HeroPlayer.transform.position))
        {
            _path = new List<Vector3>();
        }
        else
        {
            base.FindAvailablePathToTarget(targetPos, minimumPathCount, countHeroes, countEnemies, countWalls);
        }
        
    }

    protected override void FollowThePath()
    {
        if (_currentTargetIndex < _avalaiblePath.Count - 1 && _nbrOfMovement < _movement.Value - 1) 
        {
            _currentTargetIndex++;

            var nextTile = _gridManager.GetTileAtPosition(_avalaiblePath[_currentTargetIndex]);
            
            if (GetTilesInAttackRange(transform.position, _attackRange).ContainsKey(
                    _unitsManager.HeroPlayer.transform.position))
            {
                StopThePath();
                _nbrOfMovement = 0;
            }
            else
            {
                _targetPos = _gridManager.WorldToCellCenter(_avalaiblePath[_currentTargetIndex]);
                _nbrOfMovement++;
            }
        }
        
        else
        {
            StopThePath();
            _nbrOfMovement = 0;
        }
    }

    // public override bool IsPositionAvailable(Vector3 position, bool countHeroes, bool countEnemies)
    // {
    //     if (_gridManager.GetTileAtPosition(position) != null)
    //     {
    //         TileCell tile = _gridManager.GetTileAtPosition(position);
    //
    //         Vector3 tilePos = tile.transform.position;
    //
    //         if ((tile.Walkable &&
    //             GridManager.Instance.GetTileAtPosition(new Vector3(tilePos.x + 1, tilePos.y, 0)).Walkable &&
    //             GridManager.Instance.GetTileAtPosition(new Vector3(tilePos.x, tilePos.y + 1, 0)).Walkable &&
    //             GridManager.Instance.GetTileAtPosition(new Vector3(tilePos.x + 1, tilePos.y + 1, 0)).Walkable 
    //             || GetOccupiedTiles().Contains(tile))
    //             )
    //         {
    //             return true;
    //         }
    //         
    //         if (countHeroes && tile.OccupiedUnit != null)
    //         {
    //             if (tile.OccupiedUnit.Faction == Faction.Hero)
    //             {
    //                 return true;
    //             }
    //             
    //         }
    //         if (countEnemies && tile.OccupiedUnit != null)
    //         {
    //             if (tile.OccupiedUnit.Faction == Faction.Enemy)
    //             {
    //                 return true;
    //             }
    //         }
    //
    //         return tile.Walkable;
    //     }
    //
    //     return false;
    // }
    //
    //
    // protected override List<Vector3> RestrucutrePath(Vector3 startPos, Vector3 endPos)
    // {
    //     List<Vector3> tmpPath = base.RestrucutrePath(startPos, endPos);
    //     
    //     tmpPath.Insert(0, startPos);
    //
    //     return tmpPath;
    // }
    //
    public override Dictionary<Vector3, int> GetAvailableTilesInRange(Vector3 startPos, int range, 
        bool countHeroes, bool countEnemies)
    {
        Dictionary<Vector3, int> tmpDico = new Dictionary<Vector3, int>();
        Dictionary<Vector3, int> finalDico = new Dictionary<Vector3, int>();
    
        foreach (var pos in new Vector3[] { startPos, startPos + Vector3.right, startPos + Vector3.up, 
                     new Vector3(startPos.x + 1, startPos.y + 1, 0) })
        {
            tmpDico = base.GetAvailableTilesInRange(pos, range, countHeroes, countEnemies);
    
            foreach (var item in tmpDico)
            {
                if (!finalDico.ContainsKey(item.Key))
                {
                    finalDico.Add(item.Key, item.Value);
                }
            }
        }
    
        foreach (var tile in GetOccupiedTiles())
        {
            finalDico[tile.transform.position] = 0;
        }
        
        return finalDico;
    }
    //
    public override Dictionary<Vector3, int> GetTilesInAttackRange(Vector3 startPos, int range)
    {
        Dictionary<Vector3, int> dico = GetAvailableTilesInRange(startPos, 1, true, false);
    
        foreach (var tile in GetOccupiedTiles())
        {
            dico.Remove(tile.transform.position);
        }
    
        return dico;
    }
    //
    public override List<TileCell> GetOccupiedTiles()
    {
        return GetTilesInOccupiedRange(transform.position);
    }
    
    private List<TileCell> GetTilesInOccupiedRange(Vector3 startPos)
    {
        TileCell downLeft = GridManager.Instance.GetTileAtPosition(startPos);
        TileCell downRight = GridManager.Instance.GetTileAtPosition(new Vector3(startPos.x + 1, startPos.y, 0));
        TileCell upLeft = GridManager.Instance.GetTileAtPosition(new Vector3(startPos.x, startPos.y + 1, 0));
        TileCell upRight = GridManager.Instance.GetTileAtPosition(new Vector3(startPos.x + 1, startPos.y + 1, 0));
    
        if (downLeft && downRight && upLeft && upRight)
        {
            return new List<TileCell>
            {
                downLeft, downRight, upLeft, upRight
            };
        }
    
        return null;
    }
    //
    // public override void FindAvailablePathToTarget(Vector3 targetPos, int minimumPathCOunt, 
    //     bool countHeroes, bool countEnemies)
    // {
    //     // TODO foreach tile in GetOccupedTiles() -> if GetTileAtPostion(tile + neighbor).Walkable ||
    //     // TODO GetOccupedTiles.Contains(tile) -> alors ajoute au chemin.
    //     
    //     // TODO FAIRE QUE GETOCCUPEDTILES SOIT DANS TILESMANAGER ET PRENNE UN ARGUMENTT POUR LA STARTPOS ET LA RANGE !
    //
    //     _availableTiles = GetAvailableTilesInRange(transform.position, _movement.Value, 
    //         false, false);
    //
    //     _path = FindPath(transform.position, targetPos, true, false);
    //     
    //     // If the enemy is already near the player, there would be no path, so we don't need to move the enemy.
    //     if (_path.Count > 1)
    //     {
    //         _path.Remove(_path.Last());
    //         
    //         _avalaiblePath = _availableTiles.Keys.Intersect(_path).ToList();
    //     
    //         _targetPos = _avalaiblePath.First();
    //     }
    //     else
    //     {
    //         _targetPos = null;
    //         
    //         // The basic AI only focus player, if there is no path to the player, the AI won't move.
    //         _nbrOfMovementPerformed = _maxNbrOfMovementPerTurn;
    //     }
    // }
    //
    // protected override void FollowThePath()
    // {
    //     if (_currentTargetIndex < _avalaiblePath.Count - 1) 
    //     {
    //         _currentTargetIndex++;
    //
    //         var nextTile = _gridManager.GetTileAtPosition(_avalaiblePath[_currentTargetIndex]);
    //
    //         foreach (var tile in GetOccupiedTiles())
    //         {
    //             if (Vector3.Distance(tile.transform.position, nextTile.transform.position) < 0.1)
    //             {
    //                 StopThePath();
    //             }
    //         }
    //         
    //         if (nextTile.OccupiedUnit != null || GetOccupiedTiles().Contains(nextTile))
    //         {
    //             StopThePath();
    //         }
    //         else
    //         {
    //             _targetPos = _gridManager.WorldToCellCenter(_avalaiblePath[_currentTargetIndex]);
    //         }
    //     }
    //     
    //     else
    //     {
    //         StopThePath();
    //     }
    // }
}
