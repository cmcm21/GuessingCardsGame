using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Data
{
    public delegate void CardsDataLoaded(CardsData cardsData);
    public delegate void LoadDataFail(string message);

    public class DataManager
    {
        private CardsData _cardsData;
        public CardsDataLoaded OnCardsDataLoaded;
        public LoadDataFail OnDataLoadFail;
        private AsyncOperationHandle<CardsData> _asyncCardsOperationHandle;

        public void Init(AssetReference cardsReference)
        {
            _asyncCardsOperationHandle = Addressables.LoadAssetAsync<CardsData>(cardsReference);
            _asyncCardsOperationHandle.Completed += AsyncCardsOperationHandleOnCompleted;
        }

        private void AsyncCardsOperationHandleOnCompleted(AsyncOperationHandle<CardsData> obj)
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                _cardsData = obj.Result;
                OnCardsDataLoaded?.Invoke(obj.Result); 
            }
            else 
            {
                OnDataLoadFail?.Invoke(obj.OperationException.Message); 
            }
        }

        public void DisposeCardsData()
        {
            Addressables.Release(_asyncCardsOperationHandle);
        }
    }
}