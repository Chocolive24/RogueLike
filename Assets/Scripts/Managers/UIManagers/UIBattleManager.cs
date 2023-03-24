using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class UIBattleManager : MonoBehaviour
{
    private static UIBattleManager _instance;
    public static UIBattleManager Instance { get { return _instance; } }

    [SerializeField] private GameObject _battlePanel;
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private TextMeshProUGUI _currentTurnTxt;
    //[SerializeField] private Image _manaContainer;
    [SerializeField] private TextMeshProUGUI _manaNbrTxt;
    [SerializeField] private GameObject _selectedHeroObject, _tileObject, _tileUnitObject;

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
        
        _battlePanel.SetActive(false);
        
        GameManager.OnGameStateChange += GameManagerOnOnGameStateChange;
        BattleManager.OnBattleStateChange += BattleManagerOnOnBattleStateChange;
    }

    private void GameManagerOnOnGameStateChange(GameState gameState)
    {
        _battlePanel.SetActive(gameState == GameState.BATTLE);
        Debug.Log(gameState);
    }
    
    private void BattleManagerOnOnBattleStateChange(BattleState battleState)
    {
        _endTurnButton.interactable = battleState == BattleState.HEROES_TURN;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameManagerOnOnGameStateChange;
        BattleManager.OnBattleStateChange -= BattleManagerOnOnBattleStateChange;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _currentTurnTxt.text = BattleManager.Instance.State.ToString();
        
        if (UnitsManager.Instance.SelectedHero)
        {
            _manaNbrTxt.text = UnitsManager.Instance.SelectedHero.CurrentMana.ToString() + " / " +
                               UnitsManager.Instance.SelectedHero.MaxMana.ToString();
        }
        
    }

    public void EndTheTurn()
    {
        //GameManager.Instance.UpdateGameState(GameState.EnemiesTurn);
        BattleManager.Instance.UpdateBattleState(BattleState.ENEMIES_TURN);
    }
    
    public void ShowSelectedHero(BaseHero hero)
    {
        if (hero == null)
        {
            _selectedHeroObject.SetActive(false);
            return;
        }
        
        _selectedHeroObject.GetComponentInChildren<TextMeshProUGUI>().text = hero.UnitName;
        _selectedHeroObject.SetActive(true);
        
        
    }

    public void ShowTileInfo(TileCell tile)
    {
        if (tile == null)
        {
            _tileObject.SetActive(false);
            _tileUnitObject.SetActive(false);
            return;
        }
        
        _tileObject.GetComponentInChildren<TextMeshProUGUI>().text = tile.Name;
        _tileObject.SetActive(true);

        if (tile.OccupiedUnit)
        {
            string tileUnitObjTxt;
            tileUnitObjTxt = tile.OccupiedUnit.UnitName;

            if (tile.OccupiedUnit.GetComponent<BaseEnemy>())
            {
                tileUnitObjTxt += "\n dist. " + tile.OccupiedUnit.GetComponent<BaseEnemy>().CalculateDistanceFromThePlayer();
            }

            _tileUnitObject.GetComponentInChildren<TextMeshProUGUI>().text = tileUnitObjTxt;
            
            _tileUnitObject.SetActive(true);
        }
    }
}
