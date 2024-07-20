using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using WK.Unity;
using WK.Audio;
using WK.Translate;

public class CountDownSceneManager : SingletonMonoBehaviour<CountDownSceneManager>{
    [SerializeField]
    protected SlideInSceneObjController sceneObj;

    [SerializeField]
    protected TypefaceAnimator animator;

    [SerializeField]
    protected Text text;

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
    void Update()
    {
        if( animator.progress < 0.33f )
        {
            text.text = "3"; 
        }
        else if( animator.progress < 0.66f )
        {
            text.text = "2"; 
        }
        else
        {
            text.text = "1"; 
        }
    }

    //------------------------------------------------------------------------------
    [ContextMenu("StartCountDown")]
	public void StartCountDown() {
        Enter();
        text.text = "3"; 
        SoundManager.Instance.PlaySe( "countDown" );
        animator.Play();
    }

    //------------------------------------------------------------------------------
	public void StartGame() {
        Exit();
        GameSceneManager.Instance.StartTimer();
    }

    //------------------------------------------------------------------------------
    [ContextMenu("Enter")]
	public void Enter() {
        sceneObj.SlideIn();
    }

    //------------------------------------------------------------------------------
    [ContextMenu("Exit")]
	public void Exit() {
        sceneObj.SlideOut().OnComplete( () => 
                {
                GameSceneManager.Instance.StartTimer();
                }
                );
    }

    //------------------------------------------------------------------------------
    [ContextMenu("Dump")]
    void Dump()
    {
    }
}
