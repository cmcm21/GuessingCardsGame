using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public delegate void CardsAnimationFinish();

public class CardsAnimator : MonoBehaviour
{
    
    [Space]
    [Header("Cards positions info")]
    [SerializeField] private float startXPosition;
    [SerializeField] private float startYPosition;
    [SerializeField] private float ySpaceBetweenCards; 
    [SerializeField] private float xSpaceBetweenCards;
    [SerializeField] private int rows;

    [Space] 
    [Header("Starting Animation info")] 
    [SerializeField] private float animationCardToTargetTime = 0.725f;
    [SerializeField] private bool animate = true;

    private List<Vector3> _cardsPositions;
    private List<GameObject> _cards;
    private CardsManager _cardsManager;
    public CardsAnimationFinish OnCardsAnimationFinish;

    private void Awake()
    {
        _cardsPositions = new List<Vector3>();
        _cards = new List<GameObject>();
    }

    private void Start()
    {
        _cardsManager = GetComponent<CardsManager>();
        if(_cardsManager != null)
            InitCardsAnimation();
        else
            Debug.LogError($"[{GetType()}]:: Error: cardsManager is Null");
    }

    private void InitCardsAnimation()
    {
        if(_cardsManager.State == CardsManagerState.STARTED)
            OnCardsLoaded(_cardsManager.CardsGos);
        else
            _cardsManager.OnCardsLoaded += OnCardsLoaded;
    }

    private void OnCardsLoaded(List<GameObject> cards) 
    {
        _cards = cards;
        InitializePositions();
        if(animate)
            StartCoroutine(AnimateCards());
        else
            SetPositions();
    }

    private void InitializePositions()
    {
        var cardSizeX = _cardsManager.CardSize.x / 2;
        var cardSizeY = _cardsManager.CardSize.y / 2;
        for (int rowNum = 0; rowNum < rows; rowNum++)
        {
            for (int i = 0; i < _cards.Count / rows; i++)
            {
                 var position = new Vector3(
                     startXPosition + i * xSpaceBetweenCards + cardSizeX * i, 
                     startYPosition - rowNum * ySpaceBetweenCards - rowNum * cardSizeY,
                     0);
                 _cardsPositions.Add(position);               
            }
        }
    }

    private void SetPositions()
    {
        for (int i = 0; i < _cards.Count; i++)
            _cards[i].transform.position = _cardsPositions[i];
        OnCardsAnimationFinish?.Invoke();
    }

    private IEnumerator AnimateCards()
    {
        yield return new WaitForSeconds(animationCardToTargetTime);

        for (int i = 0; i < _cards.Count; i++)
            yield return AnimateCardToPosition(_cards[i], _cardsPositions[i]);

        OnCardsAnimationFinish?.Invoke();
    }

    private IEnumerator AnimateCardToPosition(GameObject card, Vector3 targetPosition)
    {
        float timer = 0f;
        while(timer < animationCardToTargetTime)
        {
            card.transform.position = Vector3.Lerp(card.transform.position, targetPosition, timer / animationCardToTargetTime);
            timer += Time.deltaTime;
            yield return null;
        }
        card.transform.position = targetPosition;
        yield return null;
    }

}
