using Data;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI matchTxt;
    [SerializeField] private TextMeshProUGUI mismatchTxt;
    [SerializeField] private TextMeshProUGUI triesTxt;

    private static string MismatchBase = "Mismatch : {0}";
    private static string MatchBase = "Match : {0}";
    private static string TriesBase = "Tries : {0}";

    private int _mismatchCount;
    private int _matchCount;

    private void Awake()
    {
        _mismatchCount = 0;
        _matchCount = 0;
    }

    private void Start()
    {
        var cardManager = FindObjectOfType<CardsManager>();
        if (cardManager == null)
        {
            Debug.LogError($"[{GetType()}]:: Error: CardManager is null");
        }
        else
        {
            cardManager.OnCardsMatch += CardManager_OnCardsMatch;            
            cardManager.OnCardsMismatch += CardManager_OnCardsMismatch;
            cardManager.OnCardsAnimationFinished += CardManager_OnCardsAnimationFinished;
        }
    }

    private void CardManager_OnCardsAnimationFinished()
    {
        UpdateTexts();
    }

    private void CardManager_OnCardsMismatch()
    {
        _mismatchCount++;
        UpdateTexts();
    }

    private void CardManager_OnCardsMatch()
    {
        _matchCount++;
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        matchTxt.text = string.Format(MatchBase, _matchCount);
        mismatchTxt.text = string.Format(MismatchBase, _mismatchCount);
        triesTxt.text = string.Format(TriesBase,_matchCount + _mismatchCount);
    }
}
