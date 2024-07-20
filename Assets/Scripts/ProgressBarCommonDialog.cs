using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WK {
namespace Unity {

public class ProgressBarCommonDialog : SingletonMonoBehaviour< ProgressBarCommonDialog > {
    [SerializeField]
    protected SlideInSceneObjController sceneObj;
    [SerializeField]
    protected Text text;
    [SerializeField]
    protected string seName;
    [SerializeField]
    protected GameObject yesButton;
    [SerializeField]
    protected GameObject noButton;
    [SerializeField]
    protected GameObject xButton;
    [SerializeField]
    protected ProgressBar bar;

    protected Action yesButtonCallback = null;
    protected Action noButtonCallback  = null;
    protected Action exitCompleteCallback = null;

    private AdsManager ads;

    void Start()
    {
        sceneObj.Init( false, false );

        ads = GameObject.Find("GameSceneManager").GetComponent<AdsManager>();
    }

    //------------------------------------------------------------------------------
    public void SetDialog( string message
            , Action yes_button_action
            , Action no_button_action
            , bool is_enable_bar
            )
    {
        text.text         = message;
        yesButtonCallback = yes_button_action;
        noButtonCallback  = no_button_action;
        bar.gameObject.SetActive( is_enable_bar );
    }

    //------------------------------------------------------------------------------
    [ContextMenu("EnterYesNoDialog")]
    public void EnterYesNoDialog()
    {
        Debug.Log( "EnterYesNoDialog" );
        yesButton.SetActive( true );
        noButton.SetActive( true );
        xButton.SetActive( false );
        if( bar.gameObject.activeSelf )
        {
            bar.SetBarSizeByRate( 1.0f );
            bar.SlideTo( 0.0f, noButtonCallback );
        }
        enterDialog();
    }

    //------------------------------------------------------------------------------
    private void enterDialog()
    {
        if( WK.Audio.SoundManager.Instance != null && seName != "" )
        {
            WK.Audio.SoundManager.Instance.PlaySe( seName );
        }

        sceneObj.SlideIn();
    }

    //------------------------------------------------------------------------------
    [ContextMenu("ExitDialog")]
    public void ExitDialog()
    {
        ExitDialog( true );
    }

    //------------------------------------------------------------------------------
    public void StopBar()
    {
        bar.StopSlide();
    }

    //------------------------------------------------------------------------------
    public void ResetBar()
    {
        bar.StopSlide();
        bar.SetBarSizeByRate( 1.0f );
        bar.SlideTo( 0.0f, noButtonCallback );
    }

    //------------------------------------------------------------------------------
    public void ExitDialog( bool is_enable_se )
    {
        if( WK.Audio.SoundManager.Instance != null && seName != "" && is_enable_se )
        {
            WK.Audio.SoundManager.Instance.PlaySe( seName );
        }

        yesButtonCallback = null;
        noButtonCallback = null;
        bar.StopSlide();

        sceneObj.SlideOut().OnComplete( () => {
                if( exitCompleteCallback != null )
                {
                    exitCompleteCallback();
                    exitCompleteCallback = null;
                } }
                );
    }

    //------------------------------------------------------------------------------
    public void SetExitCompleteCallback( Action complete_callback )
    {
        exitCompleteCallback = complete_callback;
    }

    //------------------------------------------------------------------------------
    public void YesButtonCallback()
    {
        if( yesButtonCallback != null )
        {
            ads.ShowAd();
            yesButtonCallback();
        }
    }

    /* //------------------------------------------------------------------------------ */
    public void NoButtonCallback()
    {
        if( noButtonCallback != null )
        {
            noButtonCallback();
        }
        else
        {
            ExitDialog();
        }
    }
}

}
}
