using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System;
using System.Collections;
using WK.Unity;
using WK.Audio;
using WK.Translate;
using WK.Collections;
using Sirenix.OdinInspector;

public class LicenseSceneManager : SingletonMonoBehaviour< LicenseSceneManager > {
    [SerializeField]
	private SlideInSceneObjController scene;

    //------------------------------------------------------------------------------
	void Start()
	{
        scene.Init( false, false );
        scene.gameObject.SetActive( false );
    }

    //------------------------------------------------------------------------------
    [Button]
	public void Enter()
	{
        scene.gameObject.SetActive( true );
        scene.SlideIn();
    }

    //------------------------------------------------------------------------------
    [Button]
	public void Exit()
	{
        scene.SlideOut().OnComplete( () => {
                scene.gameObject.SetActive( false );
                } );
    }

}

