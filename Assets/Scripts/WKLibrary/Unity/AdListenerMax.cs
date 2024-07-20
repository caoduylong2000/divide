using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public static class DateTimeExtensions
{
    public static long ToUnixTimestamp(this DateTime d)
    {
        var epoch = d - new DateTime(1970, 1, 1, 0, 0, 0);

        return (long) epoch.TotalSeconds;
    }
}

namespace WK
{
    public class AdListenerMax : Unity.SingletonMonoBehaviour<AdListenerMax>
    {
        const string APPLOVIN_SDK_KEY =
            "CZbSb3oEtDrbVb6ewP3Tmd9odBvNvXP550Th3KxwTmV3MchG0lpnMzT-8RM2-n73ofmr_gmXRGaotH3u1teNqB";

        enum Platform
        {
            iOS,
            Android
        };

        Platform platform;

        string bannerAdUnitId
        {
            get { return bannerAdUnitIdArray[(int) platform]; }
        }

        [SerializeField] protected bool isMediationDebugger;

        [SerializeField] string[] bannerAdUnitIdArray;

        protected string interstitialAdUnitId
        {
            get { return interstitialAdUnitIdArray[(int) platform]; }
        }

        [SerializeField] protected string[] interstitialAdUnitIdArray;

        protected string rewardedAdUnitId
        {
            get { return rewardedAdUnitIdArray[(int) platform]; }
        }

        [SerializeField] protected string[] rewardedAdUnitIdArray;

        [SerializeField] protected int intersitialDuration;

        public Action userCallbackOnCloseInterstitial = null;
        public Action userCallbackOnReward = null;
        public Action userCallbackOnRewardClose = null;

        protected DateTime prevInterstitialTime;

        [SerializeField] private bool IsShowForcibly = false;

        public bool IsNoAds
        {
            get
            {
                if (Debug.isDebugBuild)
                {
                    if (IsShowForcibly)
                    {
                        return false;
                    }
                }

                return isNoAds;
            }
            set { isNoAds = value; }
        }

        [SerializeField] private bool isNoAds = false;

        void Awake()
        {
#if UNITY_IOS
            platform = Platform.iOS;
#elif UNITY_ANDROID
            platform = Platform.Android;
#endif
            if (Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.OSXEditor)
            {
                GetComponent<Image>().enabled = true;
            }
            else
            {
                GetComponent<Image>().enabled = false;
            }

            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
                // AppLovin SDK is initialized, start loading ads
            };

            MaxSdk.SetSdkKey(APPLOVIN_SDK_KEY);
            MaxSdk.InitializeSdk();

            if (Debug.isDebugBuild && isMediationDebugger)
            {
                MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
                {
                    // Show Mediation Debugger
                    MaxSdk.ShowMediationDebugger();
                };
            }

            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
                InitializeBannerAds();
                InitializeInterstitialAds();
                InitializeRewardedAds();
                ShowBanner();
            };

            prevInterstitialTime = DateTime.Now;
        }

        //------------------------------------------------------------------------------
        //------------------------------------------------------------------------------
        //------------------------------------------------------------------------------
        public void InitializeBannerAds()
        {
            MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.clear);
        }

        public void ShowBanner()
        {
            if (IsNoAds)
                return;
            MaxSdk.ShowBanner(bannerAdUnitId);
        }

        public void HideBanner()
        {
            MaxSdk.HideBanner(bannerAdUnitId);
        }

        //------------------------------------------------------------------------------
        //------------------------------------------------------------------------------
        //------------------------------------------------------------------------------
        public bool IsLoadedInterstitialVideo()
        {
#if UNITY_EDITOR
            return true;
#else
        return MaxSdk.IsInterstitialReady(interstitialAdUnitId);
#endif
        }

        //@todo 保存しないで、起動時にリセットされたほうが良いかも
        const string PREF_KEY_PREV_INTERSTITIAL_AD_DATE = "PREF_KEY_PREV_INTERSTITIAL_AD_DATE";

        //private DateTime getPrevInterstitialDate()
        private long getPrevInterstitialDate()
        {
            try
            {
                //var prev_date = PlayerPrefs.GetString( PREF_KEY_PREV_INTERSTITIAL_AD_DATE, "" );
                var prev_date = PlayerPrefs.GetString(PREF_KEY_PREV_INTERSTITIAL_AD_DATE, "");
                if (prev_date == "")
                {
                    //return new DateTime( 1970, 1, 1 ).ToUnixTimestamp();
                    return DateTime.Now.ToUnixTimestamp();
                }
                else
                {
                    //Debug.Log( "getPrevInterstitialDate : " + prev_date.ToString() );
                    //Debug.Log( "getPrevInterstitialDate : " + prev_date + "," + DateTime.ParseExact( prev_date, DATETIME_FORMAT ) );
                    //return DateTime.Parse( prev_date );

                    return Int64.Parse(prev_date);
                }
            }
            catch (Exception e)
            {
                //return new DateTime( 1970, 1, 1 );
                return DateTime.Now.ToUnixTimestamp();
            }
        }

        public void UpdatePrevInterstitialAdTiming()
        {
            //PlayerPrefs.SetString( PREF_KEY_PREV_INTERSTITIAL_AD_DATE, DateTime.Now.ToString() );
            //PlayerPrefs.SetString( PREF_KEY_PREV_INTERSTITIAL_AD_DATE, DateTime.Now.ToUnixTimestamp().ToString() );
            //Debug.Log( "UpdatePrevInterstitialAdTiming : " + DateTime.Now.ToUnixTimestamp().ToString() );

            prevInterstitialTime = DateTime.Now;
        }

        public bool IsIntersitialTiming()
        {
            //var span = DateTime.Now - getPrevInterstitialDate();
            //return span.TotalSeconds >= intersitialDuration;
            //var span = DateTime.Now.ToUnixTimestamp() - getPrevInterstitialDate();
            //return span >= intersitialDuration;
            //Debug.Log( "IsIntersitialTiming : " + span.ToString() );
            var span = DateTime.Now - prevInterstitialTime;
            return span.TotalSeconds >= intersitialDuration;
        }

        public bool ShowInterstitial()
        {
            if (!IsIntersitialTiming())
            {
                return false;
            }

            if (IsNoAds)
            {
                OnInterstitialDismissedEvent("");
                return true;
            }

            if (IsLoadedInterstitialVideo())
            {
                MaxSdk.ShowInterstitial(interstitialAdUnitId);
                UpdatePrevInterstitialAdTiming();
#if UNITY_EDITOR
                OnInterstitialDismissedEvent("");
#endif
                return true;
            }
            else
            {
                return false;
            }
        }

        public void InitializeInterstitialAds()
        {
            // Attach callback
            MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialDismissedEvent;

            // Load the first interstitial
            LoadInterstitial();
        }

        private void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(interstitialAdUnitId);
        }

        private void OnInterstitialLoadedEvent(string adUnitId)
        {
            // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
        }

        private void OnInterstitialFailedEvent(string adUnitId, int errorCode)
        {
            // Interstitial ad failed to load. We recommend re-trying in 3 seconds.
            Invoke("LoadInterstitial", 3);
            Debug.Log("OnInterstitialFailedEvent : " + adUnitId + errorCode);
        }

        private void InterstitialFailedToDisplayEvent(string adUnitId, int errorCode)
        {
            // Interstitial ad failed to display. We recommend loading the next ad
            LoadInterstitial();
            Debug.Log("InterstitialFailedToDisplayEvent: " + adUnitId + errorCode);
        }

        private void OnInterstitialDismissedEvent(string adUnitId)
        {
            if (userCallbackOnCloseInterstitial != null)
            {
                userCallbackOnCloseInterstitial();
            }

            // Interstitial ad is hidden. Pre-load the next ad
            LoadInterstitial();
        }

        //------------------------------------------------------------------------------
        //------------------------------------------------------------------------------
        //------------------------------------------------------------------------------
        public void InitializeRewardedAds()
        {
            // Attach callback
            MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.OnRewardedAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.OnRewardedAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.OnRewardedAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

            // Load the first RewardedAd
            LoadRewardedAd();
        }

        private void LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(rewardedAdUnitId);
        }

        private void OnRewardedAdLoadedEvent(string adUnitId)
        {
            // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
        }

        private void OnRewardedAdFailedEvent(string adUnitId, int errorCode)
        {
            // Rewarded ad failed to load. We recommend re-trying in 3 seconds.
            Invoke("LoadRewardedAd", 3);
            Debug.Log("OnRewardedAdFailedEvent : " + adUnitId + "," + errorCode);
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, int errorCode)
        {
            // Rewarded ad failed to display. We recommend loading the next ad
            LoadRewardedAd();
            Debug.Log("OnRewardedAdFailedToDisplayEvent : " + adUnitId + "," + errorCode);
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId)
        {
        }

        private void OnRewardedAdClickedEvent(string adUnitId)
        {
        }

        private void OnRewardedAdDismissedEvent(string adUnitId)
        {
            if (userCallbackOnRewardClose != null)
            {
                userCallbackOnRewardClose();
            }

            // Rewarded ad is hidden. Pre-load the next ad
            LoadRewardedAd();
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward)
        {
            // Rewarded ad was displayed and user should receive the reward
            if (userCallbackOnReward != null)
            {
                userCallbackOnReward();
            }
        }

        public bool IsLoadedRewardBasedVideo()
        {
#if UNITY_EDITOR
            return true;
#else
        return MaxSdk.IsRewardedAdReady(rewardedAdUnitId);
#endif
        }

        public bool ShowRewardBasedVideo()
        {
            if (IsNoAds)
            {
                Debug.Log("ShowRewardBasedVideo No Ads");
                OnRewardedAdReceivedRewardEvent("", new MaxSdk.Reward());
                OnRewardedAdDismissedEvent("");
                return true;
            }

#if UNITY_EDITOR
            OnRewardedAdReceivedRewardEvent("", new MaxSdk.Reward());
            OnRewardedAdDismissedEvent("");
            return true;
#else
        Debug.Log("ShowRewardBasedVideo " + IsLoadedRewardBasedVideo());
        if (IsLoadedRewardBasedVideo())
        {
            MaxSdk.ShowRewardedAd(rewardedAdUnitId);
            return true;
        }
        else
        {
            return false;
        }
#endif
        }
    }
}