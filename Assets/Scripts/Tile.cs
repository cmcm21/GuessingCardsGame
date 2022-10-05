using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TileState {SHOWED, HIDED}
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
    private int _id;
    public int ID => _id;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _inputState = TileInputState.DISABLE;
        Hide();
    }
    public void Init(Sprite sprite, int id)
    {
        _originalSprite = sprite;
        _id = id;
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
        if (_state == TileState.HIDED)
            Show();
        else
            Hide();
    }

    private void Show()
    {
        if(_originalSprite != null) _spriteRenderer.sprite = _originalSprite;
        _state = TileState.SHOWED;
        Debug.Log($"[{GetType()}]:: Card id: {_id} is showed");
    }

    private void Hide()
    {
        if(hiddenSprite != null) _spriteRenderer.sprite = hiddenSprite;
        _state = TileState.HIDED;
        Debug.Log($"[{GetType()}]:: Card id: {_id} is hided");
    }


}
