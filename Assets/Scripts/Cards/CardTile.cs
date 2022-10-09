using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;


public delegate void TurnAnimationFinished(CardTile cardTile);
public enum TileState {SHOWED, HIDED, ANIMATING,DISABLED}
public enum TileInputState {ENABLE, DISABLE}

[RequireComponent(typeof(SpriteRenderer))]
public class CardTile : MonoBehaviour
{
    [SerializeField] private Sprite hiddenSprite;
    [SerializeField] private float animationDuration = 0.5f;
    
    //TODO: GAME DEFINITION FOR THE CARD ID CORRESPONDING TO THIS CARD
    private Sprite _originalSprite;
    private Vector3 _originalScale;
    private TileInputState _inputState;
    private SpriteRenderer _spriteRenderer;
    private CardsAnimator _cardsAnimator;
    private TileState _state;
    public TileState state => _state;
    private int _id;
    public int ID => _id;
    public TurnAnimationFinished OnTurnAnimationFinished;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalScale = transform.localScale;
        _inputState = TileInputState.DISABLE;
        DoAfterHideAnimation();
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
            Show();
    }

    private void Show()
    {
        if (_state == TileState.ANIMATING) return;
        if (_state == TileState.HIDED)
            StartCoroutine(TurnAnimation(true));
    }

    public void Hide()
    {
        if (_state == TileState.ANIMATING) return;
        if (_state == TileState.SHOWED)
            StartCoroutine(TurnAnimation(false));
    }

    public void Disable()
    {
        _state = TileState.DISABLED;
        gameObject.SetActive(false); 
    }

    private IEnumerator TurnAnimation(bool show)
    {
        _state = TileState.ANIMATING;

        yield return FadeIn();

        yield return null;
        if (show) DoAfterShowAnimation(); else DoAfterHideAnimation();

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

    private void DoAfterShowAnimation()
    {
        if(_originalSprite != null) _spriteRenderer.sprite = _originalSprite;
        _state = TileState.SHOWED;
        Debug.Log($"[{GetType()}]:: Card id: {_id} is showed");
        OnTurnAnimationFinished?.Invoke(this);
    }

    private void DoAfterHideAnimation()
    {
        if(hiddenSprite != null) _spriteRenderer.sprite = hiddenSprite;
        _state = TileState.HIDED;
        Debug.Log($"[{GetType()}]:: Card id: {_id} is hided");
        OnTurnAnimationFinished?.Invoke(this);
    }

    public float2 GetTileSize()
    {
        return new float2(_originalSprite.bounds.extents.x, _originalSprite.bounds.extents.y);
    }

}
