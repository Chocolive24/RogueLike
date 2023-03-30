using System;using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction
{
    Hero = 0,
    Enemy,
}

[CreateAssetMenu(fileName = "New unit", menuName = "ScriptableUnit")]
public class ScriptableUnit : ScriptableObject
{
    [SerializeField] private Faction _faction;
    
    [SerializeField] private BaseUnit _baseUnitPrefab;

    // Getters and Setters -----------------------------------------------------------------------------
    public Faction Faction => _faction;
    public BaseUnit BaseUnitPrefab => _baseUnitPrefab;
}
