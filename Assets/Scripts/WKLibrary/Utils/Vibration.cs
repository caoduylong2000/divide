using UnityEngine;
using System.Runtime.InteropServices;

namespace WK {
    public static class Vibration {

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void PlaySystemSound(int n);
#endif

        public static void Snap()
        {
#if UNITY_EDITOR
            //Debug.Log( "Snap" );
#elif UNITY_ANDROID
            //イマイチだった
            //int milliseconds = 100;
            //UniAndroidVibration.Vibrate(milliseconds);
#elif UNITY_IOS
            PlaySystemSound(1519);
            //Debug.Log( "Snap" );
#endif
        }
    }
}
