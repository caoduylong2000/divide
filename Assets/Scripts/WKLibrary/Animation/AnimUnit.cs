using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace WK { namespace Animation {
    public class AnimUnit {
        public CanvasGroup CanvasGroup{ get; set; }

        public int Type{ get; set; }

        private const int cTypeCanvasGroup = 0;

        public delegate void CompleteCallback(CanvasGroup group);
        private CompleteCallback completeCallback = null;

        //------------------------------------------------------------------------------
        public AnimUnit OnComplete(CompleteCallback callback)
        {
            completeCallback = callback;
            return this;
        }

        //------------------------------------------------------------------------------
        public IEnumerator FadeCoroutine(float to, float duration)
        {
            /* int times = (int)( duration / Time.deltaTime ); */
            /* int times = duration; */
            /* float speed = ( to - CanvasGroup.alpha ) / times; */
            float speed = ( to - CanvasGroup.alpha ) / duration;
            while( ( duration = duration - Time.deltaTime ) > 0 )
            {
                CanvasGroup.alpha += speed * Time.deltaTime;
                yield return null;
            }
            CanvasGroup.alpha = to;
            if(completeCallback != null)
            {
                completeCallback(CanvasGroup);
                completeCallback= null;
            }
        }
    }
}}
