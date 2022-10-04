using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;


//TODO: CHANGE THIS CLASS TO A STATIC CLASS OR A NORMAL CLASS
//TODO: CREATE A DATA LOADER FOR THE SPRITES AND ITS REFERENCE, USE ADDRESSABLE ASSETS AND SCRIPTABLE OBJECTS 
public delegate void CardsLoaded(List<GameObject> cards);
public enum CardsManagerState {STARTED,LOADING}
public class CardsManager : MonoBehaviour
{
    [SerializeField] private GameObject cardsFirstRow;
    [SerializeField] private GameObject cardsSecondRow;
    [SerializeField] private Sprite[] cardsSprites;

    private List<GameObject> _cardsGos;
    public List<GameObject> CardsGos => _cardsGos;
    private List<Tile> _cards;
    private List<int> _spritesSelected;
    private CardsManagerState _state;
    public CardsManagerState State => _state;

    public CardsLoaded OnCardsLoaded;
    private CardsAnimator _cardsAnimator;

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
        InitCards();
    }

    private void InitReferences()
    {
         _cardsAnimator = GetComponent<CardsAnimator>();
         if(_cardsAnimator == null) 
             Debug.LogError($"[{GetType()}]:: _cardsAnimator is null");
         else
             _cardsAnimator.OnCardsAnimationFinish += OnCardsAnimationFinish; 
       
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
                randomIndex = GetRandomIndex(cardsSprites.Length);
            } while (_spritesSelected.Contains(randomIndex));
            
            _spritesSelected.Add(randomIndex);
            for (int j = 0; j < transform.childCount; j++)
            {
                var child = transform.GetChild(j).transform.GetChild(i);
                var tile = child.GetComponent<Tile>();
                if (tile != null)
                {
                    tile.SetSprite(cardsSprites[randomIndex]);
                    InitTile(tile,child.gameObject);
                }
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
        return cardsSprites.Length >= _cards.Count && 
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

