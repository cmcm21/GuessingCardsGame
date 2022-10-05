using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AssetReference cardsReference;
    private DataManager _dataManager;
    public CardsDataLoaded OnCardsCardsDataLoaded;

    private void Awake()
    {
        _dataManager = new DataManager();
        _dataManager.OnCardsDataLoaded += DataManager_OnDataLoaded;
        _dataManager.OnDataLoadFail += DataManager_OnDataLoadFail;
    }

    private void Start()
    {
        _dataManager.Init(cardsReference);
    }

    private void DataManager_OnDataLoadFail(string message)
    {
        Debug.LogError($"[{GetType()}]:: Loading cards data error : {message}");
    }

    private void DataManager_OnDataLoaded(CardsData cardsdata)
    {
        OnCardsCardsDataLoaded?.Invoke(cardsdata);
    }
}
