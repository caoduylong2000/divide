using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using WK.Unity;
using WK.Audio;
using WK.Translate;

public class TitleSceneManager : SingletonMonoBehaviour<TitleSceneManager>{
    [SerializeField]
    protected SlideInSceneObjController sceneObj;

    //------------------------------------------------------------------------------
    void Awake()
    {
        base.Awake();
    }

    //------------------------------------------------------------------------------
    void Start()
    {
        sceneObj.Init( true, true );
    }

    //------------------------------------------------------------------------------
    void Update()
    {
    }

    //------------------------------------------------------------------------------
	public void GoToNormalMode() {
        GoToNormalMode( true );
    }

    //------------------------------------------------------------------------------
	public void GoToFourByFourMode() {
        GoToFourByFourMode( true );
    }

    //------------------------------------------------------------------------------
	public void GoToNormalMode( bool is_enable_se ) {
        Exit( is_enable_se );
        GameSceneManager.Instance.GameMode = EGameMode.three;
        GameSceneManager.Instance.IsTimeLimit = false;
        GameSceneManager.Instance.Restart();
    }

    //------------------------------------------------------------------------------
	public void GoToFourByFourMode( bool is_enable_se ) {
        Exit( is_enable_se );
        GameSceneManager.Instance.GameMode = EGameMode.four;
        GameSceneManager.Instance.IsTimeLimit = false;
        GameSceneManager.Instance.Restart();
    }

    //------------------------------------------------------------------------------
	public void GoToTimeLimitMode() {
        Exit();
        GameSceneManager.Instance.GameMode = EGameMode.three;
        GameSceneManager.Instance.IsTimeLimit = true;
        GameSceneManager.Instance.Restart();

        /* GameSceneManager.Instance.IsTimeLimit = true; */
        /* GameSceneManager.Instance.Restart(); */
    }

    //------------------------------------------------------------------------------
    [ContextMenu("Enter")]
	public void Enter() {
        Enter( null );
    }

    //------------------------------------------------------------------------------
	public void Enter( Action complete_callback ) {
        SoundManager.Instance.PlaySe( "click" );
        if( complete_callback != null )
        {
            sceneObj.SlideIn().OnComplete(
                    () => complete_callback()
                    );
        }
        else
        {
            sceneObj.SlideIn();
        }
    }

    //------------------------------------------------------------------------------
    [ContextMenu("Exit")]
	public void Exit() {
        Exit( true );
    }

    //------------------------------------------------------------------------------
	public void Exit( bool is_enable_se ) {
        if( is_enable_se )
        {
            SoundManager.Instance.PlaySe( "click" );
        }
        sceneObj.SlideOut();
    }

    //------------------------------------------------------------------------------
    [ContextMenu("Dump")]
    void Dump()
    {
    }
}
