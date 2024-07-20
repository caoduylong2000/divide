using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using WK.Unity;
using WK.Audio;
using WK.Translate;
//using NCMB;


public class RankingSceneManager : SingletonMonoBehaviour<RankingSceneManager>{
    protected class RankingData {
        public string ObjectId;
        public string Name;
        public int Score;
    };

    [SerializeField]
    protected SlideInSceneObjController sceneObj;

    [SerializeField]
    protected VerticalLayoutGroup elementRoot;

    [SerializeField]
    protected GameObject rankElementPrehab;

    /* public static readonly int cRecieveRankingSpan = 3; */

    /* public int SendedScore { get{ return PlayerPrefs.GetInt( PrefKeys.RANKING_SENDED_SCORE, 0 ); } */
    /*     protected set { PlayerPrefs.SetInt( PrefKeys.RANKING_SENDED_SCORE, value ); } } */

    /* public int ReservedSendingScore { get{ return PlayerPrefs.GetInt( PrefKeys.RANKING_RESERVED_SENDING_SCORE, 0 ); } */
    /*     protected set { PlayerPrefs.SetInt( PrefKeys.RANKING_RESERVED_SENDING_SCORE, value ); } } */

    /* public string RankingObjectId { get{ return PlayerPrefs.GetString( PrefKeys.RANKING_OBJ_ID, "" ); } */
    /*     protected set { PlayerPrefs.SetString( PrefKeys.RANKING_OBJ_ID, value ); } } */

    /* public int SendedScoreTimeLimit { get{ return PlayerPrefs.GetInt( PrefKeys.RANKING_TIME_LIMIT_SENDED_SCORE, 0 ); } */
    /*     protected set { PlayerPrefs.SetInt( PrefKeys.RANKING_TIME_LIMIT_SENDED_SCORE, value ); } } */

    /* public int ReservedSendingScoreTimeLimit { get{ return PlayerPrefs.GetInt( PrefKeys.RANKING_TIME_LIMIT_RESERVED_SENDING_SCORE, 0 ); } */
    /*     protected set { PlayerPrefs.SetInt( PrefKeys.RANKING_TIME_LIMIT_RESERVED_SENDING_SCORE, value ); } } */

    /* public string RankingObjectIdTimeLimit { get{ return PlayerPrefs.GetString( PrefKeys.RANKING_TIME_LIMIT_OBJ_ID, "" ); } */
    /*     protected set { PlayerPrefs.SetString( PrefKeys.RANKING_TIME_LIMIT_OBJ_ID, value ); } } */

    protected int getSendedScore( bool is_time_limit, EGameMode game_mode )
    {
        if( is_time_limit ) { return PlayerPrefs.GetInt( PrefKeys.RANKING_TIME_LIMIT_SENDED_SCORE, 0 ); }
        else if( game_mode == EGameMode.four ) { return PlayerPrefs.GetInt( PrefKeys.RANKING_FOUR_SENDED_SCORE, 0 ); }
        else { return PlayerPrefs.GetInt( PrefKeys.RANKING_SENDED_SCORE, 0 ); }
    }

    protected int getReservedSendingScore( bool is_time_limit, EGameMode game_mode )
    {
        if( is_time_limit ) { return PlayerPrefs.GetInt( PrefKeys.RANKING_TIME_LIMIT_RESERVED_SENDING_SCORE, 0 ); }
        else if( game_mode == EGameMode.four ) { return PlayerPrefs.GetInt( PrefKeys.RANKING_FOUR_RESERVED_SENDING_SCORE, 0 ); }
        else { return PlayerPrefs.GetInt( PrefKeys.RANKING_RESERVED_SENDING_SCORE, 0 ); }
    }

    protected string getRankingObjectId( bool is_time_limit, EGameMode game_mode )
    {
        /* if( is_time_limit ) { return PlayerPrefs.GetString( PrefKeys.RANKING_TIME_LIMIT_OBJ_ID, "" ); } */
        /* else { return PlayerPrefs.GetString( PrefKeys.RANKING_OBJ_ID, "" ); } */

        if( is_time_limit ) { return PlayerPrefs.GetString( PrefKeys.RANKING_WEEKLY_TIME_LIMIT_OBJ_ID, "" ); }
        else if ( game_mode == EGameMode.four ) { return PlayerPrefs.GetString( PrefKeys.RANKING_FOUR_WEEKLY_OBJ_ID, "" ); }
        else { return PlayerPrefs.GetString( PrefKeys.RANKING_WEEKLY_OBJ_ID, "" ); }
    }

    public void ClearSendedScore()
    {
        PlayerPrefs.SetInt( PrefKeys.RANKING_TIME_LIMIT_SENDED_SCORE, 0 );
        PlayerPrefs.SetInt( PrefKeys.RANKING_FOUR_SENDED_SCORE, 0 );
        PlayerPrefs.SetInt( PrefKeys.RANKING_SENDED_SCORE, 0 );
    }

    public void ClearReservedSendedScore()
    {
        PlayerPrefs.SetInt( PrefKeys.RANKING_TIME_LIMIT_RESERVED_SENDING_SCORE, 0 );
        PlayerPrefs.SetInt( PrefKeys.RANKING_FOUR_RESERVED_SENDING_SCORE, 0 );
        PlayerPrefs.SetInt( PrefKeys.RANKING_RESERVED_SENDING_SCORE, 0 );
    }

    protected void setSendedScore( bool is_time_limit, EGameMode game_mode, int v )
    {
        if( is_time_limit ) { PlayerPrefs.SetInt( PrefKeys.RANKING_TIME_LIMIT_SENDED_SCORE, v ); }
        else if( game_mode == EGameMode.four ) { PlayerPrefs.SetInt( PrefKeys.RANKING_FOUR_SENDED_SCORE, v ); }
        else { PlayerPrefs.SetInt( PrefKeys.RANKING_SENDED_SCORE, v ); }
    }

    protected void setReservedSendingScore( bool is_time_limit, EGameMode game_mode, int v )
    {
        if( is_time_limit ) { PlayerPrefs.SetInt( PrefKeys.RANKING_TIME_LIMIT_RESERVED_SENDING_SCORE, v ); }
        else if( game_mode == EGameMode.four ) { PlayerPrefs.SetInt( PrefKeys.RANKING_FOUR_RESERVED_SENDING_SCORE, v ); }
        else { PlayerPrefs.SetInt( PrefKeys.RANKING_RESERVED_SENDING_SCORE, v ); }
    }

    protected void setRankingObjectId( bool is_time_limit, EGameMode game_mode, string v )
    {
        /* if( is_time_limit ) { PlayerPrefs.SetString( PrefKeys.RANKING_TIME_LIMIT_OBJ_ID, v ); } */
        /* else { PlayerPrefs.SetString( PrefKeys.RANKING_OBJ_ID, v ); } */

        if( is_time_limit ) { PlayerPrefs.SetString( PrefKeys.RANKING_WEEKLY_TIME_LIMIT_OBJ_ID, v ); }
        else if( game_mode == EGameMode.four ) { PlayerPrefs.SetString( PrefKeys.RANKING_FOUR_WEEKLY_OBJ_ID, v ); }
        else { PlayerPrefs.SetString( PrefKeys.RANKING_WEEKLY_OBJ_ID, v ); }
    }

    protected string getHighScoreQueryKey( bool is_time_limit, EGameMode game_mode )
    {
        /* if( is_time_limit ) { return NCMBQueryNames.HighScoreTimeLimit; } */
        /* else { return NCMBQueryNames.HighScore; } */

        if( is_time_limit ) { return NCMBQueryNames.WeeklyHighScoreTimeLimit; }
        if( game_mode == EGameMode.four ) { return NCMBQueryNames.WeeklyHighScore4; }
        else { return NCMBQueryNames.WeeklyHighScore; }
    }

    protected int getRankingNum( bool is_time_limit, EGameMode game_mode )
    {
        if( is_time_limit ) { return Mathf.Min( cMaxRanking, PlayerPrefs.GetInt( PrefKeys.TOP100_TIME_LIMIT_NUM, 0 ) ); }
        else if( game_mode == EGameMode.four ) { return Mathf.Min( cMaxRanking, PlayerPrefs.GetInt( PrefKeys.TOP100_FOUR_NUM, 0 ) ); }
        else { return Mathf.Min( cMaxRanking, PlayerPrefs.GetInt( PrefKeys.TOP100_NUM, 0 ) ); }
    }

    protected void setRankingNum( bool is_time_limit, EGameMode game_mode, int v )
    {
        if( is_time_limit ) { PlayerPrefs.SetInt( PrefKeys.TOP100_TIME_LIMIT_NUM, v ); }
        else if( game_mode == EGameMode.four ) { PlayerPrefs.SetInt( PrefKeys.TOP100_FOUR_NUM, v ); }
        else { PlayerPrefs.SetInt( PrefKeys.TOP100_NUM, v ); }
    }

    protected string getTop100Name( bool is_time_limit, EGameMode game_mode, int i )
    {
        if( is_time_limit ) { return PlayerPrefs.GetString( PrefKeys.TOP100_TIME_LIMIT_NAME + i, "" ); }
        else if( game_mode == EGameMode.four ) { return PlayerPrefs.GetString( PrefKeys.TOP100_FOUR_NAME + i, "" ); }
        else { return PlayerPrefs.GetString( PrefKeys.TOP100_NAME + i, "" ); }
    }

    protected void setTop100Name( bool is_time_limit, EGameMode game_mode, int i, string v )
    {
        if( is_time_limit ) { PlayerPrefs.SetString( PrefKeys.TOP100_TIME_LIMIT_NAME + i, v ); }
        else if( game_mode == EGameMode.four ) { PlayerPrefs.SetString( PrefKeys.TOP100_FOUR_NAME+ i, v ); }
        else { PlayerPrefs.SetString( PrefKeys.TOP100_NAME + i, v ); }
    }

    protected string getTop100ObjID( bool is_time_limit, EGameMode game_mode, int i )
    {
        if( is_time_limit ) { return PlayerPrefs.GetString( PrefKeys.TOP100_TIME_LIMIT_OBJ_ID + i, "" ); }
        else if( game_mode == EGameMode.four ) { return PlayerPrefs.GetString( PrefKeys.TOP100_FOUR_OBJ_ID + i, "" ); }
        else { return PlayerPrefs.GetString( PrefKeys.TOP100_OBJ_ID + i, "" ); }
    }

    protected void setTop100ObjID( bool is_time_limit, EGameMode game_mode, int i, string v )
    {
        if( is_time_limit ) { PlayerPrefs.SetString( PrefKeys.TOP100_TIME_LIMIT_OBJ_ID + i, v ); }
        else if( game_mode == EGameMode.four ) { PlayerPrefs.SetString( PrefKeys.TOP100_FOUR_OBJ_ID + i, v ); }
        else { PlayerPrefs.SetString( PrefKeys.TOP100_OBJ_ID + i, v ); }
    }

    protected int getTop100Score( bool is_time_limit, EGameMode game_mode, int i )
    {
        if( is_time_limit ) { return PlayerPrefs.GetInt( PrefKeys.TOP100_TIME_LIMIT_SCORE + i, 0 ); }
        else if( game_mode == EGameMode.four ) { return PlayerPrefs.GetInt( PrefKeys.TOP100_FOUR_SCORE + i, 0 ); }
        else { return PlayerPrefs.GetInt( PrefKeys.TOP100_SCORE + i, 0 ); }
    }

    protected void setTop100Score( bool is_time_limit, EGameMode game_mode, int i, int v )
    {
        if( is_time_limit ) { PlayerPrefs.SetInt( PrefKeys.TOP100_TIME_LIMIT_SCORE + i, v ); }
        else if( game_mode == EGameMode.four ) { PlayerPrefs.SetInt( PrefKeys.TOP100_FOUR_SCORE + i, v ); }
        else { PlayerPrefs.SetInt( PrefKeys.TOP100_SCORE + i, v ); }
    }

    protected string getPrevRankingReceiveTime( bool is_time_limit, EGameMode game_mode )
    {
        if( is_time_limit ) { return PlayerPrefs.GetString( PrefKeys.PREV_TIME_LIMIT_RANKING_RECEIVE_TIME, "1970,1,1,0,0,0" ); }
        else if( game_mode == EGameMode.four ) { return PlayerPrefs.GetString( PrefKeys.PREV_FOUR_RANKING_RECEIVE_TIME, "1970,1,1,0,0,0" ); }
        else { return PlayerPrefs.GetString( PrefKeys.PREV_RANKING_RECEIVE_TIME, "1970,1,1,0,0,0" ); }
    }

    protected void setPrevRankingReceiveTime( bool is_time_limit, EGameMode game_mode, string v )
    {
        if( is_time_limit ) { PlayerPrefs.SetString( PrefKeys.PREV_TIME_LIMIT_RANKING_RECEIVE_TIME, v ); }
        else if( game_mode == EGameMode.four ) { PlayerPrefs.SetString( PrefKeys.PREV_FOUR_RANKING_RECEIVE_TIME, v ); }
        else { PlayerPrefs.SetString( PrefKeys.PREV_RANKING_RECEIVE_TIME, v ); }
    }

    protected List<RankingData> rankingDatas = new List<RankingData>();

    protected List<RankElementController> rankElements = new List<RankElementController>();

    const int cMaxRanking = 100;
    /* const int cMaxRanking = 2; */

    protected enum EState {
        idle
        , send
        , receive
        , show
    }

    protected EState currState    = 0;
    protected EState nextState    = 0;
    protected int    stateCounter = 0;
    protected float  stateTimer   = 0;


    //------------------------------------------------------------------------------
    void Awake()
    {
        base.Awake();
        stateCounter = 0;
        stateTimer   = 0.0f;
        currState    = EState.idle;
        nextState    = EState.idle;
    }

    //------------------------------------------------------------------------------
    void Start()
    {
        sceneObj.Init( false, false );
        createElements();
    }

    //------------------------------------------------------------------------------
    //void Update()
    //{
    //    ++stateCounter;
    //    stateTimer += Time.deltaTime;
    //    if( currState != nextState )
    //    {
    //        currState    = nextState;
    //        stateCounter = 0;
    //        stateTimer   = 0.0f;
    //    }

    //    switch( currState )
    //    {
    //        case EState.idle:
    //            break;
    //        case EState.send:
    //            updateSend();
    //            break;
    //        case EState.receive:
    //            updateReceive();
    //            break;
    //        case EState.show:
    //            updateShow();
    //            break;
    //    }
    //}

    //------------------------------------------------------------------------------
    private void changeState( EState next_state )
    {
        nextState = next_state;
    }

    //------------------------------------------------------------------------------
    //private void updateSend()
    //{
    //    if( stateCounter == 0 )
    //    {
    //        Debug.Log( "updateSend" );
    //        if( !IsReceiveRankingTime( GameSceneManager.Instance.IsTimeLimit, GameSceneManager.Instance.GameMode ) )
    //        {
    //            changeState( EState.show );
    //            return;
    //        }

    //        if( InputNameSceneManager.Instance.IsNoRankingName() )
    //        {
    //            InputNameSceneManager.Instance.SetConfirmCalblack(
    //                    () => {
    //                    sendHighScore( GameSceneManager.Instance.IsTimeLimit, GameSceneManager.Instance.GameMode );
    //                    }
    //                    );
    //            InputNameSceneManager.Instance.Enter();
    //        }
    //        else
    //        {
    //            sendHighScore( GameSceneManager.Instance.IsTimeLimit, GameSceneManager.Instance.GameMode );
    //        }
    //    }
    //}

    //------------------------------------------------------------------------------
    //private void updateReceive()
    //{
    //    if( stateCounter == 0 )
    //    {
    //        Debug.Log( "updateReceive" );
    //        if( !IsReceiveRankingTime( GameSceneManager.Instance.IsTimeLimit, GameSceneManager.Instance.GameMode ) )
    //        {
    //            changeState( EState.show );
    //            return;
    //        }

    //        receiveRanking( GameSceneManager.Instance.IsTimeLimit, GameSceneManager.Instance.GameMode );
    //    }
    //}

    //------------------------------------------------------------------------------
    private void updateShow()
    {
        if( stateCounter == 0 )
        {
            //@memo SetActiveした直後に
            //minHeightのサイズを取得してもうまくいかなかったのでここで。
            updateLayout();
            //@memo 更新されなかったので...
            elementRoot.transform.localPosition = elementRoot.transform.localPosition + new Vector3( 1.0f, 0.0f, 0.0f );
            elementRoot.transform.localPosition = elementRoot.transform.localPosition - new Vector3( 1.0f, 0.0f, 0.0f );
        }
    }

    //------------------------------------------------------------------------------
    public void SetScore( bool is_time_limit, EGameMode game_mode, int score )
    {
        if( getSendedScore(is_time_limit, game_mode) < score )
        {
            if( getReservedSendingScore(is_time_limit, game_mode) < score )
            {
                /* ReservedSendingScore = score; */
                setReservedSendingScore(is_time_limit,game_mode,score);
            }
        }
    }

    //------------------------------------------------------------------------------
    [ContextMenu("SendHighScore")]
//    protected void sendHighScore( bool is_time_limit, EGameMode game_mode )
//    {
//        Debug.Log( "sendHighScore" );
//        string name = InputNameSceneManager.Instance.RankingName;
//#if UNITY_EDITOR
//        //デバッグ時しか通らないはず
//        if( name == "" )
//        {
//            CommonDialogManager.Instance.SetDialog(
//                    "ERROR 名前が未入力です"
//                    , null );
//            CommonDialogManager.Instance.EnterNotationDialog();
//            changeState( EState.idle );
//            return ;
//        }
//#endif
//        if( getReservedSendingScore(is_time_limit, game_mode) == 0 )
//        {
//            Debug.Log("No ReservedSendingScore");
//            changeState( EState.receive );
//            return ;
//        }

//        /* var obj = new NCMBObject( NCMBQueryNames.HighScore ); */
//        UnityEngine.Debug.Log( "getHighScoreQueryKey : " + getHighScoreQueryKey( is_time_limit, game_mode ) );
//        var obj = new NCMBObject( getHighScoreQueryKey( is_time_limit, game_mode ) );
//        obj[NCMBKeys.Name] = InputNameSceneManager.Instance.RankingName;
//        obj[NCMBKeys.Score] = getReservedSendingScore(is_time_limit,game_mode);
//        obj[NCMBKeys.Week] = GameSceneManager.Instance.CalcWeekId();
//        if( getRankingObjectId(is_time_limit,game_mode) != "" )
//        {
//            obj.ObjectId = getRankingObjectId(is_time_limit,game_mode);
//        }

//        CommonDialogManager.Instance.SetDialog(
//                TranslateManager.Instance.GetText( "2102" )
//                , null );
//        CommonDialogManager.Instance.EnterLoadingDialog();

//        //送信待ちに移行
//        obj.SaveAsync ((NCMBException error) => {
//                if (error != null) {
//                    // 失敗
//                    UnityEngine.Debug.Log( "send contest error :" + error.ErrorCode + "," + error.ErrorMessage );
//                    CommonDialogManager.Instance.SetDialog(
//                            TranslateManager.Instance.GetText( "2100" )
//                            , null );
//                    CommonDialogManager.Instance.EnterNotationDialog();

//                    changeState( EState.show );
//                } else {
//                    //成功
//                    CommonDialogManager.Instance.ExitDialog();

//                    if( getRankingObjectId(is_time_limit,game_mode) == "" )
//                    {
//                        setRankingObjectId(is_time_limit,game_mode,obj.ObjectId);
//                    }
//                    /* SendedScore = getReservedSendingScore(is_time_limit); */
//                    setSendedScore( is_time_limit, game_mode, getReservedSendingScore(is_time_limit,game_mode) );
//                    setReservedSendingScore(is_time_limit, game_mode, 0);

//                    changeState( EState.receive );
//                    /* ClearDisplay(); */
//                    /* PlayerPrefs.SetString( PrefKeys.ContestReceiveId, obj.ObjectId ); */
//                    /* Debug.Log( "send Data : object_id : " + obj.ObjectId ); */
//                    /* contestButton.ChangeState( ContestButtonController.EState.receiveWait ); */

//                    /* contestFinishTime = DateTime.Now; */
//                    /* contestFinishTime = contestFinishTime.AddMinutes( GameVariables.Instance.ContestDuration ); */
//                    /* PlayerPrefs.SetString( PrefKeys.ContestFinishTime, */
//                    /*         WK.Utils.DateTimeToString( contestFinishTime ) ); */
//                }
//                });

//    }

    //------------------------------------------------------------------------------
    //protected void receiveRanking( bool is_time_limit, EGameMode game_mode )
    //{
    //    Debug.Log( "receiveRanking" );
    //    /* var query = new NCMBQuery<NCMBObject>( NCMBQueryNames.HighScore ); */
    //    var query = new NCMBQuery<NCMBObject>( getHighScoreQueryKey( is_time_limit, game_mode ) );
    //    query.Limit = cMaxRanking;
    //    query.WhereEqualTo( NCMBKeys.Week, GameSceneManager.Instance.CalcWeekId() );
    //    query.OrderByDescending ( NCMBKeys.Score );

    //    CommonDialogManager.Instance.SetDialog(
    //            TranslateManager.Instance.GetText( "2102" )
    //            , null );
    //    CommonDialogManager.Instance.EnterLoadingDialog();

    //    query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {
    //            if (e != null) {
    //                CommonDialogManager.Instance.SetDialog(
    //                        TranslateManager.Instance.GetText( "2100" )
    //                        , null );
    //                CommonDialogManager.Instance.EnterNotationDialog();
    //                UnityEngine.Debug.Log( "query find failed:" + e.ErrorCode + "," + e.ErrorMessage );

    //                changeState( EState.show );
    //            } else {
    //                Debug.Log( "query find success" );

    //                rankingDatas.Clear();
    //                for( int i = 0; i < objList.Count; ++i )
    //                {
    //                    var data = new RankingData();
    //                    data.ObjectId = objList[i].ObjectId;
    //                    data.Name = System.Convert.ToString( ( objList[i] )[NCMBKeys.Name] );
    //                    data.Score = System.Convert.ToInt32( ( objList[i] )[NCMBKeys.Score] );
    //                    rankingDatas.Add( data );
    //                }

    //                rankingDatas.Sort( delegate( RankingData a, RankingData b) {
    //                        return b.Score - a.Score;
    //                        });
    //                writeTop100( is_time_limit, game_mode );
    //                setupRankElements();

    //                /* PlayerPrefs.SetString( PrefKeys.PREV_RANKING_RECEIVE_TIME, */
    //                /*         WK.Utils.Utils.DateTimeToString( DateTime.Now ) ); */
    //                setPrevRankingReceiveTime( is_time_limit, game_mode, WK.Utils.Utils.DateTimeToString( DateTime.Now ) );

    //                CommonDialogManager.Instance.SetDialog(
    //                        TranslateManager.Instance.GetText( "2103" )
    //                        , null );
    //                CommonDialogManager.Instance.EnterNotationDialog();
    //                changeState( EState.show );
    //            }
    //    } );
    //}

    //------------------------------------------------------------------------------
    protected void writeTop100( bool is_time_limit, EGameMode game_mode )
    {
        /* PlayerPrefs.SetInt( PrefKeys.TOP100_NUM, rankingDatas.Count ); */
        setRankingNum( is_time_limit, game_mode, rankingDatas.Count );
        Debug.Log( "writeTop100: " + rankingDatas.Count );
        for( int i = 0; i < rankingDatas.Count; ++i )
        {
            /* PlayerPrefs.SetString( PrefKeys.TOP100_NAME   + i, rankingDatas[i].Name     ); */
            /* PlayerPrefs.SetString( PrefKeys.TOP100_OBJ_ID + i, rankingDatas[i].ObjectId ); */
            /* PlayerPrefs.SetInt(    PrefKeys.TOP100_SCORE  + i, rankingDatas[i].Score    ); */
            setTop100Name(  is_time_limit, game_mode, i, rankingDatas[i].Name     );
            setTop100ObjID( is_time_limit, game_mode, i, rankingDatas[i].ObjectId );
            setTop100Score( is_time_limit, game_mode, i, rankingDatas[i].Score    );
            /* PlayerPrefs.SetString( PrefKeys.TOP100_NAME   + i, rankingDatas[i].Name     ); */
            /* PlayerPrefs.SetString( PrefKeys.TOP100_OBJ_ID + i, rankingDatas[i].ObjectId ); */
            /* PlayerPrefs.SetInt(    PrefKeys.TOP100_SCORE  + i, rankingDatas[i].Score    ); */
        }
    }

    //------------------------------------------------------------------------------
    protected void readTop100( bool is_time_limit, EGameMode game_mode )
    {
        rankingDatas.Clear();
        /* int num = Mathf.Min( cMaxRanking, PlayerPrefs.GetInt( PrefKeys.TOP100_NUM, 0 ) ); */
        int num = getRankingNum( is_time_limit, game_mode );
        Debug.Log( "readTop100 : " + num );
        for( int i = 0; i < num; ++i )
        {
            var data = new RankingData();

            /* data.Name     = PlayerPrefs.GetString( PrefKeys.TOP100_NAME   + i, "" ); */
            /* data.ObjectId = PlayerPrefs.GetString( PrefKeys.TOP100_OBJ_ID + i, "" ); */
            /* data.Score    = PlayerPrefs.GetInt(    PrefKeys.TOP100_SCORE  + i, 0  ); */
            data.Name     = getTop100Name(  is_time_limit, game_mode, i );
            data.ObjectId = getTop100ObjID( is_time_limit, game_mode, i );
            data.Score    = getTop100Score( is_time_limit, game_mode, i );

            rankingDatas.Add( data );
        }
        rankingDatas.Sort( delegate( RankingData a, RankingData b) {
                return b.Score - a.Score;
                });
        setupRankElements();
    }

    //------------------------------------------------------------------------------
    private void setupRankElements()
    {
        clearAllElements();
        for( int i = 0; i < rankingDatas.Count; ++i )
        {
            var data = rankingDatas[i];
            rankElements[i].Set( i, data.Name, data.Score );
            rankElements[i].gameObject.SetActive( true );
        }
        updateLayout();
    }

    //------------------------------------------------------------------------------
	protected void updateLayout() {
        var rect = elementRoot.GetComponent<RectTransform>();
        var size = rect.sizeDelta;
        size.y = elementRoot.minHeight;
        rect.sizeDelta = size;
    }

    //------------------------------------------------------------------------------
    private void createElements()
    {
        for( int i = 0; i < cMaxRanking; ++i )
        {
            var obj = Instantiate( rankElementPrehab );
            var ctrl = obj.GetComponent< RankElementController >();
            rankElements.Add( ctrl );
            obj.transform.SetParent( elementRoot.transform );
            obj.transform.localScale = Vector3.one;
        }
    }

    //------------------------------------------------------------------------------
    private void clearAllElements()
    {
        for( int i = 0; i < cMaxRanking; ++i )
        {
            rankElements[i].gameObject.SetActive( false );
        }
    }

    //------------------------------------------------------------------------------
    private bool IsReceiveRankingTime( bool is_time_limit, EGameMode game_mode )
    {
        /* string prev_time_str = PlayerPrefs.GetString( PrefKeys.PREV_RANKING_RECEIVE_TIME, "1970,1,1,0,0,0" ); */
        string prev_time_str = getPrevRankingReceiveTime( is_time_limit, game_mode );
        DateTime prev_date_time = WK.Utils.Utils.StringToDateTime( prev_time_str );
        DateTime now_date_time = DateTime.Now;
        TimeSpan span = now_date_time - prev_date_time;

        /* if( UnityEngine.Debug.isDebugBuild ) */
        /* { */
        /*     Debug.Log( "span : " + span.TotalMinutes + "," + prev_time_str ); */
        /*     return span.TotalMinutes > 1; */
        /* } */

        return span.TotalSeconds > GameSceneManager.Instance.RecieveRankingSpan;
    }

    //------------------------------------------------------------------------------
    [ContextMenu("Enter")]
	public void Enter() {
        SoundManager.Instance.PlaySe( "click" );
        sceneObj.SlideIn();
        readTop100( GameSceneManager.Instance.IsTimeLimit, GameSceneManager.Instance.GameMode );
        changeState( EState.send );
    }

    //------------------------------------------------------------------------------
    [ContextMenu("Exit")]
	public void Exit() {
        SoundManager.Instance.PlaySe( "click" );
        changeState( EState.idle );
        sceneObj.SlideOut();
    }

    //------------------------------------------------------------------------------
    [ContextMenu("Dump")]
    void Dump()
    {
        Debug.Log( "elementRoot.preferredHeight : " + elementRoot.preferredHeight );
        Debug.Log( "elementRoot.minHeight : "       + elementRoot.minHeight       );
        Debug.Log( "elementRoot.flexibleHeight : "  + elementRoot.flexibleHeight  );
        var rect = elementRoot.GetComponent<RectTransform>();
        var size = rect.sizeDelta;
        size.y = elementRoot.minHeight;
        rect.sizeDelta = size;
    }
}
