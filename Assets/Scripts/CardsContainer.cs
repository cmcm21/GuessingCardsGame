using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsContainer : MonoBehaviour
{
    [Header("Cards")] 
    [SerializeField] private GameObject[] firstRowCards;
    [SerializeField] private GameObject[] secondRowCards; 
    
    [Space]
    [Header("Cards positions info")]
    [SerializeField] private float startXPosition;
    [SerializeField] private float firstRowYPosition;
    [SerializeField] private float secondRowYPosition;
    [SerializeField] private float xSpaceBetweenCards;

    [Space] 
    [Header("Starting Animation info")] 
    [SerializeField] private float animationTime;

    private List<Vector3> _firstRowCardPositions;
    private List<Vector3> _secondRowCardPositions;

    private void Awake()
    {
        _firstRowCardPositions = new List<Vector3>();
        _secondRowCardPositions = new List<Vector3>();
    }

    private void Start()
    {
       InitializePositions(); 
    }

    private void InitializePositions()
    {
        for (int i = 0; i < firstRowCards.Length; i++)
        {
            var position = new Vector3(startXPosition + i * xSpaceBetweenCards, firstRowYPosition, 0);
            firstRowCards[i].transform.position = position;
            _firstRowCardPositions.Add(position);
        }

        for (int i = 0; i < secondRowCards.Length; i++)
        {
            var position = new Vector3(startXPosition + i * xSpaceBetweenCards, secondRowYPosition, 0);
            secondRowCards[i].transform.position = position;
            _secondRowCardPositions.Add(position);
        }
    }
}
