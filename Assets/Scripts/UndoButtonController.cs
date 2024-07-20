using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UndoButtonController : MonoBehaviour
{
    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    Button button;

    [SerializeField]
    Text countText;

    [SerializeField]
    GameObject movieImage;

    bool finishedUndo = true;

    const int DEFAULT_UNDO_COUNT = 5;

    bool isEnable()
    {
        return PlayerPrefs.GetInt( PrefKeys.UNDO_COUNT, DEFAULT_UNDO_COUNT ) != 0;
    }

    void Start()
    {
        Invalidate();
    }

    public void FinishedUndo( bool is_finished )
    {
        finishedUndo = is_finished;
    }

    public void Invalidate()
    {
        bool is_enable = ResumeManager.Instance.IsEnableBack( 1 ) && !finishedUndo;
        canvasGroup.blocksRaycasts = is_enable;
        canvasGroup.alpha = is_enable ? 1.0f : 0.5f;

        //サブスク中なら無限大
        if( GameSceneManager.Instance.IsSubscribedNoAds )
        {
            PlayerPrefs.SetInt( PrefKeys.UNDO_COUNT, DEFAULT_UNDO_COUNT );
            countText.text = "∞";
            movieImage.SetActive( false );
            countText.gameObject.SetActive( true );
            return;
        }

        int count = PlayerPrefs.GetInt( PrefKeys.UNDO_COUNT, DEFAULT_UNDO_COUNT );
        movieImage.SetActive( count == 0 );
        countText.gameObject.SetActive( count != 0 );
        if( count != 0 )
        {
            countText.text = count.ToString();
        }
    }

    public void RecoverUndoCount()
    {
        PlayerPrefs.SetInt( PrefKeys.UNDO_COUNT, DEFAULT_UNDO_COUNT + 1 );
        Undo();
    }

    public void Undo()
    {
        int count = PlayerPrefs.GetInt( PrefKeys.UNDO_COUNT, DEFAULT_UNDO_COUNT );

        if( !ResumeManager.Instance.IsEnableBack( 1 ) )
        {
            Debug.Log( "Disable back" );
            return;
        }

        if( count == 0 )
        {
            RewardAdManager.Instance.ShowRewardedVideo(
                    () =>
                    {
                        RecoverUndoCount();
                        Invalidate();
                    }
            );
            return;
        }

        count--;
        PlayerPrefs.SetInt( PrefKeys.UNDO_COUNT, count );

        ResumeManager.Instance.ReadOldData( 1 );
        finishedUndo = true;
        WK.Audio.SoundManager.Instance.PlaySe("click");
        Invalidate();
    }
}
