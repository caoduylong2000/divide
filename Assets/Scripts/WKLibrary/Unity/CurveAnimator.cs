using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WK {

public class CurveAnimator : MonoBehaviour {
	public enum EStyle
	{
		Once,
		Loop,
    }

    public bool isReverse = false;

    protected Transform      cachedTransform;
    protected Image          image;
    protected Text           text;
    protected SpriteRenderer sprite;
    protected CanvasGroup    canvasGroup;
    protected float          counter       = 0.0f;
	protected Coroutine      playCoroutine = null;

	public EStyle style         = EStyle.Once;
    public bool   playOnAwake   = false;
    public float  duration      = 1.0f;
    public float  delay         = 0.0f;

    public    bool resetStartPos = false;
    protected bool isPositionRemember = false;

    protected bool isPlaying = false;
    public bool IsPlaying { get { return isPlaying; } }

    //position
    public    bool           enablePosition         = false;
	public    Vector3        positionFrom           = Vector3.zero;
	public    Vector3        positionTo             = Vector3.zero;
	public    AnimationCurve positionAnimationCurve = AnimationCurve.Linear (0, 0, 1, 1);
	protected Vector3        startPosition          = Vector3.zero;

    //rotation
    public    bool           enableRotation         = false;
	public    bool           isRotationY            = false;
	public    float          rotationFrom           = 0.0f;
	public    float          rotationTo             = 0.0f;
	public    AnimationCurve rotationAnimationCurve = AnimationCurve.Linear (0, 0, 1, 1);
	protected float          startRotation          = 0.0f;

    //scale
    public bool enableScale = false;
	public bool scaleSyncXY = true;
	public float scaleFrom = 0;
	public float scaleTo = 1.0f;
	public AnimationCurve scaleAnimationCurve = AnimationCurve.Linear (0, 0, 1, 1);
	public float scaleFromY = 0;
	public float scaleToY = 1.0f;
	public AnimationCurve scaleAnimationCurveY = AnimationCurve.Linear (0, 0, 1, 1);

    //color
    public bool enableColor = false;
	public Color colorFrom = Color.white;
	public Color colorTo = Color.white;
	public AnimationCurve colorAnimationCurve = AnimationCurve.Linear (0, 0, 1, 1);
	
    public UnityEvent OnComplete;

    protected bool isStopOnLoopEnd = false;
    protected bool isPaused = false;

    void Awake()
    {
        cachedTransform = transform;
        image           = GetComponent<Image>();
        text            = GetComponent<Text>();
        sprite          = GetComponent<SpriteRenderer>();
        canvasGroup     = GetComponent<CanvasGroup>();
    }

	protected void OnEnable()
	{
        //if( gameObject.name == "Coma" )
        //{
        //    Debug.Log("CurveAnimator : OnEnable : " + gameObject.name);
        //}
        if( playOnAwake )
        {
            Play();
        }
	}

	protected void OnDisable ()
	{
        //if( gameObject.name == "Coma" )
        //{
        //    Debug.Log("CurveAnimator : OnDisable : " + gameObject.name);
        //}
		Stop();
	}

	public void StopOnLoopEnd()
	{
        isStopOnLoopEnd = true;
	}

	public void Play()
    {
        if( delay != 0.0f )
        {
            Invoke( "play", delay );
        }
        else
        {
            play();
        }
    }

    [ContextMenu("Play")]
	private void play()
    {
        //@memo アクティブじゃない時にStartCoroutineが走るとエラーなので
		if(gameObject.activeInHierarchy == false) return;

		if(playCoroutine != null) StopCoroutine(playCoroutine);

        counter = 0.0f;
        isPaused = false;
        isStopOnLoopEnd = false;

        if( resetStartPos )
        {
            if( !isPositionRemember )
            {
                isPositionRemember = true;
                startPosition = cachedTransform.localPosition;
            }
        }
        else
        {
            startPosition = cachedTransform.localPosition;
        }
        startRotation = cachedTransform.localRotation.eulerAngles.z;

        isPlaying = true;

		switch (style)
		{
			case EStyle.Once:
                playCoroutine = StartCoroutine( playOnceCoroutine() );
				break;
			case EStyle.Loop:
                playCoroutine = StartCoroutine( playLoopCoroutine() );
				break;
		}
    }

    [ContextMenu("Stop")]
	public void Stop()
    {
		if(playCoroutine != null) StopCoroutine(playCoroutine);
		playCoroutine = null;
        isPaused = false;
        isPlaying = false;
    }

    [ContextMenu("Pause")]
	public void Pause()
    {
        isPaused = true;
    }

    [ContextMenu("Resume")]
	public void Resume()
    {
        isPaused = false;
    }

	public void SetFrame( float f )
    {
        var t = UnityEngine.Mathf.Clamp( f / duration, 0.0f, 1.0f );

        if( enablePosition )
        {
            updatePostion( t );
        }

        if( enableRotation )
        {
            updateRotation( t );
        }

        if( enableScale )
        {
            updateScale( t );
        }

        if( enableColor )
        {
            Debug.Assert( !( image == null && text == null && sprite == null && canvasGroup == null ) );
            updateColor( t );
        }
    }

	private IEnumerator playOnceCoroutine ()
    {
        float t = 0.0f;
		while(t < 1.0f)
		{
            while(isPaused)
            {
                yield return null;
            }

            counter += Time.deltaTime;
            t = UnityEngine.Mathf.Clamp( counter / duration, 0.0f, 1.0f );

            if( isReverse )
            {
                SetFrame( duration - counter );
            }
            else
            {
                SetFrame( counter );
            }

            yield return null;
		}

        OnComplete.Invoke();
    }

	IEnumerator playLoopCoroutine ()
	{
        float t = 0.0f;
		while(true)
		{
            while(isPaused)
            {
                yield return null;
            }

            counter += Time.deltaTime;
            bool is_stop = false;
			while( counter > duration )
			{
				counter -= duration;
                if( isStopOnLoopEnd )
                {
                    counter = duration;
                    isStopOnLoopEnd = false;
                    is_stop = true;
                }
			}

            if( isReverse )
            {
                SetFrame( duration - counter );
            }
            else
            {
                SetFrame( counter );
            }

            if( is_stop )
            {
                break;
            }

			yield return null;
		}
        isPlaying = false;

        OnComplete.Invoke();
	}

    void updatePostion( float t )
    {
        var k = positionAnimationCurve.Evaluate( t );
        var pos = positionFrom * ( 1.0f - k ) + positionTo * k;
        cachedTransform.localPosition = startPosition + pos; 
    }

    void updateRotation( float t )
    {
        var k = rotationAnimationCurve.Evaluate( t );
        var rot = rotationFrom * ( 1.0f - k ) + rotationTo * k;
        if( isRotationY )
        {
            cachedTransform.localRotation = Quaternion.Euler( 0.0f, startRotation + rot, 0.0f );
        }
        else
        {
            cachedTransform.localRotation = Quaternion.Euler( 0.0f, 0.0f, startRotation + rot );
        }
    }

    void updateScale( float t )
    {
        if( scaleSyncXY )
        {
            var k = scaleAnimationCurve.Evaluate( t );
            /* var scale = Mathf.Lerp( scaleFrom, scaleTo, k ); */
            //@memo 標準のlerpだとが外挿してくれない...
            var scale = scaleFrom * ( 1.0f - k ) + scaleTo * k;
            cachedTransform.localScale = Vector3.one * scale; 
        }
        else
        {
            var scale = cachedTransform.localScale;
            var kx = scaleAnimationCurve.Evaluate( t );
            //@memo 標準のlerpだとが外挿してくれない...
            /* var scale_x = Mathf.Lerp( scaleFrom, scaleTo, kx ); */
            var scale_x = scaleFrom * ( 1.0f - kx ) + scaleTo * kx;
            var ky = scaleAnimationCurveY.Evaluate( t );
            /* var scale_y = Mathf.Lerp( scaleFromY, scaleToY, ky ); */
            var scale_y = scaleFromY * ( 1.0f - ky ) + scaleToY * ky;
            scale.x = scale_x;
            scale.y = scale_y;
            cachedTransform.localScale = scale; 
        }
    }

    void updateColor( float t )
    {
        if( image == null && text == null && sprite == null && canvasGroup == null ) return;
        var k = colorAnimationCurve.Evaluate( t );
        var color = Color.Lerp( colorFrom, colorTo, k );
        if( image != null )
        {
            image.color = color;
        }
        if( text != null )
        {
            text.color = color;
        }
        if( sprite != null )
        {
            sprite.color = color;
        }
        if( canvasGroup != null )
        {
            canvasGroup.alpha = color.a;
        }
    }

}

}
