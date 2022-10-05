using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/CardsData", fileName = "cardsData", order = 0)]
public class CardsData : ScriptableObject
{
    [SerializeField] private Sprite[] sprites;
    public Sprite[] Sprites => sprites;
}
