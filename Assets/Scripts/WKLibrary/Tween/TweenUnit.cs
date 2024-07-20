using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace WK {

public class TweenUnit {
    private Transform cachedTransform;
    private Action completeCallback = null;

    //------------------------------------------------------------------------------
    public TweenUnit OnComplete(Action callback)
    {
        completeCallback = callback;
        return this;
    }

    //------------------------------------------------------------------------------
    public IEnumerator WaitCoroutine( float duration )
    {
        yield return new WaitForSeconds( duration );
        if(completeCallback != null)
        {
            completeCallback();
            completeCallback= null;
        }
    }

    //------------------------------------------------------------------------------
    public IEnumerator FadeCoroutine( CanvasGroup canvas_group, float to_alpha, float duration, Easing.EType type )
    {
        float from_alpha = canvas_group.alpha;
        float dt = 1.0f / duration;
        float t = 0.0f;
        while( true )
        {
            t = UnityEngine.Mathf.Min( 1.0f, t + dt * Time.deltaTime );
            var k = WK.Easing.Evaluate( type, t );
            var alpha = UnityEngine.Mathf.Lerp( from_alpha, to_alpha, k );
            canvas_group.alpha = alpha;

            if( t == 1.0f )
            {
                break;
            }

            yield return null;
        }
        if(completeCallback != null)
        {
            completeCallback();
            completeCallback= null;
        }
    }

    //------------------------------------------------------------------------------
    public IEnumerator FadeCoroutine( Image image, float to_alpha, float duration, Easing.EType type )
    {
        float from_alpha = image.color.a;
        Color color = image.color;
        float dt = 1.0f / duration;
        float t = 0.0f;
        while( true )
        {
            t = UnityEngine.Mathf.Min( 1.0f, t + dt * Time.deltaTime );
            var k = WK.Easing.Evaluate( type, t );
            var alpha = UnityEngine.Mathf.Lerp( from_alpha, to_alpha, k );
            color.a = alpha;
            image.color = color;

            if( t == 1.0f )
            {
                break;
            }

            yield return null;
        }

        if(completeCallback != null)
        {
            completeCallback();
            completeCallback= null;
        }
    }

    //------------------------------------------------------------------------------
    public IEnumerator FadeCoroutine( Text text, float to_alpha, float duration, Easing.EType type )
    {
        float from_alpha = text.color.a;
        Color color = text.color;
        float dt = 1.0f / duration;
        float t = 0.0f;
        while( true )
        {
            t = UnityEngine.Mathf.Min( 1.0f, t + dt * Time.deltaTime );
            var k = WK.Easing.Evaluate( type, t );
            var alpha = UnityEngine.Mathf.Lerp( from_alpha, to_alpha, k );
            color.a = alpha;
            text.color = color;

            if( t == 1.0f )
            {
                break;
            }

            yield return null;
        }

        if(completeCallback != null)
        {
            completeCallback();
            completeCallback= null;
        }
    }

    //------------------------------------------------------------------------------
    public IEnumerator MoveCoroutine( Transform transform, Vector3 to_pos, float duration, Easing.EType type, bool is_global )
    {
        cachedTransform = transform;
        var from_pos = is_global ? cachedTransform.position : cachedTransform.localPosition;

        float dt = 1.0f / duration;
        float t = 0.0f;
        while( true )
        {
            t = UnityEngine.Mathf.Min( 1.0f, t + dt * Time.deltaTime );
            var k = WK.Easing.Evaluate( type, t );
            var pos = Vector3.Lerp( from_pos, to_pos, k );
            if( is_global )
            {
                cachedTransform.position = pos;
            }
            else
            {
                cachedTransform.localPosition = pos;
            }

            if( t >= 1.0f )
            {
                break;
            }

            yield return null;
        }

        //Vector3 speed = ( to_pos - cachedTransform.localPosition ) / duration;
        //while( ( duration = duration - Time.deltaTime ) > 0 )
        //{
        //    cachedTransform.localPosition += speed * Time.deltaTime;
        //    yield return null;
        //}

        if( is_global )
        {
            cachedTransform.position = to_pos;
        }
        else
        {
            cachedTransform.localPosition = to_pos;
        }

        if(completeCallback != null)
        {
            completeCallback();
            completeCallback= null;
        }
    }

    //------------------------------------------------------------------------------
    public IEnumerator ScaleCoroutine( Transform transform, Vector3 to_scale, float duration, Easing.EType type )
    {
        cachedTransform = transform;
        var from_scale = cachedTransform.localScale;

        float dt = 1.0f / duration;
        float t = 0.0f;
        while( true )
        {
            t = UnityEngine.Mathf.Min( 1.0f, t + dt * Time.deltaTime );
            var k = WK.Easing.Evaluate( type, t );
            var scale = Vector3.Lerp( from_scale, to_scale, k );
            cachedTransform.localScale = scale;

            if( t >= 1.0f )
            {
                break;
            }

            yield return null;
        }

        //Vector3 speed = ( to - cachedTransform.localScale ) / duration;
        //while( ( duration = duration - Time.deltaTime ) > 0 )
        //{
        //    cachedTransform.localScale += speed * Time.deltaTime;
        //    yield return null;
        //}

        cachedTransform.localScale = to_scale;
        if(completeCallback != null)
        {
            completeCallback();
            completeCallback= null;
        }
    }

    //------------------------------------------------------------------------------
    public IEnumerator RotateCoroutine( Transform transform, Vector3 to_angles, float duration )
    {
        cachedTransform = transform;
        Vector3 angles = cachedTransform.localRotation.eulerAngles;
        Vector3 speed = ( to_angles - angles ) / duration;
        while( ( duration = duration - Time.deltaTime ) > 0 )
        {
            angles += speed * Time.deltaTime;
            cachedTransform.localRotation = Quaternion.Euler( angles );
            yield return null;
        }
        cachedTransform.localRotation = Quaternion.Euler( to_angles );
        if(completeCallback != null)
        {
            completeCallback();
            completeCallback= null;
        }
    }

}

}
