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
    public Faction Faction => _faction;

    // Getters and Setters -----------------------------------------------------------------------------
    
    [SerializeField] private BaseUnit _baseUnitPrefab;
    public BaseUnit BaseUnitPrefab => _baseUnitPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
