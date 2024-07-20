using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System;
using System.Collections;
using WK.Unity;
using WK.Audio;
using WK.Translate;
using WK.Collections;

public class DialogSceneManager : SingletonMonoBehaviour< DialogSceneManager > {
    [SerializeField]
	private SlideInSceneObjController scene;

    [SerializeField]
    private Text text;

    [SerializeField]
    private Text priceText;

    [SerializeField]
    private Button yesButton;

    [SerializeField]
    private Button noButton;

    [SerializeField]
    private GameObject restoreButton;

    [SerializeField]
    private GameObject notationButton;

    [SerializeField]
    private GameObject closeButton;

    [SerializeField]
    private Purchaser purchaser;

    //------------------------------------------------------------------------------
	void Start()
	{
        scene.Init( false, false );
        scene.gameObject.SetActive( false );

        restoreButton.SetActive( false );
    }

    //------------------------------------------------------------------------------
	public void ChangeToBePurchased()
    {
        yesButton.gameObject.SetActive( false );
        noButton.gameObject.SetActive( false );
        restoreButton.SetActive( false );
        notationButton.SetActive( true );
        closeButton.SetActive( true );
        priceText.gameObject.SetActive( false );
        text.text = WK.Translate.TranslateManager.Instance.GetText( "40009" );
    }

    //------------------------------------------------------------------------------
	public void EnterStore()
	{
        scene.gameObject.SetActive( true );
        Debug.Log( "IsSubscribedNoAds : " + GameSceneManager.Instance.IsSubscribedNoAds.ToString()
                      + "," + GameSceneManager.Instance.IsPurchasedNoAds.ToString() );
        //if( GameSceneManager.Instance.IsSubscribedNoAds || GameSceneManager.Instance.IsPurchasedNoAds )
        if( GameSceneManager.Instance.IsSubscribedNoAds )
        {
            ChangeToBePurchased();
        }
        else
        {
            yesButton.gameObject.SetActive( true );
            noButton.gameObject.SetActive( true );
#if UNITY_IOS
            restoreButton.SetActive( true );
#endif
            notationButton.SetActive( true );
            closeButton.SetActive( false );
            priceText.gameObject.SetActive( true );

            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener( () => BuyNoAds() );

            //text.text = WK.Translate.TranslateManager.Instance.GetText( "40000" );
            //priceText.text = purchaser.GetProductPrice( EStoreProduct.noAds );
            text.text = WK.Translate.TranslateManager.Instance.GetText( "ask_subscription" );
            priceText.text = purchaser.GetSubscriptionProductPrice( ESubscriptionProduct.noAds );
        }
        scene.SlideIn();
    }

    //------------------------------------------------------------------------------
	public void EnterConfirmQuit()
	{
        yesButton.gameObject.SetActive( true );
        noButton.gameObject.SetActive( true );

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener( () => QuitApplication() );
        text.text = WK.Translate.TranslateManager.Instance.GetText( "50000" );
        scene.gameObject.SetActive( true );
        restoreButton.SetActive( false );
        notationButton.SetActive( false );
        closeButton.SetActive( false );
        priceText.gameObject.SetActive( false );
        scene.SlideIn();
    }

    //------------------------------------------------------------------------------
	public void BuyNoAds()
    {
        //purchaser.Buy( EStoreProduct.noAds );
        purchaser.Subscribe( ESubscriptionProduct.noAds );
    }

    //------------------------------------------------------------------------------
	public void Restore()
    {
        purchaser.RestorePurchases();
    }

    //------------------------------------------------------------------------------
	public void QuitApplication()
    {
        Debug.Log( "Application.Quit()" );
        Application.Quit();
    }

    //------------------------------------------------------------------------------
	public void Exit( Action act )
	{
        scene.SlideOut().OnComplete( () => {
                if( act != null ) act();
                scene.gameObject.SetActive( false );
                } );
    }

}

