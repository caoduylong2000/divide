using System;
using UnityEngine;//for Vector

namespace WK { 
namespace Math { 
    public static class Mathf
    {
        public static Vector3 Lerp( Vector3 a, Vector3 b, float t )
        {
            return a * ( 1.0f - t ) + b * t;
        }

        public static Vector2 Lerp( Vector2 a, Vector2 b, float t )
        {
            return a * ( 1.0f - t ) + b * t;
        }

        public static float LenSqr( Vector3 v )
        {
            return v.x * v.x + v.y * v.y + v.z * v.z;
        }

        public static float LenSqr( Vector2 v )
        {
            return v.x * v.x + v.y * v.y;
        }

        public static Vector3 Approach( out bool result, Vector3 target_v, Vector3 current_v, float speed, float eps_sq )
        {
            Debug.Assert( speed <= 1.0f );
            speed = UnityEngine.Mathf.Min( speed, 1.0f );
            Vector3 rel_v = target_v - current_v;
            Vector3 out_v = current_v + rel_v * speed;
            if( LenSqr( rel_v ) < eps_sq )
            {
                result = true;
                return target_v;
            }
            else
            {
                result = false;
            }
            return out_v;
        }

        public static Vector2 Approach( out bool result, Vector2 target_v, Vector2 current_v, float speed, float eps_sq )
        {
            Debug.Assert( speed <= 1.0f );
            speed = UnityEngine.Mathf.Min( speed, 1.0f );
            Vector2 rel_v = target_v - current_v;
            Vector2 out_v = current_v + rel_v * speed;
            if( LenSqr( rel_v ) < eps_sq )
            {
                result = true;
                return target_v;
            }
            else
            {
                result = false;
            }
            return out_v;
        }

        public static float Approach( out bool result, float target_f, float current_f, float speed, float eps )
        {
            Debug.Assert( speed <= 1.0f );
            speed = UnityEngine.Mathf.Min( speed, 1.0f );
            float rel = target_f - current_f;
            float out_f = current_f + rel * speed;
            if( UnityEngine.Mathf.Abs( rel ) < eps )
            {
                out_f = target_f;
                result = true;
            }
            else
            {
                result = false;
            }
            return out_f;
        }
    }
}
}
