using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBarController : MonoBehaviour {
    [SerializeField]
    RectTransform timeBar;

    [SerializeField]
    Text timeText;

    [SerializeField]
    float duration;

    [SerializeField]
    WK.CurveAnimator animator;

    [SerializeField]
    GameObject bonusEffect;

    const float cBlinkThreshold = 10.0f;

    protected enum EState {
        sleep
        ,pause
        ,idle
        ,finished
    }

    protected EState currState    = 0;
    protected EState nextState    = 0;
    protected int    stateCounter = 0;
    protected float  stateTimer   = 0;

    protected float timer = 0.0f;
    protected float defaultLength;

    public bool IsFinished { get { return currState == EState.finished; } }

    //------------------------------------------------------------------------------
    void Awake()
    {
        stateCounter = 0;
        stateTimer   = 0.0f;
        currState    = EState.sleep;
        nextState    = EState.sleep;

        defaultLength = timeBar.sizeDelta.x;
    }

    //------------------------------------------------------------------------------
    void Update()
    {
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
            case EState.sleep:
                updateSleep();
                break;
            case EState.pause:
                break;
            case EState.idle:
                updateIdle();
                break;
            case EState.finished:
                break;
        }
    }

    //------------------------------------------------------------------------------
    private void changeState( EState next_state )
    {
        nextState = next_state;
    }

    //------------------------------------------------------------------------------
    private void setText()
    {
        int minute = (int)( timer / 60.0f );
        int sec = (int)( timer - 60.0f * minute );
        timeText.text = minute.ToString() + ":" + sec.ToString("00");
    }

    //------------------------------------------------------------------------------
    private void setBar()
    {
        var rate = timer / duration;
        var size = timeBar.sizeDelta;

        /* Debug.Log( "rate : " + rate ); */
        size.x = defaultLength * rate;
        timeBar.sizeDelta = size;
    }

    //------------------------------------------------------------------------------
    private void updateSleep()
    {
        timer = duration;
        setText();
        setBar();
        animator.SetFrame( 0.0f );
        animator.Stop();
    }

    //------------------------------------------------------------------------------
    /* private void updatePause() */
    /* { */
    /* } */

    //------------------------------------------------------------------------------
    private void updateIdle()
    {
        /* Debug.Log( "timer : " + timer ); */
        timer -= Time.deltaTime;
        timer = Mathf.Max( 0.0f, timer );
        setText();
        setBar();
        if( timer == 0.0f )
        {
            changeState( EState.finished );
        }

        if( ( timer <=  cBlinkThreshold ) && !animator.IsPlaying )
        {
            animator.Play();
        }
    }

    //------------------------------------------------------------------------------
    public void StartTimer()
    {
        changeState( EState.idle );
    }

    //------------------------------------------------------------------------------
    public void PauseTimer()
    {
        changeState( EState.pause );
    }

    //------------------------------------------------------------------------------
    public void ResumeTimer()
    {
        changeState( EState.idle );
    }

    //------------------------------------------------------------------------------
    public void BonusTimer(float bonus)
    {
        timer += bonus;
        bonusEffect.SetActive(true);
        Instantiate(bonusEffect, transform.position, Quaternion.identity);
        setText();
        setBar();
        
    }

    //------------------------------------------------------------------------------
    public void ResetTimer()
    {
        changeState( EState.sleep );
    }

    //------------------------------------------------------------------------------
    /* private void updateFinished() */
    /* { */
    /* } */

}
