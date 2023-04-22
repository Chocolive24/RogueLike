using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighbourhood
{
    public enum Direction
    {
        UP,
        RIGHT,
        LEFT,
        DOWN,
        NULL
    }
    
    public static Dictionary<Direction, Vector2> CardinalNeighbours => new Dictionary<Direction, Vector2>()
    { 
        {Direction.UP, Vector2.up},
        {Direction.RIGHT, Vector2.right},
        {Direction.LEFT, Vector2.left},
        {Direction.DOWN, Vector2.down},
    };
}
