using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Data
{
    public delegate void DataLoaded();
    public delegate void CardsDataLoaded(CardsData cardsData);
    public delegate void LoadCardsDataFail(string message);

    public static class DataManager
    {
        public static CardsDataLoaded OnCardsDataLoaded;
        public static LoadCardsDataFail OnCardsDataLoadCardsFail;
        public static DataLoaded OnDataLoaded;
        private static AsyncOperationHandle<GameplayData> _asyncCardsOperationHandle;

        public static void Init(AssetReference gameplayDataReference)
        {
            _asyncCardsOperationHandle = Addressables.LoadAssetAsync<GameplayData>(gameplayDataReference);
            _asyncCardsOperationHandle.Completed += AsyncCardsOperationHandleOnCompleted;
        }

        private static void AsyncCardsOperationHandleOnCompleted(AsyncOperationHandle<GameplayData> obj)
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                OnCardsDataLoaded?.Invoke(obj.Result.cardsData); 
                OnDataLoaded?.Invoke();
                AudioManager.Init(obj.Result.audioData);
            }
            else 
                OnCardsDataLoadCardsFail?.Invoke(obj.OperationException.Message); 
        }

        public static void DisposeData()
        {
            Addressables.Release(_asyncCardsOperationHandle);
        }
    }
}