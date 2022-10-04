using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TileState {SHOWED, HIDDED}
public enum TileInputState {ENABLE, DISABLE}

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    [SerializeField] private Sprite hiddenSprite; 
    
    //TODO: GAME DEFINITION FOR THE CARD ID CORRESPONDING TO THIS CARD
    private Sprite _originalSprite;
    private TileState _state;
    private TileInputState _inputState;
    private SpriteRenderer _spriteRenderer;
    private CardsAnimator _cardsAnimator;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _inputState = TileInputState.DISABLE;
        Hide();
    }

    public void EnableInputs(bool enable)
    {
        _inputState = enable ? TileInputState.ENABLE : TileInputState.DISABLE;
    }

    private void OnMouseDown()
    {
        if(_inputState == TileInputState.ENABLE)
            Toggle();
    }

    private void Toggle()
    {
        if (_state == TileState.HIDDED)
            Show();
        else
            Hide();
    }

    private void Show()
    {
        if(_originalSprite != null) _spriteRenderer.sprite = _originalSprite;
        _state = TileState.SHOWED;
    }

    private void Hide()
    {
        if(hiddenSprite != null) _spriteRenderer.sprite = hiddenSprite;
        _state = TileState.HIDDED;
    }

    public void SetSprite(Sprite sprite)
    {
        _originalSprite = sprite;
    }
}
