using System;
using System.Collections.Generic;
using UnityEngine;//for random

namespace WK {

public static class Easing {
    public enum EType {
        linear,
        easeInOut,
        easeOutSine,
        easeOutCubic,
        easeOutElastic,
        easeInSine,
        easeInCubic,
        elasticOut,
        bounce,
    };

    public static float Evaluate( EType tp, float t )
    {
        return AnimationCurveTemplate.GetCurve( (AnimationCurveTemplate.ECurve)tp ).Evaluate(t);
    }

    public static float Linear( float t )
    {
        return AnimationCurveTemplate.Linear.Evaluate(t);
    }

    public static float EaseInOut( float t )
    {
        return AnimationCurveTemplate.EaseInOut.Evaluate(t);
    }

    public static float EaseOutSine( float t )
    {
        return AnimationCurveTemplate.EaseOutSine.Evaluate(t);
    }

    public static float EaseOutCubic( float t )
    {
        return AnimationCurveTemplate.EaseOutCubic.Evaluate(t);
    }

    public static float EaseOutElastic( float t )
    {
        return AnimationCurveTemplate.EaseOutElastic.Evaluate(t);
    }

    public static float EaseInSine( float t )
    {
        return AnimationCurveTemplate.EaseInSine.Evaluate(t);
    }

    public static float EaseInCubic( float t )
    {
        return AnimationCurveTemplate.EaseInCubic.Evaluate(t);
    }

    public static float ElasticOut( float t )
    {
        return AnimationCurveTemplate.ElasticOut.Evaluate(t);
    }

    public static float Bounce( float t )
    {
        return AnimationCurveTemplate.Bounce.Evaluate(t);
    }
}

}
