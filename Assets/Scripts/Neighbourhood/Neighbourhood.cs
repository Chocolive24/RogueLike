using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighbourhood
{
    public static List<Vector2> CardinalNeighbours => new List<Vector2>
    { 
        Vector2.up, Vector2.right, Vector2.left, Vector2.down
    };
}
