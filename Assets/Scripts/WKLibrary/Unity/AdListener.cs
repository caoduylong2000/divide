//#define ENABLE_GOOGLE_MOBILE_ADS
#if !UNITY_STANDALONE
#define ENABLE_UNITY_ADS
#endif
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Assertions;
#if ENABLE_GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif
#if ENABLE_UNITY_ADS
using UnityEngine.Advertisements;
#endif


namespace WK {
namespace Unity {

public class AdListener : SingletonMonoBehaviour< AdListener > {
#if ENABLE_GOOGLE_MOBILE_ADS
    public AdPosition adPosition = AdPosition.Bottom;

    public string[] testDevices;

    //android
    public string adAppIdAndroid;
    public string adIdBannerAndroid;
    public string adIdInterstitialAndroid;
    public string adIdRewardAndroid;

    //iOS
    public string adAppIdIOS;
    public string adIdBannerIOS;
    public string adIdInterstitialIOS;
    public string adIdRewardIOS;

    public  int interstitialRate        = 1;//interstitialRate回に一回流す。
    private int interstitialRateCounter = 0;

    public  float interstitialInterval     = 60.0f;
    private float previousTime             = 0.0f;

    /* public  int   maxInterstitialPlayCount = 5; */
    public  int   resetAdPlayCountHours    = 20;
    private int   adPlayCount              = 0;

	static readonly string KeyPrevPlayDay = "KeyPrevPlayDay";
	static readonly string KeyAdPlayCount = "KeyAdPlayCount";

    private InterstitialAd     interstitial       = null;
    private AdRequest          request            = null;
    private BannerView         bannerView         = null;
    private RewardBasedVideoAd rewardBasedVideo   = null;

    public Action userCallbackOnCloseInterstitial = null;
    public Action userCallbackOnReward            = null;
    public Action userCallbackOnRewardClose       = null;

    protected bool  isShowingRewardVideo      = false;
    protected float delayedCallTimer          = 0.0f;
    protected int   delayedCallTimeTableIndex = 0;
    protected float[] delayedCallTimeTable = new float[] {
        0.0f,
        5.0f,
        20.0f,
        80.0f,
        300.0f,
    };

	protected bool isUseBanner () {
#if UNITY_ANDROID
        return adIdBannerAndroid != "";
#elif UNITY_IPHONE
        return adIdBannerIOS != "";
#else
        return true;
#endif
    }

	protected bool isUseInterstitial () {
#if UNITY_ANDROID
        return adIdInterstitialAndroid != "";
#elif UNITY_IPHONE
        return adIdInterstitialIOS != "";
#else
        return true;
#endif
    }

	protected bool isUseReward () {
#if UNITY_ANDROID
        return adIdRewardAndroid != "";
#elif UNITY_IPHONE
        return adIdRewardIOS != "";
#else
        return true;
#endif
    }

#if ENABLE_UNITY_ADS
    public bool isUnityAdsOnly;
    const string cUnityAdsPlacementId = "rewardedVideo";
#endif

	protected override void Awake () {
        base.Awake();

#if UNITY_ANDROID
        MobileAds.Initialize(adAppIdAndroid);
#elif UNITY_IPHONE
        MobileAds.Initialize(adAppIdIOS);
#else
#endif

		RequestBanner();
		RequestInterstitial();

        previousTime = 0.0f;

        adPlayCount = PlayerPrefs.GetInt( KeyAdPlayCount, 0 );
        if( isPastResetHoursFromPrevPlay() )
        {
            resetAdPlayCount();
        }

        if ( Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.OSXEditor )
        {
            GetComponent< Image >().enabled = true;
        }
        else
        {
            GetComponent< Image >().enabled = false;
        }


        rewardBasedVideo                        =  RewardBasedVideoAd.Instance;
        // has rewarded the user.
        rewardBasedVideo.OnAdLoaded             += HandleRewardBasedVideoLoaded;
        rewardBasedVideo.OnAdRewarded           += HandleRewardBasedVideoRewarded;
        rewardBasedVideo.OnAdClosed             += HandleRewardBasedVideoRewardedClosed;
        rewardBasedVideo.OnAdFailedToLoad       += HandleRewardBasedVideoRewardedFailed;
        rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeavingApplication;
        initDelayedCallTime();
/* #if ENABLE_UNITY_ADS */
/*         Advertisement.Initialize( unity_ads_id, Debug.isDebugBuild ); */
/* #endif */

#if ENABLE_UNITY_ADS
        //デバッグ番限定フラグゆえ
        isUnityAdsOnly &= UnityEngine.Debug.isDebugBuild;
#endif
	}

    public void Update()
    {
        //リワード動画再生中にリクエストするとコールバックが呼ばれないことがあったので
        //isShowingRewardVideoで弾いている
        if( !IsLoadedRewardBasedVideo() && !isShowingRewardVideo )
        {
            delayedCallTimer -= Time.deltaTime;
            bool is_request_reward_video = false;
            if( delayedCallTimer < 0 )
            {
                is_request_reward_video = true;
                incrementDelayedCallTime();
            }

            if( is_request_reward_video )
            {
                requestRewardBasedVideoImpl();
            }
        }
    }

	private void resetAdPlayCount()
    {
        DateTime now_date_time = DateTime.Now;

        int year  = now_date_time.Year;
        int month = now_date_time.Month;
        int day   = now_date_time.Day;
        int hour  = now_date_time.Hour;

        PlayerPrefs.SetString( KeyPrevPlayDay,
                year.ToString() + "," + month + "," + day + "," + hour );
        adPlayCount = 0;
    }

	private bool isPastResetHoursFromPrevPlay()
	{
        string prev_play_day = PlayerPrefs.GetString( KeyPrevPlayDay, "1970,1,1,0" );
        char[] delimiter = { ',' };
        string[] days = prev_play_day.Split( delimiter );
        int year  = Int32.Parse( days[0] );
        int month = Int32.Parse( days[1] );
        int day   = Int32.Parse( days[2] );
        int hour  = Int32.Parse( days[3] );
        DateTime prev_date_time = new DateTime( year, month, day, hour, 0, 0 );
        DateTime now_date_time = DateTime.Now;
        TimeSpan span = now_date_time - prev_date_time;
        /* Debug.Log( "prev "  + prev_date_time ); */
        /* Debug.Log( "now "  + now_date_time ); */
        /* Debug.Log( "span.Hours " + span.Hours ); */
        if( span.Hours >= resetAdPlayCountHours )
        {
            return true;
        }
        return false;
    }

	private AdRequest birthRequest()
	{
        AdRequest.Builder builder = new AdRequest.Builder();
        for( int i = 0; i < testDevices.Length; ++i )
        {
            builder.AddTestDevice(testDevices[i]);
        }
        return builder.Build();
    }

	private void RequestBanner()
	{
		// 広告ユニット ID を記述します
#if UNITY_ANDROID
            string ad_id = adIdBannerAndroid;
#elif UNITY_IOS
            string ad_id = adIdBannerIOS;
#else
            string ad_id = "unexpected_platform";
#endif

		// Create a banner at the bottom of the screen.
/* #if UNITY_IPHONE */
/*         Debug.Log("iPhoneモデル名" + UnityEngine.iOS.Device.generation.ToString()); */
/*         Debug.Log("Screen : " + Screen.currentResolution.width.ToString() */
/*                 + "," + Screen.currentResolution.height.ToString()); */
/*         if( UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX ) */
/*         { */
/*             bannerView = new BannerView( ad_id, AdSize.SmartBanner, 0, 620 ); */
/*         } */
/*         else */
/*         { */
/*             bannerView = new BannerView( ad_id, AdSize.SmartBanner, adPosition ); */
/*         } */
/* #else */
/*         bannerView = new BannerView( ad_id, AdSize.SmartBanner, adPosition ); */
/* #endif */
        bannerView = new BannerView( ad_id, AdSize.SmartBanner, adPosition );

        /* AdRequest.Builder builder = new AdRequest.Builder(); */
		AdRequest request = birthRequest();
		bannerView.LoadAd(request);
	}

	public void ShowBanner()
    {
        bannerView.Show();
    }

	public void HideBanner()
    {
        bannerView.Hide();
    }

	public void RequestInterstitial()
	{
#if UNITY_ANDROID
        string ad_id = adIdInterstitialAndroid;
#elif UNITY_IOS
        string ad_id = adIdInterstitialIOS;
#else
        string ad_id = "unexpected_platform";
#endif

        interstitial = new InterstitialAd (ad_id);
		AdRequest request = birthRequest();
        interstitial.LoadAd(request);
        interstitial.OnAdClosed += CloseCallback;
    }

    protected void requestRewardBasedVideoImpl()
    {
        Debug.Log( "requestRewardBasedVideoImpl : " + isUseReward() + "," + ( Application.internetReachability == NetworkReachability.NotReachable ) );

        if( !isUseReward() ) return;

        if( Application.internetReachability == NetworkReachability.NotReachable ) return;

        string ad_id;
#if UNITY_EDITOR
        ad_id = "unused";
#elif UNITY_ANDROID
        ad_id = adIdRewardAndroid;
#elif UNITY_IOS
        ad_id = adIdRewardIOS;
#else
        ad_id = "unexpected_platform";
#endif
        /* requestRewardMovie = new AdRequest.Builder() */
        /*     .AddTestDevice("68b93c58978d190e570640617c07e75a") */
        /*     .Build(); */
		AdRequest request = birthRequest();
        rewardBasedVideo.LoadAd( request, ad_id );
    }

    //リクエスト待ち時間を初期値に戻す
    private void initDelayedCallTime()
    {
        delayedCallTimeTableIndex = 0;
        delayedCallTimer = delayedCallTimeTable[delayedCallTimeTableIndex];
    }

    //次に失敗したときに再度リクエストをするまでの待ち時間を設定
    private void incrementDelayedCallTime()
    {
        delayedCallTimeTableIndex = UnityEngine.Mathf.Min( delayedCallTimeTableIndex + 1, delayedCallTimeTable.Length - 1 );
        delayedCallTimer = delayedCallTimeTable[delayedCallTimeTableIndex];
    }

    /* public void RequestRewardBasedVideo() */
    /* { */
    /*     Debug.Log( "RequestRewardBasedVideo" ); */
    /*     isRequestRewardVideo = true; */
    /*     incrementDelayedCallTime(); */
    /* } */

    public bool IsLoadedRewardBasedVideo()
    {
#if UNITY_EDITOR
        return true;
#else

#if ENABLE_UNITY_ADS
        return rewardBasedVideo.IsLoaded() || ( Advertisement.isSupported && Advertisement.IsReady( cUnityAdsPlacementId ) );
#else
        return rewardBasedVideo.IsLoaded();
#endif

#endif
    }

    public bool ShowRewardBasedVideo()
    {
        if( !isUseReward() ) return false;

#if UNITY_EDITOR
#if ENABLE_UNITY_ADS
        return showUnityAdsRewardBasedVideo();
#else
        HandleRewardBasedVideoRewarded(null,null);
        HandleRewardBasedVideoRewardedClosed(null,null);
        return true;
#endif
#else
        Debug.Log( "ShowRewardBasedVideo : " + rewardBasedVideo.IsLoaded() );

#if ENABLE_UNITY_ADS
        //for debug
        if( isUnityAdsOnly )
        {
            if( showUnityAdsRewardBasedVideo() )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
#endif

        if (rewardBasedVideo.IsLoaded())
        {
            isShowingRewardVideo = true;
            rewardBasedVideo.Show();
            return true;
        }
#if ENABLE_UNITY_ADS
        else if( showUnityAdsRewardBasedVideo() )
        {
            return true;
        }
#endif
        else
        {
            return false;
        }
#endif
    }

#if ENABLE_UNITY_ADS
    private bool showUnityAdsRewardBasedVideo()
    {
        if( Advertisement.isSupported && Advertisement.IsReady( cUnityAdsPlacementId ) )
        {
            Advertisement.Show();
            Action<ShowResult> callBack = (result) =>{
                if(result == ShowResult.Finished){
                    HandleRewardBasedVideoRewarded( null, null );
                    HandleRewardBasedVideoRewardedClosed( null, null );
                }
                else if(result == ShowResult.Failed){
                    //OnFailed();
                }
                else if(result == ShowResult.Skipped){
                    HandleRewardBasedVideoRewardedClosed( null, null );
                    //OnSkipped();
                }
            };

            //動画再生
            Advertisement.Show( cUnityAdsPlacementId, new ShowOptions {
                    resultCallback = callBack
                    });
            return true;
        }
        return false;
    }
#endif

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        Debug.Log( "HandleRewardBasedVideoLoaded" );

#if UNITY_EDITOR
        HandleRewardBasedVideoRewarded(null,null);
        HandleRewardBasedVideoRewardedClosed(null,null);
#endif
    }

    // 報酬受け渡し処理
    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        if( userCallbackOnReward != null )
        {
            userCallbackOnReward();
        }
    }

    // 動画広告リロード
    public void HandleRewardBasedVideoRewardedClosed(object sender, System.EventArgs args)
    {
        if( userCallbackOnRewardClose != null )
        {
            userCallbackOnRewardClose();
        }

        isShowingRewardVideo = false;
        //リクエスト待ち時間を初期値に戻す
        initDelayedCallTime();
    }

    // ロード失敗時
    public void HandleRewardBasedVideoRewardedFailed(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log( "Failed to load video reward : " + args.Message );
        /* if( Application.internetReachability == NetworkReachability.NotReachable ) return; */
        /* Invoke( "RequestRewardBasedVideo", 10.0f ); */
        /* RequestRewardBasedVideo(); */
    }

    protected void HandleRewardBasedVideoLeavingApplication(object sender, System.EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoLeavingApplication");
    }

	public void CloseCallback(object sender, System.EventArgs args)
    {
        /* Debug.Log( "CloseCallback" ); */
        interstitial.Destroy();
        RequestInterstitial();
        if( userCallbackOnCloseInterstitial != null )
        {
            userCallbackOnCloseInterstitial();
        }
    }

    public bool ShowInterstitialByRate()
    {
        interstitialRateCounter++;
        if ( ( interstitialRateCounter % interstitialRate ) == 0 )
        {
            return ShowInterstitial();
        }
        return false;
    }

    public bool ShowInterstitialByInterval()
    {
        /* Debug.Log( "adPlayCount : " + adPlayCount ); */
        /* if( adPlayCount >= maxInterstitialPlayCount ) */
        /* { */
        /*     return false; */
        /* } */

        float duration = Time.time - previousTime;
        /* Debug.Log( "duration : " + duration.ToString() ); */
        if ( duration > interstitialInterval )
        {
            return ShowInterstitial();
        }
        return false;
    }

    public bool ShowInterstitial()
    {
        previousTime = Time.time;
        interstitialRateCounter = 0;
        adPlayCount++;
        PlayerPrefs.SetInt( KeyAdPlayCount, adPlayCount );
        if( interstitial.IsLoaded()  )
        {
            interstitial.Show();
            return true;
        }
        else
        {
            Debug.Log( "interstitial is not ready yet" );
            return false;
        }
    }

    void OnApplicationPause( bool pause_status )
    {
        if( !pause_status )//resume時にリセット
        {
            if( isPastResetHoursFromPrevPlay() )
            {
                resetAdPlayCount();
            }
        }
    }
#endif
}

}
}
