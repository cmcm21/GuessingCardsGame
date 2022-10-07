using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;


public delegate void TurnAnimationFinished();
public enum TileState {SHOWED, HIDED, ANIMATING}
public enum TileInputState {ENABLE, DISABLE}

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    [SerializeField] private Sprite hiddenSprite;
    [SerializeField] private float animationDuration = 0.5f;
    
    //TODO: GAME DEFINITION FOR THE CARD ID CORRESPONDING TO THIS CARD
    private Sprite _originalSprite;
    private Vector3 _originalScale;
    private TileState _state;
    private TileInputState _inputState;
    private SpriteRenderer _spriteRenderer;
    private CardsAnimator _cardsAnimator;
    private int _id;
    public int ID => _id;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalScale = transform.localScale;
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
        if (_state == TileState.ANIMATING) return;
        if (_state == TileState.HIDED)
            StartCoroutine(TurnAnimation(true));
        else
            StartCoroutine(TurnAnimation(false));
    }

    private IEnumerator TurnAnimation(bool show)
    {
        _state = TileState.ANIMATING;

        yield return FadeIn();

        yield return null;
        if (show) Show(); else Hide();

        yield return FadeOut();

        _state = show ? TileState.SHOWED : TileState.HIDED;
    }

    private IEnumerator FadeIn()
    {
         float time = 0f;
         var localScale = transform.localScale;
         //fade in animation
         while (time <= animationDuration)
         {
             time += Time.deltaTime;
             var xScale = Mathf.Lerp(localScale.x, 0f, time / animationDuration);
             transform.localScale = new Vector3(xScale, localScale.y, localScale.z);
             yield return null;
         }

         transform.localScale = new Vector3(0, localScale.y, localScale.z);
         yield return null;
    }

    private IEnumerator FadeOut()
    {

        float time = 0;
        var localScale = transform.localScale;
        while (time <= animationDuration)
        {
            time += Time.deltaTime;
            var xScale = Mathf.Lerp(localScale.x, _originalScale.x, time / animationDuration);
            transform.localScale = new Vector3(xScale, localScale.y, localScale.z);
            yield return null;
        }

        transform.localScale = new Vector3(_originalScale.x, localScale.y, localScale.z);
        yield return null;
    }

    private void Show()
    {
        if(_originalSprite != null) _spriteRenderer.sprite = _originalSprite;
        Debug.Log($"[{GetType()}]:: Card id: {_id} is showed");
    }

    private void Hide()
    {
        if(hiddenSprite != null) _spriteRenderer.sprite = hiddenSprite;
        _state = TileState.HIDED;
        Debug.Log($"[{GetType()}]:: Card id: {_id} is hided");
    }

    public float2 GetTileSize()
    {
        return new float2(_originalSprite.bounds.extents.x, _originalSprite.bounds.extents.y);
    }

}
