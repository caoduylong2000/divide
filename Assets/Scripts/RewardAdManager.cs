using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardAdManager : WK.Unity.SingletonMonoBehaviour<RewardAdManager> {
    protected enum EState {
        wait
        , showMovie
    }

    protected bool isRequestReward = false;
    protected DateTime adStartedTime;

    protected Action rewardCallback;

    protected EState currState    = 0;
    protected EState nextState    = 0;
    protected int    stateCounter = 0;
    protected float  stateTimer   = 0;

	void Start () {
        stateCounter = -1;//@memo waitのstateCounter == 0を通すため
        stateTimer   = 0.0f;
        currState    = EState.wait;
        nextState    = EState.wait;
	}

	void Update () {
        ++stateCounter;
        stateTimer += Time.deltaTime;
        if( currState != nextState )
        {
            currState    = nextState;
            stateCounter = 0;
            stateTimer   = 0.0f;
        }

        switch( currState )
        {
            case EState.wait:
                updateWait();
                break;
            case EState.showMovie:
                updateShowMovie();
                break;
        }
	}

    private void changeState( EState next_state )
    {
        nextState = next_state;
    }

    private void updateWait()
    {
    }

    private void updateShowMovie()
    {
        if( isRequestReward )
        {
            execReward();
            WK.AdListenerMax.Instance.UpdatePrevInterstitialAdTiming();
            isRequestReward = false;
            changeState( EState.wait );
        }
    }

    [ContextMenu("Exec Reward")]
    private void execReward()
    {
        Debug.Log( "exec Reward" );
        rewardCallback();
    }

    private void showRewardVideo()
    {
        bool has_seen_video = false;

        WK.AdListenerMax.Instance.userCallbackOnReward = () =>
        {
            has_seen_video = true;
        };
        WK.AdListenerMax.Instance.userCallbackOnRewardClose = () => {
            if( has_seen_video )
            {
                isRequestReward = true;
            }
            else
            {
                ////たまに広告を見てもリワードがもらえないことがあるバグ対策
                //if( !isRequestReward )
                //{
                //    var span = DateTime.Now - adStartedTime;
                //    const float cSaftyNetTime = 15.0f;
                //    if( span.TotalSeconds > cSaftyNetTime )
                //    {
                //        Debug.Log( "SaftyNet launched!!!" );
                //        isRequestReward = true;
                //    }
                //}

                changeState( EState.wait );
            }
            //@memo ここで音を鳴らすとMainthreadじゃないので不味い
            /* WK.CommonDialogManager.Instance.ExitDialog( false ); */
        };

        bool is_successed = WK.AdListenerMax.Instance.ShowRewardBasedVideo();
        if( is_successed )
        {
            adStartedTime = DateTime.Now;
            changeState( EState.showMovie );
        }
        else
        {
            //string message = WK.TranslateManager.Instance.GetText( "6202" );
            //WK.CommonDialogManager.Instance.SetDialog( message, null );
            //WK.CommonDialogManager.Instance.EnterNotationDialog();
            changeState( EState.wait );
        }
    }

	public void ShowRewardedVideo( Action reward_callback ) {
        rewardCallback = reward_callback;
        showRewardVideo();
    }
}
