//------------------------------------------------------------------------
//
// (C) Copyright 2018 Nukenin Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections;

namespace WK {
namespace Unity {

public class ProgressBar : MonoBehaviour {
    [SerializeField]
    protected RectTransform barRect;

    [SerializeField]
    protected float barMinSize;

    [SerializeField]
    protected float duration = 0.5f;

    [SerializeField]
    protected string seName;

    [SerializeField]
    protected AnimationCurveTemplate.ECurve barMovement = AnimationCurveTemplate.ECurve.easeOutSine;

    protected float defaultBarSize;
    protected Coroutine slideCoroutine = null;

    void Awake() {
    }

    void Start() {
        defaultBarSize = barRect.rect.size.x;
    }

    public void SetBarSizeByRate( float rate ) {
        var size = barRect.sizeDelta;
        if( rate == 0.0f )
        {
            size.x = 0.0f;
        }
        else
        {
            size.x = rate * ( defaultBarSize - barMinSize ) + barMinSize;
        }
        barRect.sizeDelta = size;
        //Debug.Log( "sizeDelta : " + defaultBarSize + "," + size );
    }

    public void SlideTo( float rate, Action complete_callback ) {
        slideCoroutine = StartCoroutine( slideToProcess( rate, complete_callback ) );
    }

    public void StopSlide() {
        if( slideCoroutine != null )
        {
            Debug.Log( "StopSlide" );
            StopCoroutine( slideCoroutine );
            slideCoroutine = null;
        }
    }

    IEnumerator slideToProcess( float rate, Action complete_callback ) {
        var size = barRect.sizeDelta;
        float prev_rate = UnityEngine.Mathf.Max( 0.0f, ( size.x - barMinSize ) / ( defaultBarSize - barMinSize ) );
        float diff = rate - prev_rate;
        float dt = 1.0f / duration;
        float t = 0.0f;
        if( seName != "" )
        {
            WK.Audio.SoundManager.Instance.PlaySe( seName );
        }
        while(true)
        {
            t += dt * Time.deltaTime;
            if( t > 1.0f )
                t = 1.0f;

            float r = UnityEngine.Mathf.Lerp( prev_rate, rate, WK.AnimationCurveTemplate.GetCurve(barMovement).Evaluate(t) );
            if( r > 1.0f )
                r = 1.0f;

            SetBarSizeByRate(r);

            if( t == 1.0f )
            {
                break;
            }

            if( r == 1.0f )
            {
                prev_rate = 0.0f;
                rate -= 1.0f;
                prev_rate = 0.0f;
                t = 0.0f;

                SetBarSizeByRate(1.0f);
            }

            yield return null;
        }

        if( complete_callback != null )
        {
            complete_callback();
        }
        slideCoroutine = null;
    }

#if UNITY_EDITOR
    [ContextMenu("Test")]
    void Test()
    {
        var size = barRect.sizeDelta;
        size.x = 0.0f;
        barRect.sizeDelta = size;
        SlideTo( 1.0f, null );
    }
#endif

}
}
}
