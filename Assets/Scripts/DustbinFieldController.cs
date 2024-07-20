using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using WK.Unity;
using Sirenix.OdinInspector;

// @memo KeepFieldControllerを継承しようとしたが、うまく行かなかった。
// DustbinFieldController.Awake does not have a method body
// などと言われる...
// bodyあるつもりなのに...
public class DustbinFieldController : SingletonMonoBehaviour<DustbinFieldController> {
    [SerializeField]
    private GameObject failedObj;

    private CanvasGroup canvasGroup;
    private Action failedCallback;

    public float slideInSpeed = 8.0f;
    private int state         = 0;
    private int nextState     = 0;
    private int stateCounter  = 0;
    private float stateTimer  = 0;

    private const int cStateIdle      = 0;
    private const int cStateAppear    = 1;
    private const int cStateDisappear = 2;
    private const int cStateFailed    = 3;

    private int nextDustbinEnableCount      = 0;
    public int NextDustbinEnableCount { get { return nextDustbinEnableCount; } set { nextDustbinEnableCount = value; } }
    public const int cFirstNextDustbinEnableCount = 30;
    public const int cNextDustbinAmount           = 50;

    public float appearX     = 240.0f;
    public float disappearX  = 500.0f;
    private bool  isAppearing = false;

    /* NetworkReachability currReachability; */

    //------------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();

        stateCounter = 0;
        stateTimer   = 0.0f;
        state        = cStateIdle;
        nextState    = cStateIdle;
        canvasGroup  = GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;

        failedObj.SetActive( false );
        /* currReachability = Application.internetReachability; */

        ResetParams();
    }

    //------------------------------------------------------------------------------
    // Use this for initialization
    void Start () {
    }

    //------------------------------------------------------------------------------
    void Update()
    {
        ++stateCounter;
        stateTimer += Time.deltaTime;
        if( state != nextState )
        {
            state        = nextState;
            stateCounter = 0;
            stateTimer   = 0.0f;

            switch( state )
            {
                case cStateIdle:
                    enterIdle();
                    break;
            }
        }

        switch( state )
        {
            case cStateIdle:
                updateIdle();
                break;
            case cStateAppear:
                updateAppear();
                break;
            case cStateDisappear:
                updateDisappear();
                break;
            case cStateFailed:
                updateFailed();
                break;
        }
    }

    //------------------------------------------------------------------------------
    private void changeState( int next_state )
    {
        nextState = next_state;
    }

    //------------------------------------------------------------------------------
    private void enterIdle()
    {
    }

    //------------------------------------------------------------------------------
    private void updateIdle()
    {
        /* if( currReachability != Application.internetReachability ) */
        /* { */
        /*     currReachability = Application.internetReachability; */

        /*     if( currReachability == NetworkReachability.NotReachable && isAppearing ) */
        /*     { */
        /*         Disappear(); */
        /*     } */
        /* } */

        /* switch ( Application.internetReachability ) { */
        /* } */
    }

    //------------------------------------------------------------------------------
    private void updateAppear()
    {
        if( stateCounter == 0 )
        {
            failedObj.SetActive( false );
        }

        bool result;
        const float eps = 1.0f;
        float deltaTime = UnityEngine.Mathf.Min( Time.deltaTime, 0.03333f );
        float x = WK.Math.Mathf.Approach( out result, appearX, transform.localPosition.x, deltaTime * slideInSpeed, eps );
        transform.localPosition = new Vector3( x
                , transform.localPosition.y
                , transform.localPosition.z
                );
        if( result )
        {
            isAppearing = true;
            canvasGroup.blocksRaycasts = true;
            changeState( cStateIdle );
        }
    }

    //------------------------------------------------------------------------------
    private void updateDisappear()
    {
        bool result;
        const float eps = 1.0f;
        float x = WK.Math.Mathf.Approach( out result, disappearX, transform.localPosition.x, Time.deltaTime * slideInSpeed, eps );
        transform.localPosition = new Vector3( x
                , transform.localPosition.y
                , transform.localPosition.z
                );
        if( result )
        {
            changeState( cStateIdle );
        }
    }

    //------------------------------------------------------------------------------
    private void updateFailed()
    {
        if( stateCounter == 0 )
        {
            failedObj.SetActive( true );
        }

        const float cWaitTime = 2.5f;
        if( stateTimer > cWaitTime ){
            failedCallback();
            Disappear();
        }
    }

    //------------------------------------------------------------------------------
    public void ResetParams () {
        isAppearing = false;
        transform.localPosition = new Vector3( disappearX
                , transform.localPosition.y
                , transform.localPosition.z
                );
        canvasGroup.blocksRaycasts = false;
        nextDustbinEnableCount = cFirstNextDustbinEnableCount;
        if( UnityEngine.Debug.isDebugBuild && GameSceneManager.Instance.isDebugDustBinSoon )
        {
            nextDustbinEnableCount = 1;
        }
    }

    //------------------------------------------------------------------------------
    public bool IsAppearing () {
        return isAppearing;
    }

    //------------------------------------------------------------------------------
    public bool IsAppearable () {
        return ( MFSlotController.GetDropCounter() >= nextDustbinEnableCount )
            && Application.internetReachability != NetworkReachability.NotReachable;
    }

    //------------------------------------------------------------------------------
    [Button]
    public void Appear () {
        changeState( cStateAppear );
    }

    //------------------------------------------------------------------------------
    public void Disappear () {
        canvasGroup.blocksRaycasts = false;
        isAppearing = false;
        changeState( cStateDisappear );
    }

    //------------------------------------------------------------------------------
    public void Failed ( Action callback ) {
        canvasGroup.blocksRaycasts = false;
        isAppearing = false;
        failedCallback = callback;
        changeState( cStateFailed );
    }

    //------------------------------------------------------------------------------
    public void ChangeToNextDustbinCount () {
        nextDustbinEnableCount =
            MFSlotController.GetDropCounter() + cNextDustbinAmount;
    }

    //------------------------------------------------------------------------------
    public void RequestSlotDragable( bool enable ) {
        canvasGroup.blocksRaycasts = enable;//event handler系はinteractableでは防げないみたい
    }
}
