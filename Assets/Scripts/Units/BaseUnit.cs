using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    [SerializeField] private string _unitName;
    public string UnitName => _unitName;
    
    private TileCell _occupiedTile;
    public TileCell OccupiedTile { get => _occupiedTile; set => _occupiedTile = value; }

    [SerializeField] private Faction _faction;
    public Faction Faction => _faction;
    
    
    [SerializeField] protected int _hp;
    [SerializeField] protected int _attack;
    [SerializeField] protected int _shield;
    [SerializeField] protected int _movement;

    public int Movement
    {
        get => _movement;
        set => _movement = value;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
