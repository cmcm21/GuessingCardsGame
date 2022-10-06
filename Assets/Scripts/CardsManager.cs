using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;

//TODO: REFACTOR THIS CODE IN ORDER TO STOP TO USE CARDS FIRST ROW AND CARDS SECOND ROW,
//TODO: AND ONLY USE ONE OBJECT CONTAINER 
public delegate void CardsLoaded(List<GameObject> cards);
public enum CardsManagerState {STARTED,LOADING}
public class CardsManager : MonoBehaviour
{
    [SerializeField] private GameObject cardsFirstRow;
    [SerializeField] private GameObject cardsSecondRow;
    
    private Sprite[] _cardsSprites;
    private List<GameObject> _cardsGos;
    public List<GameObject> CardsGos => _cardsGos;
    private List<Tile> _cards;
    private List<int> _spritesSelected;
    private CardsManagerState _state;
    public CardsManagerState State => _state;

    public CardsLoaded OnCardsLoaded;
    private CardsAnimator _cardsAnimator;
    private float2 _cardSize = float2.zero;
    public float2 CardSize => _cardSize;

    private void Awake()
    {
        _cards = new List<Tile>();
        _cardsGos = new List<GameObject>();
        _spritesSelected = new List<int>();
        _state = CardsManagerState.LOADING;
    }

    private void Start()
    {
        InitReferences();
    }

    private void InitReferences()
    {
         _cardsAnimator = GetComponent<CardsAnimator>();
         if(_cardsAnimator == null) 
             Debug.LogError($"[{GetType()}]:: _cardsAnimator is null");
         else
             _cardsAnimator.OnCardsAnimationFinish += OnCardsAnimationFinish;

         var gameManager = FindObjectOfType<GameManager>();
         if(gameManager == null)
             Debug.LogError($"[{GetType()}]:: game manager is null");
         else
             gameManager.OnCardsCardsDataLoaded += OnCardsCardsDataLoaded;
    }

    private void OnCardsCardsDataLoaded(CardsData cardsData)
    {
        _cardsSprites = cardsData.Sprites;
        InitCards();
    }

    private void OnCardsAnimationFinish()
    {
        foreach(var tile in _cards)
            tile.EnableInputs(true);
    }

    private void InitCards()
    {
         if (InitializedCards() && ValidateCards())
         {
             CardsGoShuffle();
             OnCardsLoaded?.Invoke(_cardsGos);
             _state = CardsManagerState.STARTED;
         }
         else
             Debug.LogError("sprites or cards not valid");        
    }

    private bool InitializedCards()
    {
        if (cardsFirstRow.transform.childCount != cardsSecondRow.transform.childCount) return false;
        for (int i = 0; i < cardsFirstRow.transform.childCount; i++)
        {
            int randomIndex = 0;
            do {
                randomIndex = GetRandomIndex(_cardsSprites.Length);
            } while (_spritesSelected.Contains(randomIndex));
            
            _spritesSelected.Add(randomIndex);
            for (int j = 0; j < transform.childCount; j++)
            {
                var child = transform.GetChild(j).transform.GetChild(i);
                var tile = child.GetComponent<Tile>();
                if (tile != null)
                {
                    tile.Init(_cardsSprites[randomIndex],randomIndex);
                    InitTile(tile,child.gameObject);
                }

                if(_cardSize.Equals(float2.zero))
                    _cardSize = tile.GetTileSize();
            }
        }
        return true;
    }

    private void InitTile(Tile tile, GameObject tileGo)
    {
         if (tile != null)
         {
             _cards.Add(tile);
             _cardsGos.Add(tileGo);
             tileGo.gameObject.SetActive(true);
         }          
    }

    private bool ValidateCards()
    {
        return _cardsSprites.Length >= _cards.Count && 
               _spritesSelected.Count == cardsSecondRow.transform.childCount;
    }

    private int GetRandomIndex(int max)
    {
        return UnityRandom.Range(0, max);
    }

    private void CardsGoShuffle()
    {
        var random = new Random();
        _cardsGos = _cardsGos.OrderBy(obj => random.Next()).ToList();
    }
}

