using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;

public delegate void CardsLoaded(List<GameObject> cards);
public enum CardsManagerState {STARTED,LOADING}
public class CardsManager : MonoBehaviour
{
    [SerializeField] private GameObject cardsContainer;
    [SerializeField] private int comparedCards = 2;
    [SerializeField] private float restartCardsWaitTime = 0.5f;
    [SerializeField] private float startWaitTime = 0.5f;
    
    private Sprite[] _cardsSprites;
    private List<GameObject> _cardsGos;
    public List<GameObject> CardsGos => _cardsGos;
    private List<Tile> _cards;
    private List<Tile> _showingCardsIds;
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
        _showingCardsIds = new List<Tile>();
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
        EnableCardsInput(true);
    }

    private void EnableCardsInput(bool value)
    {
        foreach(var tile in _cards)
            tile.EnableInputs(value);
    }

    private void InitCards()
    {
         if (InitializedCards() && ValidateCards())
         {
             CardsGoShuffle();
             StartCoroutine(WaitThen(() => { OnCardsLoaded?.Invoke(_cardsGos); },startWaitTime));
             _state = CardsManagerState.STARTED;
         }
         else
             Debug.LogError("sprites or cards not valid");        
    }

    private bool InitializedCards()
    {
        for (int i = 0; i < cardsContainer.transform.childCount; i += 2)
        {
            int randomIndex = 0;
            do {
                randomIndex = GetRandomIndex(_cardsSprites.Length);
            } while (_spritesSelected.Contains(randomIndex));
            
            _spritesSelected.Add(randomIndex);
            for (int j = i; j <= i + 1; j++)
            {
                var child = cardsContainer.transform.GetChild(j);
                var tile = child.GetComponent<Tile>();
                if (tile != null)
                    InitTile(tile,child.gameObject,randomIndex);

                if(_cardSize.Equals(float2.zero))
                    _cardSize = tile.GetTileSize();
            }
        }
        return true;
    }
    
    private void InitTile(Tile tile, GameObject tileGo, int randomIndex)
    {
         tile.Init(_cardsSprites[randomIndex],randomIndex);
         _cards.Add(tile);
         _cardsGos.Add(tileGo);
         tileGo.gameObject.SetActive(true);
         tile.OnTurnAnimationFinished += Tile_OnTurnAnimationFinished;
    }
    
    private void Tile_OnTurnAnimationFinished(Tile tile)
    {
        if (tile.state == TileState.SHOWED)
        {
           _showingCardsIds.Add(tile);
           if (_showingCardsIds.Count.Equals(comparedCards))
           {
               EnableCardsInput(false); 
               CheckCards();
           }
        }
        else if (tile.state == TileState.HIDED && _showingCardsIds.Count > 0)
        {
            _showingCardsIds.RemoveAt(0);
            if(_showingCardsIds.Count == 0)
                EnableCardsInput(true);
        }
    }

    private void CheckCards()
    {
        bool sameIds = true;
        var firstCardId = _showingCardsIds[0];
        
        foreach(var cardId in _showingCardsIds)
            sameIds = cardId.ID == firstCardId.ID;
        
        if (!sameIds)
        {
            StartCoroutine(WaitThen(() =>
            {
                foreach (var cardId in _showingCardsIds)
                    cardId.Hide();
            },restartCardsWaitTime));
        }
        else
        {
            StartCoroutine(WaitThen( () =>
            {
                foreach (var cardId in _showingCardsIds)
                    cardId.Disable();
                _showingCardsIds.Clear();
                EnableCardsInput(true);
            },restartCardsWaitTime));
            CheckAllCards();
        }
    }

    private void CheckAllCards()
    {
        
    }

    private IEnumerator WaitThen(Action callback, float waitTime)
    {
       yield return new WaitForSeconds(waitTime);
       callback?.Invoke();
    }

    private bool ValidateCards()
    {
        return _cardsSprites.Length >= _cards.Count;
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

