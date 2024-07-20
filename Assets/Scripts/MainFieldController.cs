/**
 *
 * アニメーションなどの見た目の推移はブロック側に任せる
 *
 * こっちではブロックが消えるべきか割られるべきか、ゲーム的にロジカルなステートを管理する
 *
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using WK.Collections;
using WK.Utils;
using WK.Unity;
using WK.Audio;
using Sirenix.OdinInspector;

public class MainFieldController : SingletonMonoBehaviour< MainFieldController > {
	//public GameObject slotPrehab;
    [ReadOnly]
	public int xSlotNum = 3;

    [ReadOnly]
	public int ySlotNum = 3;

	public const int MaxSlotNumX = 4;
	public const int MaxSlotNumY = 4;

    [SerializeField]
    protected GameObjectPool slotPool;

    [SerializeField]
    protected GridLayoutGroup gridLayoutGroup;

    [SerializeField]
	private int score = 0;
    public int Score { get { return score; } set { score = value; } }

	//public GameObject[,] slots;
    [SerializeField]
	protected SlotController[,] slots;

    private int gridSize = 3;

    private CanvasGroup canvasGroup;

	private const int eStateIdle 			= 0;
	private const int eStateDisappearing 	= 1;
	private const int eStateDividing 		= 2;
	private const int eStateReady 			= 3;
	private const int eStateResult 			= 4;

	private const int cAllDisappearBonus = 100;
    private const int cBlockDividedScore = 10;

	private int state = 0;
	private int nextState = 0;
	private int stateCounter = 0;

	public int currCombo { get; protected set; }

	Queue<SlotController> checkSlots;

	private bool isCheckRequested  = false;
    private bool isRequestAddScore = false;

	public void RequestCheck () {
		isCheckRequested = true;
	}

	public void Ready ( int grid_size ) {
        gridSize = grid_size;
		//@memo stateはresultのままにしておきたいので、stateはidleにしない
		changeState( eStateReady );
	}

	public void GoToIdle () {
		changeState( eStateIdle );
	}

	public bool IsStableIdle () {
		return ( state == eStateIdle ) && ( nextState == eStateIdle );
	}

	private void enqueueToCheckQueue( SlotController slt )
	{
		checkSlots.Enqueue( slt );
	}

	protected override void Awake () {
        base.Awake();

        currCombo = 0;
        isRequestAddScore = false;
		//slots = new GameObject[MaxSlotNumX,MaxSlotNumY];
		slots = new SlotController[MaxSlotNumX,MaxSlotNumY];
		checkSlots = new Queue<SlotController>();

        canvasGroup = GetComponent<CanvasGroup>();
    }

	// Use this for initialization
	void Start () {
        resetSlots( 3, 3 );

		//for( int i_x = 0; i_x < xSlotNum; ++i_x )
		//{
		//	for( int i_y = 0; i_y < ySlotNum; ++i_y )
		//	{
        //        GameObject slot = slotPool.Birth();
		//		slot.name = "Slot-" + i_x.ToString() + "-" + i_y.ToString();
		//		slot.transform.SetParent( transform );
		//		slot.transform.localScale = new Vector3 ( 1.0f, 1.0f, 1.0f );
		//		slots[ i_x, i_y ] = slot.GetComponent<SlotController>();
		//		//slots[ i_x, i_y ].GetComponent<SlotController>().SetIndex( i_x, i_y );
		//		slots[ i_x, i_y ].SetIndex( i_x, i_y );

		//		////GameObject slot = Instantiate( slotPrehab, new Vector3( 0, 0, 0 ), Quaternion.identity ) as GameObject;
		//		///* GameObject slot = Instantiate( slotPrehab ) as GameObject; */
		//		///* slot.name = "Slot-" + i_x.ToString() + "-" + i_y.ToString(); */
		//		///* slot.transform.SetParent( transform ); */
		//		///* slot.transform.localScale = new Vector3 ( 1.0f, 1.0f, 1.0f ); */

		//		//string name = "Slot-" + i_x.ToString() + "-" + i_y.ToString();
		//		//GameObject slot = Utils.FindChildRecursively( transform, name ).gameObject;
		//		//slots[ i_x, i_y ] = slot;
		//		//slots[ i_x, i_y ].GetComponent<SlotController>().SetIndex( i_x, i_y );
		//	}
		//}
	}

    //------------------------------------------------------------------------------
    [Button]
	void ResetSlots4x4() {
        resetSlots( 4, 4 );
    }

    //------------------------------------------------------------------------------
    [Button]
	void ResetSlots3x3() {
        resetSlots( 3, 3 );
    }

    //------------------------------------------------------------------------------
	void resetSlots( int slot_x, int slot_y ) {
        xSlotNum = slot_x;
        ySlotNum = slot_y;

        const float FIELD_SIZE = 1200.0f;
        const float SPACING_BASE = 25.0f;
        float spacing = SPACING_BASE - 5.0f * ( slot_x - 3 );
        gridLayoutGroup.cellSize = new Vector2( FIELD_SIZE / xSlotNum - spacing,  FIELD_SIZE / ySlotNum - spacing );
        gridLayoutGroup.spacing = new Vector2( spacing, spacing );

        slotPool.KillAll();
		for( int i_x = 0; i_x < xSlotNum; ++i_x )
		{
			for( int i_y = 0; i_y < ySlotNum; ++i_y )
			{
                GameObject slot = slotPool.Birth();
				slot.name = "Slot-" + i_x.ToString() + "-" + i_y.ToString();
				slot.transform.SetParent( transform );
				slot.transform.localScale = new Vector3 ( 1.0f, 1.0f, 1.0f );
				slots[ i_x, i_y ] = slot.GetComponent<SlotController>();
				//slots[ i_x, i_y ].GetComponent<SlotController>().SetIndex( i_x, i_y );
				slots[ i_x, i_y ].SetIndex( i_x, i_y );
				slots[ i_x, i_y ].UpdateSkin();

				////GameObject slot = Instantiate( slotPrehab, new Vector3( 0, 0, 0 ), Quaternion.identity ) as GameObject;
				///* GameObject slot = Instantiate( slotPrehab ) as GameObject; */
				///* slot.name = "Slot-" + i_x.ToString() + "-" + i_y.ToString(); */
				///* slot.transform.SetParent( transform ); */
				///* slot.transform.localScale = new Vector3 ( 1.0f, 1.0f, 1.0f ); */

				//string name = "Slot-" + i_x.ToString() + "-" + i_y.ToString();
				//GameObject slot = Utils.FindChildRecursively( transform, name ).gameObject;
				//slots[ i_x, i_y ] = slot;
				//slots[ i_x, i_y ].GetComponent<SlotController>().SetIndex( i_x, i_y );
			}
		}
    }

	// Update is called once per frame
	void Update () {
		++stateCounter;
		if( state != nextState )
		{
			state = nextState;
			stateCounter = 0;

			switch( state )
			{
				case eStateIdle:
					enterIdle();
					break;
				case eStateDividing:
					enterDividing();
					break;
				case eStateDisappearing:
					enterDisappearing();
					break;
				case eStateReady:
					enterReady();
					break;
				case eStateResult:
					enterResult();
					break;
			}
		}

		switch( state )
		{
			case eStateIdle:
				updateIdle();
				break;
			case eStateDisappearing:
				updateDisappear();
				break;
			case eStateDividing:
				updateDivide();
				break;
			case eStateReady:
				updateReady();
				break;
			case eStateResult:
				updateResult();
				break;
		}
	}

	//------------------------------------------------------------------------------
	private void enterIdle()
	{
		currCombo = 0;
        SetDroppable( true );
	}

	//------------------------------------------------------------------------------
	private void enterDividing()
	{
        SetDroppable( false );
        GameSceneManager.Instance.PauseTimer();
        InvalidateDividableColor();
	}

	//------------------------------------------------------------------------------
	private void enterDisappearing()
	{
        SetDroppable( false );
	}

	//------------------------------------------------------------------------------
	public void ClearBlocks()
	{
		for( int i_x = 0; i_x < MaxSlotNumX; ++i_x )
		{
			for( int i_y = 0; i_y < MaxSlotNumY; ++i_y )
			{
                var slot = slots[ i_x, i_y ];
                if( slot != null )
                {
                    BlockController block = slot.GetComponentInChildren< BlockController >();
                    if( block )
                    {
                        Destroy( block.gameObject );
                    }
                }
			}
		}
    }

	//------------------------------------------------------------------------------
	private void enterReady()
	{
		score = 0;
#if UNITY_EDITOR
        if( GameSceneManager.Instance.isDebugScore )
        {
            score = GameSceneManager.Instance.debugScore;
        }
#endif
		currCombo = 0;
		stateCounter = 0;
		isCheckRequested = false;
        ClearBlocks();
        resetSlots( gridSize, gridSize );
        ResumeManager.Instance.RequestUpdate();
	}

	//------------------------------------------------------------------------------
	private void enterResult()
	{
	}

	//------------------------------------------------------------------------------
	private bool checkAndRequestDivision()
	{
        isRequestAddScore = true;

		bool is_dividable = false;
		for( int i_x = 0; i_x < xSlotNum; ++i_x )
		{
			for( int i_y = 0; i_y < ySlotNum; ++i_y )
			{
				BlockController block = slots[ i_x, i_y ].GetComponentInChildren< BlockController >();
				if( block )
				{
                    //enqueueToCheckQueue( slots[ i_x, i_y ].GetComponent<SlotController>() );
                    enqueueToCheckQueue( slots[ i_x, i_y ] );
				}
			}
		}

		if( checkSlots.Count != 0 )
		{
			do {
				SlotController slt = checkSlots.Dequeue();
				is_dividable |= checkDivisionAround_( slt );
			} while( checkSlots.Count != 0 );
		}

		return is_dividable;
	}

	//------------------------------------------------------------------------------
	public bool checkFinished()
	{
		for( int i_x = 0; i_x < xSlotNum; ++i_x )
		{
			for( int i_y = 0; i_y < ySlotNum; ++i_y )
			{
				BlockController block = slots[ i_x, i_y ].GetComponentInChildren< BlockController >();
				if( block == null)
				{
					return false;
				}
			}
		}

		return true;
	}

	//------------------------------------------------------------------------------
	private void updateIdle()
	{
		if( isCheckRequested )
		{
			bool is_dividable = checkAndRequestDivision();
			if( is_dividable )
			{
				currCombo++;
				changeState( eStateDividing );
                SetDroppable( false );
			}
			isCheckRequested = false;
            //Debug.Log( "isRequestCheck : " + isCheckRequested + "," + is_dividable );
		}
		else
		{
			float last_dropped_interval = Time.time - SlotController.lastDroppedTime;
			const float cWaitInterval = 1.0f;
            if(checkFinished())
            {
                SetDroppable( false );
                StockFieldController stock_field_ctrl = GameObject.Find( "StockField" ).GetComponent<StockFieldController>();
                stock_field_ctrl.SetDraggable( false );

                if( last_dropped_interval > cWaitInterval )
                {
                    GameObject.Find( "GameSceneManager" ).GetComponent< GameSceneManager >().GoToResult();
                    changeState( eStateResult );
                }
            }
		}
	}

	//------------------------------------------------------------------------------
	private void updateDisappear()
	{
	}

	//------------------------------------------------------------------------------
	private void updateDivide()
	{
        if( isRequestAddScore )
        {
            addScore();
            isRequestAddScore = false;
        }

		if( IsAllBlockIdle() )
		{
			RequestCheck();
			bool is_dividable = checkAndRequestDivision();
			if( !is_dividable )
			{
                GameSceneManager.Instance.ResumeTimer();
				changeState( eStateIdle );
			}
			else
			{
				currCombo++;
				if (currCombo == 2)
					GameSceneManager.Instance.BonusTimer(1.0f);
				if (currCombo == 3)
					GameSceneManager.Instance.BonusTimer(2.0f);
				if (currCombo == 4)
					GameSceneManager.Instance.BonusTimer(3.0f);
				if (currCombo >= 5)
                    GameSceneManager.Instance.BonusTimer(4.0f);
            }
		}
	}

	//------------------------------------------------------------------------------
	private void updateReady()
	{
	}

	//------------------------------------------------------------------------------
	private void updateResult()
	{
	}

	//------------------------------------------------------------------------------
    public void AddScore( int add_score )
    {
        score += add_score;
        /* Debug.Log( score.ToString() ); */
    }

	//------------------------------------------------------------------------------
	private void addScore()
	{
        bool is_all_disappeared = true;
        int divided_count = 0;
        Vector3 combo_pos = Vector3.zero;
        int grid_size = GameSceneManager.Instance.GameMode == EGameMode.four ? 4 : 3;
		for( int i_x = 0; i_x < xSlotNum; ++i_x )
		{
			for( int i_y = 0; i_y < ySlotNum; ++i_y )
			{
				BlockController block = slots[ i_x, i_y ].GetComponentInChildren< BlockController >();
				if( block )
				{
                    /* score += block.GetAddScoreBase() * currCombo * currCombo; */
                    /* block.ClearAddScoreBase(); */
                    if( block.IsBeingDivided() )
                    {
                        combo_pos += ComboEffectManager.Instance.GetComboEffectPos( i_x, i_y, grid_size );
                        /* Debug.Log( i_x.ToString() + ", " + i_y.ToString() ); */
                        divided_count++;
                    }

                    if( block.number != 1 )
                    {
                        is_all_disappeared = false;
                    }
				}
			}
		}

        if( divided_count > 0 )
        {
            ComboEffectManager.Instance.Birth( combo_pos / divided_count, currCombo );
        }

        AddScore( System.Math.Max( 0, ( divided_count - 1 ) ) * cBlockDividedScore * currCombo * currCombo );

        if( is_all_disappeared )
        {
            SoundManager.Instance.ClearAllSeRequest();
            SoundManager.Instance.PlaySe( "allDisappear" );
            AddScore( cAllDisappearBonus );
        }
	}

	//------------------------------------------------------------------------------
	private void changeState( int st )
	{
		nextState = st;
        /* Debug.Log( "changeState:" + st ); */
	}

	//------------------------------------------------------------------------------
	// check it is possible to be divided
	private bool checkDivisionAround_( SlotController slt ) {
		Assert.IsNotNull( slt );

		BlockController block = slt.GetComponentInChildren< BlockController >();
		if( block == null )
		{
			return false;
		}

		int number = block.number;
		int i_x = slt.X;
		int i_y = slt.Y;

		bool isDividable = false;
		isDividable |= requestDivide_( number, i_x - 1, i_y );
		isDividable |= requestDivide_( number, i_x + 1, i_y );
		isDividable |= requestDivide_( number, i_x, i_y - 1 );
		isDividable |= requestDivide_( number, i_x, i_y + 1 );
		if( isDividable )
		{
			block.RequestDisappear();
		}

		return isDividable;
	}


	//------------------------------------------------------------------------------
	private bool requestDivide_( int number, int i_x, int i_y ) {
		if( 0 <= i_x
				&& i_x < xSlotNum
				&& 0 <= i_y
				&& i_y < ySlotNum
		  )
		{
			BlockController neighbor = slots[ i_x, i_y ].GetComponentInChildren< BlockController >();
			if( neighbor != null )
			{
				int remainder = neighbor.number % number;
				if( remainder == 0 )
				{

					/* bool is_already_requested = neighbor.number != neighbor.GetNextNumber();//既に他の隣接ブロックからdivideリクエストされている */
					/* if( is_already_requested ) */
					/* { */
					/* 	int already_requested_number = neighbor.number / neighbor.GetNextNumber(); */
					/* 	int stacked_divide_number = ( already_requested_number * number );//重ねがけ(重ね割)するとしたらの数字 */
					/* 	int remainder_again = neighbor.number % stacked_divide_number; */
					/* 	if( remainder_again == 0 ) */
					/* 	{ */
					/* 		neighbor.RequestDivide( stacked_divide_number ); */
					/* 	} */
					/* } */
					/* else */
					/* { */
					/* 	neighbor.RequestDivide( number ); */
					/* } */

                    neighbor.RequestDivide( number );
					return true;
				}
				/*
				if( remainder == 0 )
				{
					if( neighbor.GetNextNumber() % number == 0 )//既に他の隣接ブロックでdivideリクエストが掛かってる可能性がある
					{
						neighbor.RequestDivide( number );
					}
					return true;
				}
				*/
			}
		}

		return false;
	}

	//------------------------------------------------------------------------------
	private bool IsAllBlockIdle()
	{
		bool is_idle = true;
		for( int i_x = 0; i_x < xSlotNum; ++i_x )
		{
			for( int i_y = 0; i_y < ySlotNum; ++i_y )
			{
				BlockController block = slots[ i_x, i_y ].GetComponentInChildren< BlockController >();
				if( block )
				{
					if( block.GetState() != (int)BlockController.EState.cState_Idle )
					{
						is_idle = false;
					}
				}
			}
		}
		return is_idle;
	}

	//------------------------------------------------------------------------------
	public WK.Collections.Tuple< int[], int > GetDividableNumbers( int[] candidate_array, int array_length )
	{
        int[] primeCandidates = new int[ array_length ];
        int numPrimeCandidates = 0;
        for( int i = 0; i < array_length; ++i )
        {
            if( checkDividableNumber( candidate_array[ i ] ) )
            {
                primeCandidates[ numPrimeCandidates++ ] = candidate_array[ i ];
            }
        }

        return new WK.Collections.Tuple< int[], int >( primeCandidates, numPrimeCandidates );
    }

	//------------------------------------------------------------------------------
    private bool checkDividableNumber( int num )
    {
        for( int i_x = 0; i_x < xSlotNum; ++i_x )
        {
            for( int i_y = 0; i_y < ySlotNum; ++i_y )
            {
                BlockController block = slots[ i_x, i_y ].GetComponentInChildren< BlockController >();
                if( block )
                {
                    if( ( block.number % num ) == 0 )
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

	//------------------------------------------------------------------------------
    public bool CheckDividableNumberConsideringPlace( int num )
    {
        for( int i_x = 0; i_x < xSlotNum; ++i_x )
        {
            for( int i_y = 0; i_y < ySlotNum; ++i_y )
            {
                BlockController block = slots[ i_x, i_y ].GetComponentInChildren< BlockController >();
                if( block == null )
                {
                    if( checkDividable_( num, i_x - 1, i_y )) return true;
                    if( checkDividable_( num, i_x + 1, i_y )) return true;
                    if( checkDividable_( num, i_x, i_y - 1 )) return true;
                    if( checkDividable_( num, i_x, i_y + 1 )) return true;
                }
            }
        }
        return false;
    }

    //------------------------------------------------------------------------------
	private bool checkDividable_( int number, int i_x, int i_y ) {
		if( 0 <= i_x
				&& i_x < xSlotNum
				&& 0 <= i_y
				&& i_y < ySlotNum
		  )
		{
            BlockController block = slots[ i_x, i_y ].GetComponentInChildren< BlockController >();
            if( block )
            {
                return ( block.number % number == 0 ) || ( number % block.number == 0 );
            }
        }
        return false;
    }

	//------------------------------------------------------------------------------
    public void ChangeColorDividable( int num, int i_x, int i_y )
    {
		if( 0 <= i_x
				&& i_x < xSlotNum
				&& 0 <= i_y
				&& i_y < ySlotNum
		  )
		{
            BlockController block = slots[ i_x, i_y ].GetComponentInChildren< BlockController >();
            changeColorDividable_( num, i_x - 1, i_y );
            changeColorDividable_( num, i_x + 1, i_y );
            changeColorDividable_( num, i_x, i_y - 1 );
            changeColorDividable_( num, i_x, i_y + 1 );
        }
    }

    //------------------------------------------------------------------------------
	private void changeColorDividable_( int number, int i_x, int i_y ) {
		if( 0 <= i_x
				&& i_x < xSlotNum
				&& 0 <= i_y
				&& i_y < ySlotNum
		  )
		{
            BlockController block = slots[ i_x, i_y ].GetComponentInChildren< BlockController >();
            if( block )
            {
                try {
                    //Debug.Log( "ChangeColorDividable " + i_x + "," + i_y + "," + block.number + "," + number );
                    bool is_dividable =
                        ( ( block.number % number == 0 ) || ( number % block.number == 0 ) )
                        && ( block.number != 1 && number != 1 );
                    if( is_dividable )
                    {
                        //Debug.Log( "ChangeColorDividable " + i_x + "," + i_y );
                        block.ChangeColorDividable( true );
                    }
                }
                catch ( Exception e )
                {
                    string message = WK.Translate.TranslateManager.Instance.GetText( "2500" ) + "\n" + e.Message;
                    CommonDialogManager.Instance.SetDialog( message, null );
                    CommonDialogManager.Instance.EnterNotationDialog();
                    CommonDialogManager.Instance.SetExitCompleteCallback( () => {
                            ResumeManager.Instance.Clear();
                            Application.Quit();
                            }
                            );
                }
            }
        }
    }

    //------------------------------------------------------------------------------
	public void ClearDividableColor() {
        for( int i_x = 0; i_x < xSlotNum; ++i_x )
        {
            for( int i_y = 0; i_y < ySlotNum; ++i_y )
            {
                BlockController block = slots[ i_x, i_y ].GetComponentInChildren< BlockController >();
                if( block )
                {
                    block.ChangeColorDividable( false );
                }
            }
        }
    }

    //------------------------------------------------------------------------------
	public void InvalidateDividableColor() {
        ClearDividableColor();
        for( int i_x = 0; i_x < xSlotNum; ++i_x )
        {
            for( int i_y = 0; i_y < ySlotNum; ++i_y )
            {
                BlockController block = slots[ i_x, i_y ].GetComponentInChildren< BlockController >();
                if( block )
                {
                    //Debug.Log( "InvalidateDividableColor " + i_x + "," + i_y + "," + block.number );
                    ChangeColorDividable( block.number, i_x, i_y );
                }
            }
        }
    }

	//------------------------------------------------------------------------------
    public int GetFreeSpaceCount()
    {
        int count = 0;
        for( int i_x = 0; i_x < xSlotNum; ++i_x )
        {
            for( int i_y = 0; i_y < ySlotNum; ++i_y )
            {
                BlockController block = slots[ i_x, i_y ].GetComponentInChildren< BlockController >();
                if( block == null )
                {
                    count++;
                }
            }
        }
        return count;
    }

	//------------------------------------------------------------------------------
	public void SetBlocksRaycast( bool block ) {
        canvasGroup.blocksRaycasts = block;
    }

	//------------------------------------------------------------------------------
	//public bool GetDroppable() {
    //    bool droppable = true;
    //    for( int i_x = 0; i_x < xSlotNum; ++i_x )
    //    {
    //        for( int i_y = 0; i_y < ySlotNum; ++i_y )
    //        {
    //            droppable &= slots[ i_x, i_y ].GetComponentInChildren< WK.Unity.UI.RaycastExtender >().raycastTarget;
    //        }
    //    }
    //    return droppable;
    //}

	//------------------------------------------------------------------------------
	public void SetDroppable( bool enable ) {
        for( int i_x = 0; i_x < xSlotNum; ++i_x )
        {
            for( int i_y = 0; i_y < ySlotNum; ++i_y )
            {
                slots[ i_x, i_y ].GetComponentInChildren< WK.Unity.UI.RaycastExtender >().raycastTarget = enable;
            }
        }
	}

	//------------------------------------------------------------------------------
	public void SetDroppableXY( bool enable, int i_x, int i_y ) {
        slots[ i_x, i_y ].GetComponentInChildren< WK.Unity.UI.RaycastExtender >().raycastTarget = enable;
	}

	//------------------------------------------------------------------------------
	public void SetSaveData( string str ) {
        char[] delimiter = { ',' };
        string[] input = str.Split( delimiter );

        /* BlockController block = slot.block.GetComponent< BlockController >(); */
        /* block.SetNumberImidiately( Int32.Parse( input[0] ) ); */

        /* for( int i = 0; i < numPreNumbers; ++i ) */
        /* { */
        /*     preNumbers[i] = Int32.Parse( input[ i + 1 ] ); */
        /*     nextBlockObjs[ i ].GetComponentInChildren<Text>().text = input[ i + 1 ]; */
        /* } */

        ClearBlocks();

        int grid_size = input.Length == 17 ? 4 : 3;
        xSlotNum = grid_size;
        ySlotNum = grid_size;
        resetSlots( xSlotNum, ySlotNum );
        //Debug.Log( "grid_size : " + grid_size + "," + input.Length + "," + str );
        GameSceneManager.Instance.GameMode = ( grid_size == 4 ) ?
            EGameMode.four : EGameMode.three;

        int input_index = 0;
        for( int i_x = 0; i_x < xSlotNum; ++i_x )
        {
            for( int i_y = 0; i_y < ySlotNum; ++i_y )
            {
                if( input[ input_index ] != "" )
                {
                    int number = Int32.Parse( input[ input_index ] );

                    if( number == 0 )
                    {
                        string message = WK.Translate.TranslateManager.Instance.GetText( "2500" );
                        CommonDialogManager.Instance.SetDialog( message, null );
                        CommonDialogManager.Instance.EnterNotationDialog();
                        CommonDialogManager.Instance.SetExitCompleteCallback( () => {
                                ResumeManager.Instance.Clear();
                                Application.Quit();
                                }
                                );
                        break;
                    }

                    GameObject block = StockFieldController.Instance.CreateBlock();
                    var bctrl = block.GetComponent<BlockController>();
                    bctrl.SetNumberImidiately( number );
                    bctrl.UpdateSkin();
                    Debug.Assert( slots[ i_x, i_y ] != null, "( x,y : " + i_x + "," + i_y + ")" );
                    block.transform.SetParent( slots[ i_x, i_y ].transform );
                    block.transform.localPosition = new Vector3( 0.0f, 0.0f, 0.0f );
                    block.transform.localScale = new Vector3 ( 1.0f, 1.0f, 1.0f );
                    bctrl.RequestDragable( false );
                }
                input_index++;
            }
        }
	}

	//------------------------------------------------------------------------------
	public string GetSaveData() {
        string str = "";
        for( int i_x = 0; i_x < xSlotNum; ++i_x )
        {
            for( int i_y = 0; i_y < ySlotNum; ++i_y )
            {
                GameObject block = slots[ i_x, i_y ].block;
                if( block )
                {
                    str += block.GetComponent<BlockController>().number.ToString();
                }
                str += ",";
            }
        }
        return str;
	}

	//------------------------------------------------------------------------------
	public string GetClearSaveDataStr() {
        string str = "";
        for( int i_x = 0; i_x < xSlotNum; ++i_x )
        {
            for( int i_y = 0; i_y < ySlotNum; ++i_y )
            {
                str += ",";
            }
        }
        return str;
	}

	//------------------------------------------------------------------------------
    private bool isKeepFree()
    {
        SlotController slot = GameObject.Find( "KeepField" ).GetComponent< SlotController >();
        return slot.block == null;
    }

	//------------------------------------------------------------------------------
    public void SetSlotColorNormal()
    {
		for( int i_x = 0; i_x < xSlotNum; ++i_x )
		{
			for( int i_y = 0; i_y < ySlotNum; ++i_y )
			{
				slots[ i_x, i_y ].SetColorNormal();
			}
		}
    }

    //------------------------------------------------------------------------------
	public void UpdateSkin () {
		for( int i_x = 0; i_x < MaxSlotNumX; ++i_x )
		{
			for( int i_y = 0; i_y < MaxSlotNumY; ++i_y )
			{
                if( slots[ i_x, i_y ] != null )
                {
                    slots[ i_x, i_y ].UpdateSkin();
                    BlockController block = slots[ i_x, i_y ].GetComponentInChildren< BlockController >();
                    if( block != null )
                    {
                        block.UpdateSkin();
                    }
                }
			}
		}
    }

	//------------------------------------------------------------------------------
    /* private bool predictMated() */
    /* { */
    /*     int free_space_count = GetFreeSpaceCount() + isKeepFree() ? 1 : 0; */
    /*     if( free_space_count == 4 ) */
    /*     { */

    /*     } */
    /* } */

}
