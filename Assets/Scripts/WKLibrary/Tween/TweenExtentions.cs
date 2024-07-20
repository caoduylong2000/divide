using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//拡張メソッド系だけはusingしないと使えないので,namespaceを二重にしておく
namespace WK {
namespace Tween {

public static class TweenExtentions
{
    public static TweenUnit TwWait( this CanvasGroup canvas_group, float duration ) { return waitImpl( duration ); }
    public static TweenUnit TwWait( this Image image, float duration ) { return waitImpl( duration ); }
    public static TweenUnit TwWait( this Text text, float duration ) { return waitImpl( duration ); }
    public static TweenUnit TwWait( this Transform transform, float duration ) { return waitImpl( duration ); }

    //------------------------------------------------------------------------------
    private static TweenUnit waitImpl( float duration )
    {
        TweenUnit unit = new TweenUnit();
        TweenManager.Instance.StartCoroutine( unit.WaitCoroutine( duration ) );
        return unit;
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwFade( this CanvasGroup canvas_group, float to, float duration )
    {
        return TwFade( canvas_group, to, duration, Easing.EType.linear );
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwFade( this CanvasGroup canvas_group, float to, float duration, Easing.EType type )
    {
        TweenUnit unit = new TweenUnit();
        TweenManager.Instance.StartCoroutine( unit.FadeCoroutine( canvas_group, to, duration, type ) );
        return unit;
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwFade( this Image image, float to, float duration )
    {
        return TwFade( image, to, duration, Easing.EType.linear );
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwFade( this Image image, float to, float duration, Easing.EType type  )
    {
        TweenUnit unit = new TweenUnit();
        TweenManager.Instance.StartCoroutine( unit.FadeCoroutine( image, to, duration, type ) );
        return unit;
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwFade( this Text text, float to, float duration )
    {
        return TwFade( text, to, duration, Easing.EType.linear );
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwFade( this Text text, float to, float duration, Easing.EType type )
    {
        TweenUnit unit = new TweenUnit();
        TweenManager.Instance.StartCoroutine( unit.FadeCoroutine( text, to, duration, type ) );
        return unit;
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwScale( this Text text, Vector3 to_scale, float duration )
    {
        TweenUnit unit = TwScale( text.transform, to_scale, duration );
        return unit;
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwScale( this Text text, Vector3 from_scale, Vector3 to_scale, float duration )
    {
        TweenUnit unit = TwScale( text.transform, from_scale, to_scale, duration );
        return unit;
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwScale( this Transform transform, Vector3 from_scale, Vector3 to_scale, float duration )
    {
        transform.localScale = from_scale;
        return TwScale( transform, to_scale, duration );
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwScale( this Transform transform, Vector3 to_scale, float duration )
    {
        return TwScale( transform, to_scale, duration, Easing.EType.linear );
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwScale( this Transform transform, Vector3 to_scale, float duration, Easing.EType type )
    {
        TweenUnit unit = new TweenUnit();
        TweenManager.Instance.StartCoroutine( unit.ScaleCoroutine( transform, to_scale, duration, type ) );
        return unit;
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwRotate( this Transform transform, Vector3 from_angles, Vector3 to_angles, float duration )
    {
        transform.localRotation = Quaternion.Euler( from_angles );
        return TwScale( transform, to_angles, duration );
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwRotate( this Transform transform, Vector3 to_angles, float duration )
    {
        TweenUnit unit = new TweenUnit();
        TweenManager.Instance.StartCoroutine( unit.RotateCoroutine( transform, to_angles, duration ) );
        return unit;
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwMove( this Transform transform, Vector3 from_pos, Vector3 to_pos, float duration )
    {
        transform.localPosition = from_pos;
        return TwMove( transform, to_pos, duration );
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwMove( this Transform transform, Vector3 to_pos, float duration )
    {
        return TwMove( transform, to_pos, duration, Easing.EType.linear );
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwMove( this Transform transform, Vector3 to_pos, float duration, Easing.EType type )
    {
        TweenUnit unit = new TweenUnit();
        TweenManager.Instance.StartCoroutine( unit.MoveCoroutine( transform, to_pos, duration, type, false ) );
        return unit;
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwMoveGlobal( this Transform transform, Vector3 from_pos, Vector3 to_pos, float duration )
    {
        transform.position = from_pos;
        return TwMoveGlobal( transform, to_pos, duration );
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwMoveGlobal( this Transform transform, Vector3 to_pos, float duration )
    {
        return TwMoveGlobal( transform, to_pos, duration, Easing.EType.linear );
    }

    //------------------------------------------------------------------------------
    public static TweenUnit TwMoveGlobal( this Transform transform, Vector3 to_pos, float duration, Easing.EType type )
    {
        TweenUnit unit = new TweenUnit();
        TweenManager.Instance.StartCoroutine( unit.MoveCoroutine( transform, to_pos, duration, type, true ) );
        return unit;
    }
}

}
}
