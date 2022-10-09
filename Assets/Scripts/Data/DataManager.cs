using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Data
{
    public delegate void CardsDataLoaded(CardsData cardsData);
    public delegate void LoadCardsDataFail(string message);

    public class DataManager
    {
        public CardsDataLoaded OnCardsDataLoaded;
        public LoadCardsDataFail OnCardsDataLoadCardsFail;
        private AsyncOperationHandle<GameplayData> _asyncCardsOperationHandle;

        public void Init(AssetReference gameplayDataReference)
        {
            _asyncCardsOperationHandle = Addressables.LoadAssetAsync<GameplayData>(gameplayDataReference);
            _asyncCardsOperationHandle.Completed += AsyncCardsOperationHandleOnCompleted;
        }

        private void AsyncCardsOperationHandleOnCompleted(AsyncOperationHandle<GameplayData> obj)
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                OnCardsDataLoaded?.Invoke(obj.Result.cardsData); 
                AudioManager.Init(obj.Result.audioData);
            }
            else 
                OnCardsDataLoadCardsFail?.Invoke(obj.OperationException.Message); 
        }

        public void DisposeCardsData()
        {
            Addressables.Release(_asyncCardsOperationHandle);
        }
    }
}