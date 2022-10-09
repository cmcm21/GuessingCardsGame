using Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AssetReference gameplayDataReference;
    private DataManager _dataManager;
    public CardsDataLoaded OnCardsCardsDataLoaded;

    private void Awake()
    {
        _dataManager = new DataManager();
        _dataManager.OnCardsDataLoaded += DataManager_OnDataLoaded;
        _dataManager.OnCardsDataLoadCardsFail += DataManager_OnDataLoadFail;
    }

    private void Start()
    {
        _dataManager.Init(gameplayDataReference);
        var cardManager = FindObjectOfType<CardsManager>();
        if(cardManager != null)
            cardManager.OnAllCardsMatched += CardManager_OnAllCardsMatched;
        else
            Debug.LogError($"[{GetType()}]::CardManager is null");
    }

    private void CardManager_OnAllCardsMatched()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
