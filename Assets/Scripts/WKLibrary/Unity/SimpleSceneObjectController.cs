using UnityEngine;

namespace WK { namespace Unity {

public class SimpleSceneObjectController : MonoBehaviour {
    protected CanvasGroup canvasGroup;
    public float alphaSpeed = 5.0f;

    const int eStateIdle    = 0;
    const int eStateFadeOut = 1;
    const int eStateFadeIn  = 2;

    int state = eStateIdle;

    private SimpleSceneObjectController reserveDisappearCtrl = null;
    private SimpleSceneObjectController reserveAppearCtrl = null;

    public delegate void CompleteCallback();
    private CompleteCallback completeCallback = null;

	void Awake () {
        canvasGroup = GetComponent< CanvasGroup >();
        Debug.Assert( canvasGroup != null );
        canvasGroup.alpha = 0.0f; 
        canvasGroup.blocksRaycasts = false; 
        state = eStateIdle;
        //@memo
        //Awakeでinteractable = falseにすると
        //子供のボタンの色に不整合が起きる
        //(interactableなのにdisinteractableな色になる)
        //ちなみに、インスペクタで最初からinteractable = falseにしておけば
        //この問題は起きない
        //
        //Startで呼ぼうとしたが、他のStart内からInit()が呼ばれることが多く、
        //そうすると、Startの呼ばれる順によってinteractableの状態が変わってしまう。
        //故に、canvasGroupのinteractableはここではいじらないこととする
    }

	void Start () {
        /* canvasGroup.interactable = false; */ 
	}

    public void Init( bool is_enable, bool is_visible )
    {
        var rect_trans = gameObject.GetComponent< RectTransform >();
        var parent_rect_trans = gameObject.transform.parent.GetComponent< RectTransform >();
        rect_trans.sizeDelta = parent_rect_trans.sizeDelta;

        gameObject.transform.localPosition = new Vector3( 0.0f, 0.0f, 0.0f );
        SetEnable( is_enable, is_visible );
    }

	public void SetInteractableAndBlock ( bool is_enable ) {
        Debug.Assert( canvasGroup != null );
        canvasGroup.interactable = is_enable; 
        canvasGroup.blocksRaycasts = is_enable; 
    }

	public void SetVisible ( bool is_visible ) {
        Debug.Assert( canvasGroup != null );
        canvasGroup.alpha = is_visible ? 1.0f : 0.0f;
    }

	public void SetEnable ( bool is_enable, bool is_visible ) {
        SetInteractableAndBlock( is_enable );
        SetVisible( is_visible );
    }

	public bool IsStateIdle () {
        return state == eStateIdle;
    }

	public SimpleSceneObjectController FadeIn () {
        return FadeIn( null );
    }

	public SimpleSceneObjectController FadeIn ( SimpleSceneObjectController behind_ctrl ) {
        state = eStateFadeIn;
        canvasGroup.interactable = false; 
        canvasGroup.blocksRaycasts = true; 
        reserveDisappearCtrl = behind_ctrl;
        if( reserveDisappearCtrl != null )
        {
            reserveDisappearCtrl.SetInteractableAndBlock( false );
        }
        return this;
    }

	public SimpleSceneObjectController FadeOut () {
        return FadeOut( null );
    }

	public SimpleSceneObjectController FadeOut ( SimpleSceneObjectController behind_ctrl ) {
        state = eStateFadeOut;
        canvasGroup.interactable = false; 
        canvasGroup.blocksRaycasts = true; 
        reserveAppearCtrl = behind_ctrl;
        if( reserveAppearCtrl != null )
        {
            reserveAppearCtrl.SetInteractableAndBlock( false );
            reserveAppearCtrl.SetVisible( true );
        }
        return this;
    }

    public SimpleSceneObjectController OnComplete(CompleteCallback callback)
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

	// Update is called once per frame
	void Update () {
        switch( state )
        {
            case eStateIdle:
                break;
            case eStateFadeIn:
                canvasGroup.alpha += alphaSpeed * Time.deltaTime;
                if( canvasGroup.alpha >= 1.0f )
                {
                    canvasGroup.alpha = 1.0f;
                    canvasGroup.interactable = true; 
                    state = eStateIdle;
                    if( reserveDisappearCtrl != null )
                    {
                        reserveDisappearCtrl.SetVisible( false );
                        reserveDisappearCtrl = null;
                    }
                    doCompleteCallback();
                }
                break;
            case eStateFadeOut:
                canvasGroup.alpha -= alphaSpeed * Time.deltaTime;
                if( canvasGroup.alpha <= 0.0f )
                {
                    canvasGroup.alpha = 0.0f;
                    canvasGroup.blocksRaycasts = false; 
                    state = eStateIdle;
                    if( reserveAppearCtrl != null )
                    {
                        reserveAppearCtrl.SetInteractableAndBlock( true );
                        reserveAppearCtrl = null;
                    }
                    doCompleteCallback();
                }
                break;
        }
	}
}

}}
