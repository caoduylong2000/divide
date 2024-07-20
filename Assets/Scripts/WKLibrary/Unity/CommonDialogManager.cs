using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WK {
namespace Unity {

public class CommonDialogManager : SingletonMonoBehaviour< CommonDialogManager > {
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
    protected GameObject loadingObj;
    [SerializeField]
    protected GameObject xButton;

    protected Action yesButtonCallback = null;
    protected Action noButtonCallback  = null;
    protected Action exitCompleteCallback = null;

    void Start()
    {
        sceneObj.Init( false, false );
    }

    //------------------------------------------------------------------------------
    public void SetDialog( string message, Action yes_button_action, Action no_button_action = null )
    {
        text.text         = message;
        yesButtonCallback = yes_button_action;
        noButtonCallback  = no_button_action;
    }

    //------------------------------------------------------------------------------
    [ContextMenu("EnterLoadingDialog")]
    public void EnterLoadingDialog()
    {
        yesButton.SetActive( false );
        noButton.SetActive( false );
        loadingObj.SetActive( true );
        xButton.SetActive( false );
        enterDialog();
    }

    //------------------------------------------------------------------------------
    [ContextMenu("EnterNotationDialog")]
    public void EnterNotationDialog()
    {
        yesButton.SetActive( false );
        noButton.SetActive( false );
        loadingObj.SetActive( false );
        xButton.SetActive( true );

        //@memo LayoutRebuilder.MarkLayoutForRebuild ではrebuildされなかった...
        xButton.transform.localPosition = xButton.transform.localPosition + new Vector3( 1.0f, 0.0f, 0.0f );
        xButton.transform.localPosition = xButton.transform.localPosition - new Vector3( 1.0f, 0.0f, 0.0f );
        enterDialog();
    }

    //------------------------------------------------------------------------------
    [ContextMenu("EnterYesNoDialog")]
    public void EnterYesNoDialog()
    {
        Debug.Log( "EnterYesNoDialog" );
        yesButton.SetActive( true );
        noButton.SetActive( true );
        loadingObj.SetActive( false );
        xButton.SetActive( false );
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
    public void ExitDialog( bool is_enable_se )
    {
        if( WK.Audio.SoundManager.Instance != null && seName != "" && is_enable_se )
        {
            WK.Audio.SoundManager.Instance.PlaySe( seName );
        }

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
