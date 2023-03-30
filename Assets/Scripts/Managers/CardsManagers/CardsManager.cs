using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardsManager : MonoBehaviour
{
    private static CardsManager _instance;
    public static CardsManager Instance => _instance;

    private List<ScriptableCard> _scrBasicMoveCards;
    private List<ScriptableCard> _scrBasicAttackCards;
    private List<ScriptableCard> _scrBasicDefenseCards;
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public List<ScriptableCard> ScrBasicMoveCards => _scrBasicMoveCards;
    public List<ScriptableCard> ScrBasicAttackCards => _scrBasicAttackCards;
    public List<ScriptableCard> ScrBasicDefenseCards => _scrBasicDefenseCards;

    // Methods ---------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        
        _scrBasicMoveCards = Resources.LoadAll<ScriptableCard>("Cards/MoveCards/BasicMoveCards").ToList();
        _scrBasicAttackCards = Resources.LoadAll<ScriptableCard>(
            "Cards/AttackCards/PaladinCards/BasicAttackCards").ToList();
    }
    
    public BaseCard InstantiateCard(List<ScriptableCard> scriptableCards, Rarety rarety)
    {
        var randomPrefab = GetRandomCard<BaseCard>(scriptableCards, rarety);
        var spawnedCard = Instantiate(randomPrefab);

        return spawnedCard;
    }
    
    private T GetRandomCard<T>(List<ScriptableCard> scriptableCards, Rarety rarety) where T : BaseCard
    {
        return (T)scriptableCards.Where(u => u.Rarety == rarety).OrderBy
            (o => Random.value).First().BaseCardPrefab;
    }
}
