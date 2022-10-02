using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;

public class CardsManager : MonoBehaviour
{
    [SerializeField] private GameObject cardsContainer;
    [SerializeField] private GameObject cardsFirstRow;
    [SerializeField] private GameObject cardsSecondRow;
    [SerializeField] private Sprite[] cardsSprites;

    private List<GameObject> _cardsGos;
    private List<Tile> _cards;
    private List<int> _spritesSelected;

    private void Awake()
    {
        _cards = new List<Tile>();
        _cardsGos = new List<GameObject>();
        _spritesSelected = new List<int>();
    }

    private void Start()
    {
        if (InitializedCards() && ValidateCards())
        {
           
        }
        else
            Debug.LogError("sprites or cards not valid"); 
    }

    private bool InitializedCards()
    {
        if (cardsFirstRow.transform.childCount != cardsSecondRow.transform.childCount) return false;
        for (int i = 0; i < cardsFirstRow.transform.childCount; i++)
        {
            var randomIndex = GetRandomIndex(cardsSprites.Length);
            _spritesSelected.Add(randomIndex);
            for (int j = 0; j < cardsContainer.transform.childCount; j++)
            {
                var child = cardsContainer.transform.GetChild(j).transform.GetChild(i);
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
}

