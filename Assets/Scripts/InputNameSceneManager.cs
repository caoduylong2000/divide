using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using WK.Unity;
using WK.Audio;

public class InputNameSceneManager : SingletonMonoBehaviour<InputNameSceneManager>{
    [SerializeField]
    protected SlideInSceneObjController sceneObj;

    [SerializeField]
    protected InputField inputField;

    [SerializeField]
    protected Button confirmButton;

    Action confirmCallback = null;

    public string RankingName { get{ return PlayerPrefs.GetString( PrefKeys.RANKING_NAME, "" ); }
        protected set { PlayerPrefs.SetString( PrefKeys.RANKING_NAME, value ); } }
    protected override void Awake()
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
    }

    //------------------------------------------------------------------------------
    public void ObserveButtonInteractable()
    {
        if( inputField.text == "" )
        {
            confirmButton.interactable = false;
        }
        else
        {
            confirmButton.interactable = true;
        }
    }

    //------------------------------------------------------------------------------
    public void SetConfirmCalblack( Action callback )
    {
        confirmCallback = callback;
    }

    //------------------------------------------------------------------------------
    public void Confirm()
    {
        RankingName = inputField.text;

        if( confirmCallback != null )
        {
            confirmCallback();
            confirmCallback = null;
        }

        Exit();
    }

    //------------------------------------------------------------------------------
    [ContextMenu("Enter")]
	public void Enter() {
        SoundManager.Instance.PlaySe( "click" );
        sceneObj.SlideIn();
        inputField.text = RankingName;
        /* Debug.Log( "inputField.text : " + inputField.text ); */
        /* Debug.Log( "RankingName : " + RankingName ); */
        /* inputText.text = RankingName; */
        /* Debug.Log( "inputText.text : " + inputText.text ); */
        /* Debug.Log( "RankingName : " + RankingName ); */
        ObserveButtonInteractable();
    }

    //------------------------------------------------------------------------------
    [ContextMenu("Exit")]
	public void Exit() {
        SoundManager.Instance.PlaySe( "click" );
        sceneObj.SlideOut();
    }

    //------------------------------------------------------------------------------
	public bool IsNoRankingName() {
        return RankingName == "";
    }

    //------------------------------------------------------------------------------
    [ContextMenu("ClearName")]
	private void clearName() {
        RankingName = "";
    }
}
