#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
#define RECEIPT_VALIDATION
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
#if RECEIPT_VALIDATION
using UnityEngine.Purchasing.Security;
#endif
using System.Linq;
using WK.Translate;
using WK.Unity;

public enum EStoreProduct
{
    noAds,
    max
};

public enum ESubscriptionProduct
{
    noAds,
    max
};

public static class DictionaryExtensions
{
    public static V Get<K, V>(this Dictionary<K, V> self, K key, V defaultValue = default(V))
    {
        V value;
        return self.TryGetValue(key, out value) ? value : defaultValue;
    }

    public static string ToString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
    {
        return dictionary == null
            ? "null"
            : ("{" + string.Join(", ", dictionary.Select(kv => "'" + kv.Key + "': " + kv.Value).ToArray()) + "}");
    }
}

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class Purchaser : MonoBehaviour, IStoreListener
{
    [SerializeField] private string googlePublicKey;

    private IAppleExtensions m_AppleExtensions;
#pragma warning disable 0414
    private bool m_IsGooglePlayStoreSelected;
#pragma warning restore 0414

    private IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;

    private static IStoreController storeController = null; // The Unity Purchasing system.
    private static IExtensionProvider storeExtensionProvider = null; // The store-specific Purchasing subsystems.

#if UNITY_IOS
    public static string[] nonConsumableProductIDs = { "product_no_ads_divide" };
#else
    public static string[] nonConsumableProductIDs = {"product_no_ads"};
#endif

#if UNITY_ANDROID
    //色々とミスがあってiOSと名前が異なるようになってしまった。
    public static string[] subscriptionProductIDs = {"subscription_no_ads_divide_android"};
#else
    public static string[] subscriptionProductIDs = {"subscription_no_ads_divide"};
#endif


#if DEVELOPMENT_BUILD
    const int SUBSCRIPTION_MERGIN_SECONDS = 10;
#else
    //念の為Expireに1日のマージンを設ける
    const int SUBSCRIPTION_MERGIN_SECONDS = 24 * 60 * 60;
#endif

    SubscriptionManager subscriptionManager = null;

    [SerializeField] private UnityEvent undoButtonInvalidate;

    //------------------------------------------------------------------------------
    void Awake()
    {
        Debug.Log("Purchaser awake");
        InitializePurchasing();
    }

    public void InitializePurchasing()
    {
        if (storeController == null)
        {
            Debug.Log("initializePurchasing");
            initializePurchasing();
        }
    }

    //------------------------------------------------------------------------------
    private void initializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        // Unityの課金システム構築
        StandardPurchasingModule module = StandardPurchasingModule.Instance();
#if UNITY_EDITOR
        module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
#endif
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);


        foreach (var id in nonConsumableProductIDs)
        {
            builder.AddProduct(id, ProductType.NonConsumable);
        }

        foreach (var id in subscriptionProductIDs)
        {
            builder.AddProduct(id, ProductType.Subscription);
        }

        //it doesn't need anymore
        //builder.Configure<IGooglePlayConfiguration>().SetPublicKey(googlePublicKey);
        
        m_IsGooglePlayStoreSelected = Application.platform == RuntimePlatform.Android &&
                                      module.appStore == AppStore.GooglePlay;

        // 非同期の課金処理の初期化を開始
        UnityPurchasing.Initialize(this, builder);
    }

    //------------------------------------------------------------------------------
    public bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return storeController != null && storeExtensionProvider != null;
    }

    //------------------------------------------------------------------------------
    public void Buy(EStoreProduct p)
    {
        Debug.Log("Buy" + p.ToString());
        buyProductID(nonConsumableProductIDs[(int) p]);
    }

    //------------------------------------------------------------------------------
    public void Subscribe(ESubscriptionProduct p)
    {
        Debug.Log("Subscribe" + p.ToString());
        buyProductID(subscriptionProductIDs[(int) p]);
    }

    //------------------------------------------------------------------------------
    public bool IsPurchased(EStoreProduct p)
    {
        if (storeController == null) return false;

        Product product = storeController.products.WithID(nonConsumableProductIDs[(int) p]);
        return product.hasReceipt;
    }

    //------------------------------------------------------------------------------
    public string GetProductCurrencyCode(EStoreProduct p)
    {
        if (storeController == null) return "can't get price";

        Product product = storeController.products.WithID(nonConsumableProductIDs[(int) p]);
        return product.metadata.isoCurrencyCode;
    }

    //------------------------------------------------------------------------------
    public float GetProductPriceFloat(EStoreProduct p)
    {
        if (storeController == null) return 0.0f;

        Product product = storeController.products.WithID(nonConsumableProductIDs[(int) p]);
        return (float) product.metadata.localizedPrice;
    }

    //------------------------------------------------------------------------------
    public string GetProductPrice(EStoreProduct p)
    {
        if (storeController == null) return "can't get price";

        Product product = storeController.products.WithID(nonConsumableProductIDs[(int) p]);
        return product.metadata.localizedPriceString;
    }

    //------------------------------------------------------------------------------
    public string GetSubscriptionProductPrice(ESubscriptionProduct p)
    {
        if (storeController == null) return "can't get price";

        Product product = storeController.products.WithID(subscriptionProductIDs[(int) p]);
        return product.metadata.localizedPriceString;
    }

    //------------------------------------------------------------------------------
    private void buyProductID(string productId)
    {
        Debug.Log("BuyProduct " + productId);
        // 初期化されていない
        if (!IsInitialized())
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
            return;
        }

        Product product = storeController.products.WithID(productId);
        // 購入できないアイテムの場合
        if (product == null || !product.availableToPurchase)
        {
            CommonDialogManager.Instance.SetDialog(
                TranslateManager.Instance.GetText("40006")
                , null
            );
            CommonDialogManager.Instance.EnterNotationDialog();
            Debug.Log("The product isn't available.");
            return;
        }

        // 通信不可の場合は何もしない（初期化は必ず終了している）
        if (!hasNetworkConnection())
        {
            CommonDialogManager.Instance.SetDialog(
                TranslateManager.Instance.GetText("40007")
                , null
            );
            CommonDialogManager.Instance.EnterNotationDialog();
            Debug.Log("No Network.");
            return;
        }

        CommonDialogManager.Instance.SetDialog(
            TranslateManager.Instance.GetText("40005")
            , null
        );
        CommonDialogManager.Instance.EnterLoadingDialog();

        storeController.InitiatePurchase(product);
    }

    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google.
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized." + storeController + "," + storeExtensionProvider);
            return;
        }

        // 通信不可の場合は何もしない
        if (!hasNetworkConnection())
        {
            Debug.Log("No Network Connection");
            return;
        }

        // If we are running on an Apple device ...
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            /* var apple = storeExtensionProvider.GetExtension<IAppleExtensions>(); */
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            m_AppleExtensions.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result +
                          ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    //------------------------------------------------------------------------------
    private static bool hasNetworkConnection()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    //------------------------------------------------------------------------------
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
        m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);

        Debug.Log("OnInitialized: PASS");

        storeController = controller;
        storeExtensionProvider = extensions;

        m_GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();

        Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();

        foreach (var item in controller.products.all)
        {
            if (item.availableToPurchase)
            {
                Debug.Log(string.Join(" - ",
                    new[]
                    {
                        item.metadata.localizedTitle,
                        item.transactionID,
                        item.receipt
                    }));
                if (item.definition.type == ProductType.Subscription)
                {
                    if (item.receipt != null)
                    {
                        subsribeImpl(item);
                    }
                    else
                    {
                        //レシートが切れている = 期限切れ
                        PlayerPrefs.SetInt(PrefKeys.EXPIRE_SEC_SUBSCRIPTION_NO_ADS, 0);
                        Debug.Log("the product should have a valid receipt");
                    }
                }
                else
                {
                    Debug.Log("the product is not a subscription product");
                }
            }
        }
    }

    //------------------------------------------------------------------------------
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);

        //しっかりやる場合、初期化にミスったら再起動を促す
    }

    public void OnInitializeFailed(InitializationFailureReason error, string msgError)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);

        //しっかりやる場合、初期化にミスったら再起動を促す
    }

    //------------------------------------------------------------------------------
    //@memo Restore時にも処理が走る
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log("Purchase Process");
#if RECEIPT_VALIDATION
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
        // Local validation is available for GooglePlay and Apple stores
        if (m_IsGooglePlayStoreSelected ||
            Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer ||
            Application.platform == RuntimePlatform.tvOS)
        {
            try
            {
                var result = validator.Validate(args.purchasedProduct.receipt);
                Debug.Log("Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    Debug.Log(productReceipt.productID);
                    Debug.Log(productReceipt.purchaseDate);
                    Debug.Log(productReceipt.transactionID);

                    GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                    if (null != google)
                    {
                        Debug.Log(google.purchaseState);
                        Debug.Log(google.purchaseToken);
                    }

                    AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                    if (null != apple)
                    {
                        Debug.Log(apple.originalTransactionIdentifier);
                        Debug.Log(apple.subscriptionExpirationDate);
                        Debug.Log(apple.cancellationDate);
                        Debug.Log(apple.quantity);
                    }
                }
            }
            catch (IAPSecurityException)
            {
                Debug.Log("Invalid receipt, not unlocking content");
                CommonDialogManager.Instance.SetDialog(
                    TranslateManager.Instance.GetText("40008")
                    , null
                );
                CommonDialogManager.Instance.EnterNotationDialog();
                return PurchaseProcessingResult.Complete;
            }
        }
#endif

        if (String.Equals(args.purchasedProduct.definition.id, nonConsumableProductIDs[(int) EStoreProduct.noAds],
            StringComparison.Ordinal))
        {
            GameSceneManager.Instance.SetPurchasedNoAds();
            DialogSceneManager.Instance.ChangeToBePurchased();
            /* StoreSceneManager.Instance.SetPurchasedNoAds(); */
            WK.AdListenerMax.Instance.HideBanner();

            //AdjustManager.Instance.SendPurchaseEvent( GetProductPriceFloat(EStoreProduct.noAds), GetProductCurrencyCode(EStoreProduct.noAds), args.purchasedProduct.transactionID );

            CommonDialogManager.Instance.ExitDialog();
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));


            /* foreach( var level in  GameVariables.Instance.paymentPackLevel[i] ) */
            /* { */
            /*     GameVariables.Instance.SetLevelState( level, ELevelState.unlocked ); */
            /* } */
        }
        else if (String.Equals(args.purchasedProduct.definition.id,
            subscriptionProductIDs[(int) ESubscriptionProduct.noAds], StringComparison.Ordinal))
        {
            Product item = args.purchasedProduct;
            Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();
            Debug.Log("item.receipt : " + item.receipt == null ? "null" : item.receipt);
            if (item.receipt != null)
            {
                subsribeImpl(item);
                SaveData.Data.purchasedSubscriptionCount++;
                SaveData.SaveMain();
            }

            WK.AdListenerMax.Instance.HideBanner();
            undoButtonInvalidate.Invoke();
            DialogSceneManager.Instance.ChangeToBePurchased();
            CommonDialogManager.Instance.ExitDialog();
        }

        /* for(int i = 0; i < productIDs.Length; ++i ) { */
        /*     var id = productIDs[i]; */
        /* } */

        // Return a flag indicating whether this product has completely been received, or if the application needs
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still
        // saving purchased products to the cloud, and when that save is delayed.
        return PurchaseProcessingResult.Complete;
    }

    void subsribeImpl(Product item)
    {
        Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();
        Debug.Log("item.receipt : " + item.receipt == null ? "null" : item.receipt);
        if (item.receipt != null)
        {
            if (checkIfProductIsAvailableForSubscriptionManager(item.receipt))
            {
                string intro_json =
                    (introductory_info_dict == null ||
                     !introductory_info_dict.ContainsKey(item.definition.storeSpecificId))
                        ? String.Empty
                        : introductory_info_dict[item.definition.storeSpecificId];
                subscriptionManager = new SubscriptionManager(item, intro_json);
                SubscriptionInfo info = subscriptionManager.getSubscriptionInfo();
                debugLogSubscriptionProduct(info);
                DateTime expire_date = info.getExpireDate();
                
                //lay console側では5分でexpireして再更新されているが、
                //ここで取得できる情報としては、expire dateは1ヶ月後になるし
                //purchase dateも変わらない模様
#if DEVELOPMENT_BUILD
                expire_date = info.getPurchaseDate().AddMinutes(5);
#endif
                int subscription_duration =
                    (int) ((expire_date - GameSceneManager.BASE_TIME).TotalSeconds +
                           SUBSCRIPTION_MERGIN_SECONDS);
                PlayerPrefs.SetInt(PrefKeys.EXPIRE_SEC_SUBSCRIPTION_NO_ADS, subscription_duration);

                if (Result.True == info.isSubscribed())
                {
                    if (Result.True == info.isCancelled())
                    {
                        Debug.Log("Subscription Canceled");
                    }
                    else
                    {
                    }
                }
                else if (Result.True == info.isExpired())
                {
                    Debug.Log("Subscription Expired");
                }
            }
            else
            {
                //ここにきたらあかん
                //念の為subscription duration設定しておく
                DateTime expire_date = DateTime.UtcNow;
#if DEVELOPMENT_BUILD
                expire_date = expire_date.AddMinutes(5);
#else
                    expire_date = expire_date.AddMonths(1);
#endif
                int subscription_duration =
                    (int) ((expire_date - GameSceneManager.BASE_TIME).TotalSeconds +
                           SUBSCRIPTION_MERGIN_SECONDS);
                PlayerPrefs.SetInt(PrefKeys.EXPIRE_SEC_SUBSCRIPTION_NO_ADS, subscription_duration);

                Debug.Log(
                    "This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
            }
        }
    }

    void debugLogSubscriptionProduct(SubscriptionInfo info)
    {
        Debug.Log("product id is: " + info.getProductId());
        Debug.Log("purchase date is: " + info.getPurchaseDate());
        Debug.Log("subscription next billing date is: " + info.getExpireDate().ToString("G"));
        Debug.Log("is subscribed? " + info.isSubscribed().ToString());
        Debug.Log("is expired? " + info.isExpired().ToString());
        Debug.Log("is cancelled? " + info.isCancelled());
        Debug.Log("product is in free trial peroid? " + info.isFreeTrial());
        Debug.Log("product is auto renewing? " + info.isAutoRenewing());
        Debug.Log("subscription remaining valid time until next billing date is: " +
                  info.getRemainingTime().TotalSeconds.ToString());
        Debug.Log("is this product in introductory price period? " +
                  info.isIntroductoryPricePeriod());
        Debug.Log("the product introductory localized price is: " +
                  info.getIntroductoryPrice());
        Debug.Log("the product introductory price period is: " +
                  info.getIntroductoryPricePeriod());
        Debug.Log("the number of product introductory price period cycles is: " +
                  info.getIntroductoryPricePeriodCycles());
        string debug_log = "";
        debug_log += "Expire secs:" + PlayerPrefs.GetInt(PrefKeys.EXPIRE_SEC_SUBSCRIPTION_NO_ADS, 0);
        debug_log += "\n" + "IsSubscribedNoAds:" + GameSceneManager.Instance.IsSubscribedNoAds.ToString();
        debug_log += "\n" + "Now secs:" +
                     (System.DateTime.UtcNow - GameSceneManager.BASE_TIME).TotalSeconds.ToString();
        Debug.Log(debug_log);
    }

    //------------------------------------------------------------------------------
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // 初期化されていない
        if (!IsInitialized())
        {
            Debug.Log("OnPurchaseFailed. Not initialized.");
            return;
        }

        //@memo
        //UnityIAPのバグっぽい。Androidで初期化時に
        //OnPurchaseFailedを通る
        //http://answers.unity3d.com/questions/1332076/unity-in-app-purchases-non-consumable-duplicate-tr.html
        if (GameSceneManager.Instance.IsPurchasedNoAds ||
            GameSceneManager.Instance.IsSubscribedNoAds)
        {
            return;
        }

        CommonDialogManager.Instance.SetDialog(
            TranslateManager.Instance.GetText("40008")
            , null
        );
        CommonDialogManager.Instance.EnterNotationDialog();

        Debug.Log("Purchase Failed storeSpecificId : " + product.definition.storeSpecificId);
        if (String.Equals(product.definition.storeSpecificId,
            subscriptionProductIDs[(int) ESubscriptionProduct.noAds], StringComparison.Ordinal))
        {
            Debug.Log("Purchase Failed failureReason : " + failureReason);
            if (failureReason == PurchaseFailureReason.DuplicateTransaction) //既に購入済み
            {
                Debug.Log("Purchase Failed subscribe Again");
                subsribeImpl(product);
                WK.AdListenerMax.Instance.HideBanner();
                undoButtonInvalidate.Invoke();
            }
        }

        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
            product.definition.storeSpecificId, failureReason));
    }

    /// <summary>
    /// iOS Specific.
    /// This is called as part of Apple's 'Ask to buy' functionality,
    /// when a purchase is requested by a minor and referred to a parent
    /// for approval.
    ///
    /// When the purchase is approved or rejected, the normal purchase events
    /// will fire.
    /// </summary>
    /// <param name="item">Item.</param>
    private void OnDeferred(Product item)
    {
        Debug.Log("Purchase deferred: " + item.definition.id);
    }

    private bool checkIfProductIsAvailableForSubscriptionManager(string receipt)
    {
        var receipt_wrapper = (Dictionary<string, object>) MiniJson.JsonDecode(receipt);
        if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
        {
            Debug.Log("The product receipt does not contain enough information");
            return false;
        }

        var store = (string) receipt_wrapper["Store"];
        var payload = (string) receipt_wrapper["Payload"];

        if (payload != null)
        {
            switch (store)
            {
                case GooglePlay.Name:
                {
                    var payload_wrapper = (Dictionary<string, object>) MiniJson.JsonDecode(payload);
                    if (!payload_wrapper.ContainsKey("json"))
                    {
                        Debug.Log(
                            "The product receipt does not contain enough information, the 'json' field is missing");
                        return false;
                    }

                    var original_json_payload_wrapper =
                        (Dictionary<string, object>) MiniJson.JsonDecode((string) payload_wrapper["json"]);
                    if (original_json_payload_wrapper == null ||
                        !original_json_payload_wrapper.ContainsKey("developerPayload"))
                    {
                        Debug.Log(
                            "The product receipt does not contain enough information, the 'developerPayload' field is missing");
                        return false;
                    }

                    var developerPayloadJSON = (string) original_json_payload_wrapper["developerPayload"];
                    var developerPayload_wrapper =
                        (Dictionary<string, object>) MiniJson.JsonDecode(developerPayloadJSON);
                    if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") ||
                        !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                    {
                        Debug.Log(
                            "The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                        return false;
                    }

                    return true;
                }
                case AppleAppStore.Name:
                case AmazonApps.Name:
                case MacAppStore.Name:
                {
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }

        return false;
    }
}