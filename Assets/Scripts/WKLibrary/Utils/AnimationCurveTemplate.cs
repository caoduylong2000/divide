using System;
using System.Collections.Generic;
using UnityEngine;//for random

namespace WK {

public static class AnimationCurveTemplate {
    public enum ECurve {
        linear,
        easeInOut,
        easeOutSine,
        easeOutCubic,
        easeOutElastic,
        easeInSine,
        easeInCubic,
        elasticOut,
        Bounce,
    };

    static public AnimationCurve Linear    = AnimationCurve.Linear( 0.0f, 0.0f, 1.0f, 1.0f );
    static public AnimationCurve EaseInOut = AnimationCurve.EaseInOut( 0.0f, 0.0f, 1.0f, 1.0f );
    static public AnimationCurve EaseOutSine;
    static public AnimationCurve EaseOutCubic;
    static public AnimationCurve EaseOutElastic;
    static public AnimationCurve EaseInSine;
    static public AnimationCurve EaseInCubic;
    static public AnimationCurve ElasticOut;
    static public AnimationCurve Bounce;

    static public AnimationCurve GetCurve( ECurve c )
    {
        switch( c )
        {
            case ECurve.linear:
                return Linear;
                break;
            case ECurve.easeInOut:
                return EaseInOut;
                break;
            case ECurve.easeOutSine:
                return EaseOutSine;
                break;
            case ECurve.easeOutCubic:
                return EaseOutCubic;
                break;
            case ECurve.easeOutElastic:
                return EaseOutElastic;
                break;
            case ECurve.easeInSine:
                return EaseInSine;
                break;
            case ECurve.easeInCubic:
                return EaseInCubic;
                break;
            case ECurve.elasticOut:
                return ElasticOut;
                break;
            case ECurve.Bounce:
                return Bounce;
                break;
        }
        Debug.LogError("illegal ECurve");
        return Linear;
    }

    public static void Init()
    {
        {
            EaseOutSine = new AnimationCurve();
            var keys = new float[][] {
                new float[] {2f, 0.3333333f, 2f, 0.3333333f, 0f, 0f }
                , new float[] {0f, 0.3333333f, 0f, 0.3333333f, 1f, 1f }
            };
            for( int i = 0; i < keys.Length; ++i )
            {
                var key = new Keyframe();
                key.inTangent  = keys[i][0];
                key.inWeight   = keys[i][1];
                key.outTangent = keys[i][2];
                key.outWeight  = keys[i][3];
                key.time       = keys[i][4];
                key.value      = keys[i][5];
                EaseOutSine.AddKey( key );
            }
        }

        {
            EaseOutCubic = new AnimationCurve();
            var keys = new float[][] {
                 new float[] {4.673104f, 0.3333333f, 4.673104f, 0.1061431f, 0f, 0f }
               , new float[] {1.696814f, 0.3333333f, 1.696814f, 0.1478284f, 0.2257983f, 0.7133362f }
               , new float[] {0.3233358f, 0.3333333f, 0.3233358f, 0.08139559f, 0.4649858f, 0.9113398f }
               , new float[] {0f, 0.3333333f, 0f, 0.3333333f, 1f, 1f }
            };
            for( int i = 0; i < keys.Length; ++i )
            {
                var key = new Keyframe();
                key.inTangent  = keys[i][0];
                key.inWeight   = keys[i][1];
                key.outTangent = keys[i][2];
                key.outWeight  = keys[i][3];
                key.time       = keys[i][4];
                key.value      = keys[i][5];
                EaseOutCubic.AddKey( key );
            }
        }

        {
            EaseOutElastic = new AnimationCurve();
            float[][] keys = new float[][]{
                  new float[] { 9.262799f,    0.3333333f, 9.262799f,    0.05387204f, 0.0f,       0.0f       }
                , new float[] { -0.04204061f, 0.3333333f, -0.04204061f, 0.2664664f,  0.1520271f, 1.197994f  }
                , new float[] { -0.01311274f, 0.3333333f, -0.01311274f, 0.1932568f,  0.300508f,  0.9009288f }
                , new float[] { 0.06282867f,  0.3333333f, 0.06282867f,  0.2160169f,  0.4503533f, 1.05362f   }
                , new float[] { -0.02260892f, 0.3333333f, -0.02260892f, 0.106497f,   0.6012712f, 0.9745393f }
                , new float[] { -0.04592519f, 0.3333333f, -0.04592519f, 0.147563f,   0.7502257f, 1.0122f    }
                , new float[] { 0.0f,         0.3333333f, 0.0f,         0.3333333f,  1f,         1f         }
            };
            for( int i = 0; i < keys.Length; ++i )
            {
                var key = new Keyframe();
                key.inTangent  = keys[i][0];
                key.inWeight   = keys[i][1];
                key.outTangent = keys[i][2];
                key.outWeight  = keys[i][3];
                key.time       = keys[i][4];
                key.value      = keys[i][5];
                EaseOutElastic.AddKey( key );
            }
        }

        {
            EaseInSine = new AnimationCurve();
            var keys = new float[][] {
                  new float[] {0f, 0.3333333f, 0f, 0.3333333f, 0f, 0f }
                , new float[] {1.908523f, 0.0289256f, 1.908523f, 0.3333333f, 1f, 1f }
            };
            for( int i = 0; i < keys.Length; ++i )
            {
                var key = new Keyframe();
                key.inTangent  = keys[i][0];
                key.inWeight   = keys[i][1];
                key.outTangent = keys[i][2];
                key.outWeight  = keys[i][3];
                key.time       = keys[i][4];
                key.value      = keys[i][5];
                EaseInSine.AddKey( key );
            }
        }

        {
            EaseInCubic = new AnimationCurve();
            var keys = new float[][] {
                new float[] {0.06696551f, 0.3333333f, 0.06696551f, 0.06310838f, 0f, 0f }
                , new float[] {0.9916368f, 0.3333333f, 0.9916368f, 0.3724979f, 0.7526175f, 0.2162872f }
                , new float[] {1.963568f, 0.164645f, 1.963568f, 0.1849736f, 0.8623065f, 0.391315f }
                , new float[] {8.074507f, 0.08006028f, 8.074507f, 0.3333333f, 1f, 1f }
            };
            for( int i = 0; i < keys.Length; ++i )
            {
                var key = new Keyframe();
                key.inTangent  = keys[i][0];
                key.inWeight   = keys[i][1];
                key.outTangent = keys[i][2];
                key.outWeight  = keys[i][3];
                key.time       = keys[i][4];
                key.value      = keys[i][5];
                EaseInCubic.AddKey( key );
            }
        }

        {
            ElasticOut = new AnimationCurve();
            var keys = new float[][] {
                new float[] {-0.9905003f, 0.3333333f, -0.9905003f, 0.4033525f, 0f, 1f }
                , new float[] {0f, 0.3333333f, 0f, 0.3333333f, 0.09917323f, 0.8941354f }
                , new float[] {-2.183656f, 0.3333333f, -2.183656f, 0.04041768f, 0.2140469f, 1.187092f }
                , new float[] {0f, 0.3333333f, 0f, 0.3333333f, 1f, 0f }
            };
            for( int i = 0; i < keys.Length; ++i )
            {
                var key = new Keyframe();
                key.inTangent  = keys[i][0];
                key.inWeight   = keys[i][1];
                key.outTangent = keys[i][2];
                key.outWeight  = keys[i][3];
                key.time       = keys[i][4];
                key.value      = keys[i][5];
                ElasticOut.AddKey( key );
            }
        }

        {
            Bounce = new AnimationCurve();
            var keys = new float[][] {
                  new float[] {1.854388f, 0.3333333f, 1.854388f, 0.09200365f, 0.0f, 1.0f }
                , new float[] {-0.002724095f, 0.3333333f, -0.002724095f, 0.2972133f, 0.1512088f, 1.199471f }
                , new float[] {-0.01311274f, 0.3333333f, -0.01311274f, 0.1932568f, 0.300508f, 0.9009288f }
                , new float[] {0.06282867f, 0.3333333f, 0.06282867f, 0.2160169f, 0.4503533f, 1.05362f }
                , new float[] {-0.02260892f, 0.3333333f, -0.02260892f, 0.106497f, 0.6012712f, 0.9745393f }
                , new float[] {-0.04592519f, 0.3333333f, -0.04592519f, 0.147563f, 0.7502257f, 1.0122f }
                , new float[] {0f, 0.3333333f, 0f, 0.3333333f, 1f, 1f }
            };
            for( int i = 0; i < keys.Length; ++i )
            {
                var key = new Keyframe();
                key.inTangent  = keys[i][0];
                key.inWeight   = keys[i][1];
                key.outTangent = keys[i][2];
                key.outWeight  = keys[i][3];
                key.time       = keys[i][4];
                key.value      = keys[i][5];
                Bounce.AddKey( key );
            }
        }

    }

    class Initializer {
        public Initializer()
        {
            AnimationCurveTemplate.Init();
        }
    }
    private static Initializer initializer = new Initializer();

}

}

