using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new Card", menuName = "Scriptable Card")]
public class ScriptableCard : ScriptableObject
{
    [SerializeField] private Rarety _rarety;
    [SerializeField] private BaseCard _baseCardPrefab;

    // Getters and Setters --------------------------------------------------------------------------
    public Rarety Rarety => _rarety;
    public BaseCard BaseCardPrefab => _baseCardPrefab;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
