using UnityEngine;
using System.Collections;

namespace WK { namespace Animation {
    public static class CanvasGroupExtention 
    {
        public static AnimUnit AnimFade(this CanvasGroup canvas_group, float to, float duration)
        {
            AnimUnit unit = new AnimUnit();
            unit.CanvasGroup = canvas_group;
            Debug.Assert( canvas_group != null );
            /* AnimationManager.Instance.StartCoroutine( canvas_group.FadeCoroutine( to, duration ) ); */
            /* canvas_group.GetComponent<MonoBehaviour>().StartCoroutine( canvas_group.FadeCoroutine( to, duration ) ); */
            /* canvas_group.GetComponent<MonoBehaviour>().StartCoroutine( unit.FadeCoroutine( to, duration ) ); */
            AnimationManager.Instance.StartCoroutine( unit.FadeCoroutine( to, duration ) );
            return unit;
        }

        /* public static CanvasGroup OnComplete(this CanvasGroup canvas_group) */
        /* { */
        /*     Debug.Log("Complete"); */
        /*     return canvas_group; */
        /* } */

        /* public static IEnumerator FadeCoroutine(this CanvasGroup canvas_group, float to, float duration ) */
        /* { */
        /*     int times = (int)( duration / Time.deltaTime ); */
        /*     float speed = ( to - canvas_group.alpha ) / times; */
        /*     /1* Debug.Log( "times : " + times.ToString() ); *1/ */
        /*     /1* Debug.Log( "Fade : " + canvas_group.alpha.ToString() ); *1/ */
        /*     while( times-- > 0 ) */
        /*     { */
        /*         canvas_group.alpha += speed; */
        /*         /1* Debug.Log( "Fade : " + canvas_group.alpha.ToString() ); *1/ */
        /*         yield return null; */
        /*     } */
        /*     canvas_group.alpha = to; */
        /* } */
    }   
}}
