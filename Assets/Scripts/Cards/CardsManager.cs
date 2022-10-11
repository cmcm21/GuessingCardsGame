using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;

public delegate void CardsLoaded(List<GameObject> cards);
public delegate void CardsMatch();
public delegate void CardsMismatch();
public delegate void AllCardsMatched();

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
    private List<CardTile> _cards;
    private List<CardTile> _showingCardsIds;
    private List<int> _spritesSelected;
    private CardsManagerState _state;
    public CardsManagerState State => _state;

    public CardsLoaded OnCardsLoaded;
    public AllCardsMatched OnAllCardsMatched;
    public CardsMismatch OnCardsMismatch;
    public CardsMatch OnCardsMatch;
    public CardsAnimationFinish OnCardsAnimationFinished;
    
    private CardsAnimator _cardsAnimator;
    private float2 _cardSize = float2.zero;
    public float2 CardSize => _cardSize;

    private void Awake()
    {
        _cards = new List<CardTile>();
        _cardsGos = new List<GameObject>();
        _spritesSelected = new List<int>();
        _state = CardsManagerState.LOADING;
        _showingCardsIds = new List<CardTile>();
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
             _cardsAnimator.OnCardsAnimationFinish += CardsAnimator_OnCardsAnimationFinish;

         var gameManager = FindObjectOfType<GameManager>();
         if(gameManager == null)
             Debug.LogError($"[{GetType()}]:: game manager is null");
         else
             gameManager.OnCardsCardsDataLoaded += GameManager_OnCardsCardsDataLoaded;
    }

    private void GameManager_OnCardsCardsDataLoaded(CardsData cardsData)
    {
        _cardsSprites = cardsData.Sprites;
        InitCards();
    }

    private void CardsAnimator_OnCardsAnimationFinish()
    {
        EnableCardsInput(true);
        OnCardsAnimationFinished?.Invoke();
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
                var tile = child.GetComponent<CardTile>();
                if (tile != null)
                    InitTile(tile,child.gameObject,randomIndex);

                if(_cardSize.Equals(float2.zero))
                    _cardSize = tile.GetTileSize();
            }
        }
        return true;
    }
    
    private void InitTile(CardTile cardTile, GameObject tileGo, int randomIndex)
    {
         cardTile.Init(_cardsSprites[randomIndex],randomIndex);
         _cards.Add(cardTile);
         _cardsGos.Add(tileGo);
         tileGo.gameObject.SetActive(true);
         cardTile.OnTurnAnimationFinished += Tile_OnTurnAnimationFinished;
    }
    
    private void Tile_OnTurnAnimationFinished(CardTile cardTile)
    {
        if (cardTile.state == TileState.SHOWED)
        {
           _showingCardsIds.Add(cardTile);
           if (_showingCardsIds.Count.Equals(comparedCards))
           {
               EnableCardsInput(false); 
               CheckCards();
           }
        }
        else if (cardTile.state == TileState.HIDED && _showingCardsIds.Count > 0)
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
            DoOnCardsMismatch();
        else
            DoOnCardsMatch();
    }

    private void DoOnCardsMismatch()
    {
        StartCoroutine(WaitThen(() =>
        {
            foreach (var cardId in _showingCardsIds)
                cardId.Hide();
            
            AudioManager.PlaySfx(ClipId.SFX_CARDS_MISMATCH); 
            OnCardsMismatch?.Invoke(); 
            
        },restartCardsWaitTime));       
    }

    private void DoOnCardsMatch()
    {
        StartCoroutine(WaitThen( () =>
        {
            foreach (var cardId in _showingCardsIds)
                cardId.Disable();
            
            _showingCardsIds.Clear();
            EnableCardsInput(true);
            
            AudioManager.PlaySfx(ClipId.SFX_CARDS_MATCH);
            CheckAllCards();       
            OnCardsMatch?.Invoke(); 
            
        },restartCardsWaitTime));
    }
    
    

    private void CheckAllCards()
    {
        int counter = 0;
        foreach(var card in _cards)
            if (card.state == TileState.DISABLED)
                counter++;

        Debug.Log($"[{GetType()}]:: cards disabled: {counter}");
        if (counter == _cards.Count)
            StartCoroutine(WaitThen(() => OnAllCardsMatched?.Invoke(), startWaitTime));
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

