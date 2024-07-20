using UnityEngine;
using System;
using System.IO;
using System.Collections;
using WK.Unity;

public class ShareButton : MonoBehaviour {
    string imageName = "com_waken_divide_ss.png";
    string imagePath
    {
        get
        {
            return System.IO.Path.Combine (Application.persistentDataPath,imageName);
        }
    }

    public void ClickCallback()
    {
        StartCoroutine( captureAndShare() );
    }

    private IEnumerator captureAndShare()
    {
        deleteSS();

        WK.AdListenerMax.Instance.HideBanner();

        // レイアウト設定のために1フレーム待つ
        yield return new WaitForEndOfFrame();

        ScreenCapture.CaptureScreenshot( imageName );

        int counter = 0;
        //ファイルができるまで待つ
        while ( !File.Exists( imagePath ) ) {
            counter++;
            if( counter > 120 ) { break; }//何らかの理由で2秒経っても生成されない時は抜ける
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();//ファイルができてても書き込み終わっていないかもしれないので念のためもう1フレーム待つ

        share();

        if( !( GameSceneManager.Instance.IsPurchasedNoAds ||
                    GameSceneManager.Instance.IsSubscribedNoAds ) )
        {
            WK.AdListenerMax.Instance.ShowBanner();
        }
    }

    private void deleteSS()
    {
        if ( File.Exists( imagePath ) == true ) {
            File.Delete( imagePath ) ;
        }
    }

    private void share()
    {
        string appURL = "";
#if UNITY_ANDROID
        appURL = "https://play.google.com/store/apps/details?id=" + Config.androidAppId;
#elif UNITY_IOS
        appURL = "https://itunes.apple.com/jp/app/divide-number-puzzle/id" + Config.iOSAppId;
#endif

        int score = GameObject.Find( "GameSceneManager" ).GetComponent< GameSceneManager >().GetCurrentScore();

		if (Application.systemLanguage == SystemLanguage.Japanese)
        {
            SocialConnector.SocialConnector.Share ("【Divide】で、"
                    + score.ToString()
                    + "点とりました。#AppDivide", appURL, imagePath);
        }
        else
        {
            SocialConnector.SocialConnector.Share ("【Divide】I got "
                    + score.ToString()
                    + " points!#AppDivide", appURL, imagePath);
        }
    }

    //閉じるときに一時ファイルを削除する。
    private void OnApplicationQuit()
    {
        deleteSS();
    }
}
