using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using WK.Unity;
using WK.Audio;
using WK.Translate;

public class FinishedSceneManager : SingletonMonoBehaviour<FinishedSceneManager>{
    [SerializeField]
    protected SlideInSceneObjController sceneObj;

    [SerializeField]
    protected TypefaceAnimator animator;

    //------------------------------------------------------------------------------
    void Awake()
    {
        base.Awake();
    }

    //------------------------------------------------------------------------------
    void Start()
    {
        sceneObj.Init( false, false );
    }

    //------------------------------------------------------------------------------
	public void GoToResult() {
        Invoke( "goToResult", 3.0f );
    }

    //------------------------------------------------------------------------------
	private void goToResult() {
        Exit();
        GameSceneManager.Instance.GoToResult();
    }

    //------------------------------------------------------------------------------
    [ContextMenu("StartFinished")]
	public void StartFinished() {
        Enter();
        animator.Play();
        SoundManager.Instance.PlaySe( "finished" );
    }

    //------------------------------------------------------------------------------
    [ContextMenu("Enter")]
	public void Enter() {
        sceneObj.SlideIn();
    }

    //------------------------------------------------------------------------------
    [ContextMenu("Exit")]
	public void Exit() {
        sceneObj.SlideOut();
    }

    //------------------------------------------------------------------------------
    [ContextMenu("Dump")]
    void Dump()
    {
    }
}
