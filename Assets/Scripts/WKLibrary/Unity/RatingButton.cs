using UnityEngine;
using System.Collections;

namespace WK { namespace Unity {

public class RatingButton : MonoBehaviour {
    public string clickSeName = null;

    public void ClickCallback(){
        if( clickSeName != null )
        {
            if( WK.Audio.SoundManager.Instance ) WK.Audio.SoundManager.Instance.PlaySe( clickSeName );
        }

#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + Config.androidAppId);
#elif UNITY_IOS
        Application.OpenURL("itms-apps://itunes.apple.com/app/id" + Config.iOSAppId);
#endif
    }
}

}}
