using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CardsAnimationFinish();

public class CardsAnimator : MonoBehaviour
{
    
    [Space]
    [Header("Cards positions info")]
    [SerializeField] private float startXPosition;
    [SerializeField] private float firstRowYPosition;
    [SerializeField] private float secondRowYPosition;
    [SerializeField] private float xSpaceBetweenCards;

    [Space] 
    [Header("Starting Animation info")] 
    [SerializeField] private float animationCardToTargetTime = 0.725f;

    private List<Vector3> _cardsPositions;
    private List<GameObject> _cards;

    public CardsAnimationFinish OnCardsAnimationFinish;

    private void Awake()
    {
        _cardsPositions = new List<Vector3>();
        _cards = new List<GameObject>();
    }

    private void Start()
    {
        var cardsManager = FindObjectOfType<CardsManager>();
        if(cardsManager != null)
            cardsManager.OnCardsLoaded += OnCardsLoaded;
    }

    private void OnCardsLoaded(List<GameObject> cards)
    {
        _cards = cards;
        InitializePositions();
        StartCoroutine(AnimateCards());
    }

    private void InitializePositions()
    {
        for (int i = 0; i < _cards.Count / 2; i++)
        {
            var position = new Vector3(startXPosition + i * xSpaceBetweenCards, firstRowYPosition, 0);
            _cardsPositions.Add(position);
        }

        for (int i = 0; i < _cards.Count / 2; i++)
        {
            var position = new Vector3(startXPosition + i * xSpaceBetweenCards, secondRowYPosition, 0);
            _cardsPositions.Add(position);
        }
    }

    private IEnumerator AnimateCards()
    {
        yield return new WaitForSeconds(animationCardToTargetTime);

        int i = 0;
        for (; i < _cards.Count / 2; i++)
            yield return AnimateCardToPosition(_cards[i], _cardsPositions[i]);
        
        yield return null;

        for (; i < _cards.Count; i++) 
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
