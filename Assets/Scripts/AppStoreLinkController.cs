using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppStoreLinkController : MonoBehaviour {
    [SerializeField]
    protected string androidAppId;

    [SerializeField]
    protected string iOSAppId;

	public void OnClick () {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + androidAppId );
#elif UNITY_IOS
        Application.OpenURL("itms-apps://itunes.apple.com/app/apple-store/id" + iOSAppId + "?ls=1&mt=8" );
#endif
	}
}
