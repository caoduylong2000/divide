using UnityEngine;

namespace WK { namespace Unity {

public class SlideInSceneObjController : MonoBehaviour {
    public GameObject slideInObj;
    public Vector3    slideInPos;
    public Vector3    slideOutPos;
    public float      slideInSpeed = 10.0f;
    public float      alphaSpeed   = 5.0f;

    protected Transform slideInObjTransform;

    protected CanvasGroup canvasGroup;
    protected const int eStateIdle     = 0;
    protected const int eStateSlideIn  = 1;
    protected const int eStateSlideOut = 2;

    protected int   state        = eStateIdle;
    protected int   nextState    = eStateIdle;
    protected int   stateCounter = 0;
    protected float stateTimer   = 0;

    protected const int cStateIdle          = 0;

    protected bool isFinishSlideIn;
    public bool IsFinishSlideIn { get { return isFinishSlideIn; } }
    protected bool isFinishSlideOut;
    public bool IsFinishSlideOut { get { return isFinishSlideOut; } }

    public delegate void CompleteCallback();
    private CompleteCallback completeCallback = null;

    protected void changeState( int next_state )
    {
        nextState = next_state;
    }

	void Awake () {
        canvasGroup = GetComponent< CanvasGroup >();
        Debug.Assert( canvasGroup != null );
        canvasGroup.alpha          = 0.0f;
        canvasGroup.blocksRaycasts = false;
        stateCounter = 0;
        stateTimer   = 0.0f;
        state        = cStateIdle;
        nextState    = cStateIdle;
        isFinishSlideOut = false;
        isFinishSlideIn = false;
        slideInObjTransform = slideInObj.transform;
    }

	void Start () {
        canvasGroup.interactable   = false;
	}

    public void Init( bool is_enable, bool is_visible )
    {
        var rect_trans = gameObject.GetComponent< RectTransform >();
        var parent_rect_trans = gameObject.transform.parent.GetComponent< RectTransform >();
        rect_trans.sizeDelta = parent_rect_trans.sizeDelta;

        gameObject.transform.localPosition = new Vector3( 0.0f, 0.0f, 0.0f );
        slideInObjTransform.localPosition = slideOutPos;

        completeCallback = null;
        changeState( cStateIdle );

        SetEnable( is_enable, is_visible );
    }

	public void SetEnable ( bool is_enable, bool is_visible ) {
        Debug.Assert( canvasGroup != null );
        if( is_enable )
        {
            canvasGroup.interactable = true; 
            canvasGroup.blocksRaycasts = true; 
        }
        else
        {
            canvasGroup.interactable = false; 
            canvasGroup.blocksRaycasts = false; 
        }

        canvasGroup.alpha = is_visible ? 1.0f : 0.0f;
    }

    public SlideInSceneObjController SlideIn()
    {
        changeState(eStateSlideIn);
        canvasGroup.interactable = false; 
        canvasGroup.blocksRaycasts = true; 
        isFinishSlideOut = false;
        isFinishSlideIn = false;
        return this;
    }

    public SlideInSceneObjController SlideInImmediately()
    {
        changeState(eStateSlideIn);
        canvasGroup.interactable = false; 
        canvasGroup.blocksRaycasts = true; 
        isFinishSlideOut = false;
        isFinishSlideIn = false;
        canvasGroup.alpha = 1.0f;
        slideInObjTransform.localPosition = slideInPos;
        return this;
    }

    public SlideInSceneObjController SlideOut()
    {
        changeState(eStateSlideOut);
        canvasGroup.interactable = false; 
        canvasGroup.blocksRaycasts = true; 
        isFinishSlideOut = false;
        isFinishSlideIn = false;
        return this;
    }

    public SlideInSceneObjController SlideOutImmediately()
    {
        changeState(eStateSlideOut);
        canvasGroup.interactable = false; 
        canvasGroup.blocksRaycasts = true; 
        isFinishSlideOut = false;
        isFinishSlideIn = false;
        canvasGroup.alpha = 0.0f;
        slideInObjTransform.localPosition = slideOutPos;
        return this;
    }

    public SlideInSceneObjController OnComplete(CompleteCallback callback)
    {
        completeCallback = callback;
        return this;
    }

    private void doCompleteCallback()
    {
        if( completeCallback != null )
        {
            completeCallback();
            completeCallback = null;
        }
    }

	public bool IsStateIdle () {
        return state == eStateIdle;
    }

    void Update()
    {
        ++stateCounter; 
        stateTimer += Time.deltaTime;
        if( state != nextState )
        {
            state        = nextState;
            stateCounter = 0;
            stateTimer   = 0.0f;
        }

        const float cRelPosEPS = 5.0f;

        switch( state )
        {
            case eStateIdle:
                doUpdateIdle();
                break;
            case eStateSlideIn:
                {
                    bool is_alpha_done = false;
                    canvasGroup.alpha += alphaSpeed * Time.deltaTime;
                    if( canvasGroup.alpha >= 1.0f )
                    {
                        canvasGroup.alpha = 1.0f;
                        is_alpha_done = true;
                    }

                    Vector3 relPos = slideInPos - slideInObjTransform.localPosition;
                    slideInObjTransform.localPosition += relPos * Time.deltaTime * slideInSpeed;

                    bool is_move_done = false;
                    if( WK.Math.Mathf.LenSqr( relPos ) <= cRelPosEPS )
                    {
                        slideInObjTransform.localPosition = slideInPos;
                        is_move_done = true;
                    }

                    if( is_alpha_done && is_move_done )
                    {
                        isFinishSlideIn = true;
                        canvasGroup.interactable = true; 
                        doCompleteCallback();
                        changeState(eStateIdle);
                    }
                }
                break;
            case eStateSlideOut:
                {
                    bool is_alpha_done = false;
                    canvasGroup.alpha -= alphaSpeed * Time.deltaTime;
                    if( canvasGroup.alpha <= 0.0f )
                    {
                        canvasGroup.alpha = 0.0f;
                        is_alpha_done = true;
                    }

                    Vector3 relPos = slideOutPos - slideInObjTransform.localPosition;
                    slideInObjTransform.localPosition += relPos * Time.deltaTime * slideInSpeed;

                    bool is_move_done = false;
                    if( WK.Math.Mathf.LenSqr( relPos ) <= cRelPosEPS )
                    {
                        slideInObjTransform.localPosition = slideOutPos;
                        is_move_done = true;
                    }

                    if( is_alpha_done && is_move_done )
                    {
                        isFinishSlideOut = true;
                        canvasGroup.blocksRaycasts = false; 
                        doCompleteCallback();
                        changeState(eStateIdle);
                    }
                }
                break;
        }
    }

    protected virtual void doUpdateIdle()
    {
    }
}

}}
