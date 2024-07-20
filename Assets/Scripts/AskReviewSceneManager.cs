using UnityEngine;
using System.Collections;
using WK.Unity;
using System.Runtime.InteropServices;

[RequireComponent(typeof(SlideInSceneObjController))]
public class AskReviewSceneManager : SingletonMonoBehaviour<AskReviewSceneManager> {
    SlideInSceneObjController sceneObjCtrl;

	private const string cIsNeverReviewKey = "isNeverReview";
    private bool isNever = false;
    private bool IsNever { get { return isNever; } }
    private const int cAskReviewCount = 4;
    private int askReviewCounter = 0;

#if UNITY_IOS
    [DllImport ("__Internal")]
    private static extern float ShowReviewDialog();
#endif

	void Start () {
        sceneObjCtrl = GetComponent<SlideInSceneObjController>();
        sceneObjCtrl.Init( false, false );
        isNever = PlayerPrefs.GetInt(cIsNeverReviewKey, 0) == 1;
        askReviewCounter = 0;
	}

	public void IncrementAskCounter()
    {
        askReviewCounter++;
    }

	public bool IsAskTiming()
    {
        /* Debug.Log( "askReviewCounter : " + askReviewCounter ); */
#if UNITY_EDITOR
        if( GameSceneManager.Instance.isDebugAskReview ) return true ;
#endif
        if( isNever ) return false ;
        if( askReviewCounter >= cAskReviewCount )
        {
            return true;
        }
        return false;
    }

	public void AskToReview () {
        sceneObjCtrl.SlideIn();
	}

	public void Later () {
        askReviewCounter = 0;
        sceneObjCtrl.SlideOut();
	}

	public void GoToReview () {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + Config.androidAppId);
#elif UNITY_IOS
        ShowReviewDialog();
#endif
        PlayerPrefs.SetInt(cIsNeverReviewKey, 1);
        askReviewCounter = 0;
        isNever = true;
        sceneObjCtrl.SlideOut();
	}

	public void NeverAsk () {
        PlayerPrefs.SetInt(cIsNeverReviewKey, 1);
        isNever = true;
        sceneObjCtrl.SlideOut();
	}
}
