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
    // Singleton -------------------------------------------------------------------------------------------------------
    #region Singleton

    private static UIBattleManager _instance;
    public static UIBattleManager Instance { get { return _instance; } }

    #endregion
    
    // References ------------------------------------------------------------------------------------------------------
    #region UIGameobjects
    [SerializeField] private GameObject _battlePanel;
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private TextMeshProUGUI _currentTurnTxt;
    [SerializeField] private TextMeshProUGUI _notEnoughManaTxt;
    [SerializeField] private TextMeshProUGUI _manaNbrTxt;
    [SerializeField] private GameObject _selectedHeroObject, _tileObject, _tileUnitObject;
    #endregion

    #region Managers

    private BattleManager _battleManager;
    private CardPlayedManager _cardPlayedManager;
    private UnitsManager _unitsManager;

    #endregion
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    #region Getters and Setters

    public GameObject BattlePanel { get => _battlePanel; set => _battlePanel = value; }

    public Button EndTurnButton => _endTurnButton;

    #endregion
    
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
        
        _battlePanel.SetActive(false);
        _notEnoughManaTxt.gameObject.SetActive(false);
    }
    
    
    private void OnDestroy()
    {
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _battleManager = BattleManager.Instance;
        _cardPlayedManager = CardPlayedManager.Instance;
        _unitsManager = UnitsManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (_unitsManager.SelectedHero)
        {
            if (!_cardPlayedManager.HasACardOnIt)
            {
                _manaNbrTxt.text = _unitsManager.SelectedHero.CurrentMana.ToString() + " / " +
                                   _unitsManager.SelectedHero.MaxMana.ToString();
            }
            else
            {
                _manaNbrTxt.text = _unitsManager.SelectedHero.CurrentMana.ToString() + " / " +
                                   _unitsManager.SelectedHero.MaxMana.ToString();
            }
        }
    }

    public void EndTheTurn()
    {
        _battleManager.IsPlayerTurn = false;
    }
    
    public void ShowSelectedHero(BaseHero hero)
    {
        if (!hero)
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
                tileUnitObjTxt += "\n dist. " + 
                                  tile.OccupiedUnit.GetComponent<BaseEnemy>().CalculateDistanceFromThePlayer();
            }

            _tileUnitObject.GetComponentInChildren<TextMeshProUGUI>().text = tileUnitObjTxt;
            
            _tileUnitObject.SetActive(true);
        }
    }

    public IEnumerator NotEnoughManaCo()
    {
        _notEnoughManaTxt.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        
        _notEnoughManaTxt.gameObject.SetActive(false);
    }
}
