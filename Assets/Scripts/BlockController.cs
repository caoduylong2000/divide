using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using WK.Audio;

public class BlockController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public GameObject divideEffectPrehab;
    public GameObject fixFramePrehab;
    [SerializeField]
    protected Image blockImage;
    [SerializeField]
    protected Image blockShadowImage;
    [SerializeField]
    protected Image blockHighlightImage;
    [SerializeField]
    protected Text text;
    [SerializeField]
    protected WK.CurveAnimator curveAnimator;
    private CanvasGroup canvasGroup;
	public int number = 1;
	public static GameObject blockBeginDragged;
	public static GameObject blockInDustbin;
	private Vector3 startPosition;
	private Transform startParent;
	private Transform cachedTransform;
	public bool isDragable = true;
	private bool requestDragable = true;
	public bool isFramed = true;
	private bool requestFramed = true;

	public float moveDustbinSpeed = 8.0f;

	private static Vector3 sGrabPosOffset = new Vector3( 0.0f, 100.0f, 0.0f );
    private GameObject shadowObj = null;

	private int currState;
	private int nextState;
	private int stateCounter;
	private float stateTimer;

	private float cDivideWaitSec = 0.9f;

    private Action backFromDustbinFinishCallback = null;

    List<int> divideNumbers;

    private Vector3 dustbinPos;
	private Transform canvasTransform;

    private Color colorDrag = WK.Utils.Utils.GetRgbColor( 0x65FBBCFF );
    private Color colorDrop = WK.Utils.Utils.GetRgbColor( 0x1BD9B2FF );
    private Color colorDividable = WK.Utils.Utils.GetRgbColor( 0x73FFE3FF );
    public static int draggingCheckCounter = 0;
    private int addScoreBase = 0;
	public enum EState
	{
		cState_Idle
		, cState_Combo
		, cState_Dividing
		, cState_Disappear
		, cState_GoToDustbin
		, cState_DustbinIdle
		, cState_BackFromDustbin
	};

	public int GetState()
	{
		return currState;
	}

    //------------------------------------------------------------------------------
	void Awake () {
        divideNumbers = new List<int>();
        canvasGroup = GetComponent<CanvasGroup>();
        addScoreBase = 0;
        cachedTransform = transform;
		requestDragable = true;
    }

	// Use this for initialization
	void Start () {
		//Text text = GetComponentsInChildren<Text>()[ 0 ];
		text.text = number.ToString();
		stateCounter = 0;
		stateTimer = 0.0f;
		isDragable = true;
		isFramed = false;
		requestFramed = false;

		canvasTransform = GameObject.Find( "Canvas" ).transform;
        shadowObj = cachedTransform.Find( "ShadowImage" ).gameObject;
        shadowObj.SetActive( false );
        blockImage.color = colorDrop;
	}

	//------------------------------------------------------------------------------
	public void RequestDragable ( bool enable ) {
		requestDragable = enable;
	}

	//------------------------------------------------------------------------------
	public void RequestFramed () {
		requestFramed = true;
	}

	//------------------------------------------------------------------------------
	public void Hoge () {
		//Text text = GetComponentsInChildren<Text>()[ 0 ];
		text.text = number.ToString() + "a";
	}

	//------------------------------------------------------------------------------
	public int GetAddScoreBase()
	{
		return addScoreBase;
	}

	//------------------------------------------------------------------------------
	public void ClearAddScoreBase()
	{
		addScoreBase = 0;
	}

	//------------------------------------------------------------------------------
    public bool IsBeingDivided()
    {
        return currState == (int)(EState.cState_Dividing);
    }

	//------------------------------------------------------------------------------
	public void RequestDivide( int rdn )
	{
		Assert.AreEqual( 0, number % rdn );
        divideNumbers.Add( rdn );
        /* Debug.Log( "divideNumbers : " +rdn.ToString() + ", len: " + divideNumbers.Count ); */

		changeState( EState.cState_Dividing );
	}

	//------------------------------------------------------------------------------
	public void RequestDisappear()
	{
        RequestDivide( number );
		changeState( EState.cState_Dividing ); //1にdivideしてからdisappear
	}

	// Update is called once per frame
	void Update () {
		stateCounter++;
		stateTimer += Time.deltaTime;
		if( nextState != currState )
		{
			currState = nextState;
			stateCounter = 0;
			stateTimer = 0.0f;
            switch( currState )
            {
                case (int)EState.cState_Idle:
                    break;
                case (int)EState.cState_Combo:
                    break;
                case (int)EState.cState_Dividing:
                    enterDividing();
                    break;
                case (int)EState.cState_Disappear:
                    break;
                case (int)EState.cState_GoToDustbin:
                    break;
                case (int)EState.cState_DustbinIdle:
                    break;
                case (int)EState.cState_BackFromDustbin:
                    break;
            }
		}

		if( isFramed != requestFramed )
		{
			isFramed = requestFramed;
            if( isFramed )
            {
                GameObject frame = Instantiate( fixFramePrehab, new Vector3( 0.0f, 0.0f, 0.0f ), Quaternion.identity ) as GameObject;
                frame.transform.SetParent( gameObject.transform );
                frame.transform.localPosition = new Vector3( 0, 0, 0 );
                frame.transform.localScale = new Vector3( 1, 1, 1 );
            }
        }

		if( isDragable != requestDragable )
		{
			isDragable = requestDragable;
            canvasGroup.blocksRaycasts = isDragable;
		}

		switch( currState )
		{
			case (int)EState.cState_Idle:
                {
                }
				break;
			case (int)EState.cState_Combo:
				break;
			case (int)EState.cState_Dividing:
                if( number == 1 )
                {
                    canvasGroup.alpha = 0.0f;
                    //GetComponentInChildren<Image>().enabled = false;
                    //GetComponentInChildren<Text>().enabled = false;
                }

				if( stateTimer > cDivideWaitSec ) // wait
				{
					if( number == 1 )
					{
						changeState( EState.cState_Disappear );
					}
					else
					{
						changeState( EState.cState_Idle );
					}
				}
				break;
			case (int)EState.cState_Disappear:
                Destroy( this.gameObject );
				break;
			case (int)EState.cState_GoToDustbin:
                updateGoToDustbin();
				break;
			case (int)EState.cState_DustbinIdle:
                updateDustbinIdle();
				break;
			case (int)EState.cState_BackFromDustbin:
                updateBackFromDustbin();
				break;
		}
	}

    //------------------------------------------------------------------------------
    private void updateGoToDustbin()
    {
        const float eps = 0.01f;
        /* Vector3 rel_pos = dustbinPos - transform.localPosition; */
        /* transform.localPosition += rel_pos * Time.deltaTime * moveDustbinSpeed; */
        bool result;
        cachedTransform.localPosition =
            WK.Math.Mathf.Approach( out result, dustbinPos
                    , cachedTransform.localPosition
                    , Time.deltaTime * moveDustbinSpeed, eps );

        Vector3 rel_scale = -cachedTransform.localScale;
        cachedTransform.localScale += rel_scale * Time.deltaTime * moveDustbinSpeed;

        /* if( Mathf.Abs( rel_scale.x ) < eps ) */
        if( result )
        {
            cachedTransform.localPosition = dustbinPos;
            cachedTransform.localScale = new Vector3( 0.0f, 0.0f, 0.0f );
            changeState( EState.cState_DustbinIdle );
        }
    }

    //------------------------------------------------------------------------------
    private void updateDustbinIdle()
    {
    }

    //------------------------------------------------------------------------------
    private void updateBackFromDustbin()
    {
        Vector3 target_pos = startParent.GetComponent< RectTransform >().TransformPoint( new Vector3( 0, 0, 0) );
        target_pos = cachedTransform.parent.GetComponent< RectTransform >().InverseTransformPoint( target_pos );

        Vector3 rel_pos = target_pos - cachedTransform.localPosition;
        cachedTransform.localPosition += rel_pos * Time.deltaTime * moveDustbinSpeed;

        Vector3 rel_scale = new Vector3( 1, 1, 1 ) - cachedTransform.localScale;
        cachedTransform.localScale += rel_scale * Time.deltaTime * moveDustbinSpeed;

        const float eps = 0.01f;
        if( Mathf.Abs( rel_scale.x ) < eps )
        {
            cachedTransform.SetParent( startParent );
            cachedTransform.localPosition = new Vector3( 0, 0, 0 );
            cachedTransform.localScale = new Vector3( 1, 1, 1 );
            canvasGroup.blocksRaycasts = true;

            if( backFromDustbinFinishCallback != null )
            {
                backFromDustbinFinishCallback();
                backFromDustbinFinishCallback = null;
            }

            changeState( EState.cState_Idle );
        }
    }

    //------------------------------------------------------------------------------
	public void changeState ( EState currState ) {
		nextState = (int)currState;
	}

    //------------------------------------------------------------------------------
	private void calcDividedNumber( int num, ref int min, int marks ) {
        for( int i = 0; i < divideNumbers.Count; ++i )
        {
            if( ( marks & ( 1 << i ) ) == 0 )
            {
                marks |= 1 << i;
                /* Debug.Log( "divideNumbers:" + divideNumbers[ i ].ToString() + ", min: " + min.ToString() ); */
                if( num % divideNumbers[ i ] == 0 )
                {
                    int next_num = num / divideNumbers[ i ];
                    if( next_num < min )
                    {
                        min = next_num;
                    }
                    calcDividedNumber( next_num, ref min, marks );
                }
            }
        }
    }

	private void enterDividing () {
        int next_number = number;
        int marks = 0;
        calcDividedNumber( next_number, ref next_number, marks );

        divideNumbers.Clear();
        int divided_number = number / next_number;
        addScoreBase = divided_number;
        setNumberImpl_( next_number );

        requestDivideSound();

        GameObject effect = Instantiate( divideEffectPrehab, new Vector3( 0.0f, 0.0f, 0.0f ), Quaternion.identity ) as GameObject;
        DividedNumberEmitter emitter = effect.GetComponent<DividedNumberEmitter>();
        const int cMaxParticle = 16;
        emitter.SetParticle( Math.Min( cMaxParticle, divided_number ), next_number );
        emitter.SetColor( blockImage.color );
        emitter.SetSprite( ColorManager.Instance.EffectSprite );
        emitter.Emit();
        effect.transform.SetParent(cachedTransform);
        effect.transform.localPosition = new Vector3( 0, 0, 0 );
        effect.transform.localScale = new Vector3( 1, 1, 1 );
		effect.transform.SetParent( EmitterRoot.Instance.transform );//一番上に描画するために

        //@memo アニメーション調整が必要
        //curveAnimator.Play();

        VibrationManager.Instance.Vibrate();

        //複数回呼ばれてしまう可能性があるが他に良い場所がないので許容
        StartCoroutine( this.DelayMethod( 0.03f, () => MainFieldController.Instance.InvalidateDividableColor() ) );
	}

	public void OnBeginDrag( PointerEventData eventData )
	{
		canvasGroup.blocksRaycasts = false;
		blockBeginDragged = gameObject;
		startPosition = cachedTransform.position;
		startParent = cachedTransform.parent;
		cachedTransform.SetParent( canvasTransform );//一番上に描画するために
        blockImage.color = colorDrag;
        draggingCheckCounter = 0;
        //@memo アニメーション調整が必要
        //curveAnimator.Play();
	}

	public void OnDrag( PointerEventData eventData )
	{
        if(blockBeginDragged == null)
        {
            return;
        }

		//cachedTransform.position = Input.mousePosition + sGrabPosOffset;
        var cam = Camera.main;
        var screen_pos = Input.mousePosition + sGrabPosOffset;
        screen_pos.z = cam.nearClipPlane;
		cachedTransform.position =
            Camera.main.ScreenToWorldPoint( screen_pos );
        //Debug.Log("OnDrag " + cachedTransform.position);
        shadowObj.SetActive( true );
        draggingCheckCounter = 0;
	}

	public void OnEndDrag( PointerEventData eventData )
	{
        shadowObj.SetActive( false );
		canvasGroup.blocksRaycasts = true;
        blockImage.color = colorDrop;
		blockBeginDragged = null;
		if ( blockInDustbin == gameObject )
        {
        }
		else if ( cachedTransform.parent == canvasTransform )
		{
			cachedTransform.position = startPosition;
            cachedTransform.SetParent( startParent );
            SoundManager.Instance.PlaySe( "return" );
            //@memo アニメーション調整が必要
            //curveAnimator.Play();
		}
        else
        {
            //@memo アニメーション調整が必要
            //curveAnimator.Play();
            SoundManager.Instance.PlaySe( "drop" );
        }
	}

	public void BackToOrgPosition()
	{
        shadowObj.SetActive( false );
		canvasGroup.blocksRaycasts = true;
        blockImage.color = colorDrop;
		blockBeginDragged = null;
        cachedTransform.position = startPosition;
        cachedTransform.SetParent( startParent );
        //SoundManager.Instance.PlaySe( "return" );
    }

	public void ChangeColorDividable( bool is_dividable )
	{
        blockImage.color = is_dividable ?  colorDividable : colorDrop;
    }

	public void SetNumberImidiately( int num ) {
		if( num != number )
		{
			setNumberImpl_( num );
		}
	}

	private void requestDivideSound (){
		SoundManager.Instance.PlaySe( "divide0" );
	}

	private void setNumberImpl_ ( int num ) {
		number = num;
        updateColor( number );
		text.text = number.ToString();
	}

	public bool IsDisappearable() {
		return number == 1;
	}

	public void GoToDustbin( Vector3 dust_bin_pos ) {
        dustbinPos = cachedTransform.parent.GetComponent< RectTransform >().InverseTransformPoint( dust_bin_pos );
		canvasGroup.blocksRaycasts = true;
        changeState( EState.cState_GoToDustbin );
	}

	public void BackFromDustbin( Action callback ) {
        backFromDustbinFinishCallback = callback;
        changeState( EState.cState_BackFromDustbin );
	}

	void updateColor( int num ) {
        colorDrop = ColorManager.Instance.GetBlockColor( num );
        float h, s, v;
        Color.RGBToHSV( colorDrop, out h, out s, out v );
        colorDrag = Color.HSVToRGB( Mathf.Repeat( h + 0.0f, 1.0f ), s - 0.2f, v + 0.05f );
        colorDividable = Color.HSVToRGB( Mathf.Repeat( h + 0.0f, 1.0f ), s - 0.1f, v + 0.15f );
        blockImage.color = colorDrop;
    }

	public void UpdateSkin() {
        updateColor( number );

        blockImage.sprite = ColorManager.Instance.BlockSprite;
        blockHighlightImage.sprite = ColorManager.Instance.BlockHighlightSprite;
        blockHighlightImage.gameObject.SetActive(
                blockHighlightImage.sprite != null
                );
        blockShadowImage.sprite = ColorManager.Instance.BlockShadowSprite;
        blockShadowImage.gameObject.SetActive(
                blockShadowImage.sprite != null
                );
    }

}
