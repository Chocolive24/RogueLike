using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    
    public Enemy(int hp, int attack, int shield) : base(hp, attack, shield)
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        DisplayStats();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void DisplayStats()
    {
        Debug.Log("Enemy's stats : ");
        base.DisplayStats();
    }
}
