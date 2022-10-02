using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TileState {SHOWED, HIDDED}

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    [SerializeField] private Sprite hiddenSprite; 
    
    private Sprite _originalSprite;
    private TileState _state;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Hide();
    }
    
    private void OnMouseDown()
    {
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
