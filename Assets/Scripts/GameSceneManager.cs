//#define ENABLE_FIREBASE

//#define AD_CHECK
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System.Collections;
using WK.Unity;
using WK.Audio;
using WK.Translate;
using WK.Collections;
using Sirenix.OdinInspector;
using System.Reflection;
using UnityEngine.Purchasing;

#if ENABLE_FIREBASE
using Firebase;
using Firebase.Analytics;
#endif

public enum EGameMode
{
    three,
    four,
};

public class GameSceneManager : SingletonMonoBehaviour<GameSceneManager>
{
    public GameObject introdcutionFormula;

    private int currScore;
    private int bestScore;
    private int bestScore4;
    private int bestScoreTimeLimit;


    private const int cAppVersion = 20200125;
    private int version = 0;

    private SimpleSceneObjectController gameScene;
    private SimpleSceneObjectController resultScene;
    private SlideInSceneObjController menuScene;
    private SlideInSceneObjController menuSkinScene;
    private SlideInSceneObjController purchaseScene;
    private SlideInSceneObjController playAdsScene;
    private SlideInSceneObjController dustbinMenuScene;
    private SlideInSceneObjController updateDialog;

    [SerializeField] public int RecieveRankingSpan = 60 * 60 * 3;

    /* [SerializeField] */
    /* private GameObject[] catPainterLink; */

    /* [SerializeField] */
    /* private GameObject[] solokusLink; */

    [SerializeField] private GameObject timeLimitBg;

    [SerializeField] private Text modeLabel;

    [SerializeField] private TimeBarController timeBarCtrl;

    [SerializeField] private CanvasScaler canvasScaler;

    [SerializeField] private Text messagePurchase;

    [SerializeField] private Text skinPrice;

    private TagProcessor tagProcessor;

    public TagProcessor GetTagProcessor()
    {
        return tagProcessor;
    }

    private const int eStateIntroduction = 0;
    private const int eStateGame = 1;
    private const int eStateResult = 2;
    private const int eStateMenu = 3;
    private const int eStateDustbinMenu = 4;
    private const int eStateMenuFadeOut = 5;
    private const int eStateDustbinMenuFadeOut = 6;
    private const int eStateResultFadeOut = 7;
    private const int eStateThrowAwayToTheDustbin = 8;
    private const int eStateStore = 9;
    private const int eStateLicense = 10;
    private const int eStateConfirmQuit = 11;
    private const int eStateShowMovieForDustbin = 12;
    private const int eStateShowMovieForBackStep = 13;
    private const int eStateTitle = 14;
    private const int eStateLaunch = 15;
    private const int eStateFinished = 16;

    private int state = 0;
    private int nextState = 0;
    private int stateCounter = 0;
    private float stateTimer = 0;

    private int prevStateForDialog = 0;

    private bool isRequestReward = false;
    private bool isResetProgressBarDialog = false;

    private int[] numBackSteps = { 10, 5 };

    private DustbinFieldController dustbinFieldCtrl = null;
    private MoneySystem moneySystem;
    private AdsManager adsManager;

    private Text scoreText = null;
    private Text bestScoreText = null;

    public int BestScore
    {
        get { return bestScore; }
        protected set { bestScore = value; }
    }

    public int BestScore4
    {
        get { return bestScore4; }
        protected set { bestScore4 = value; }
    }

    public int BestScoreTimeLimit
    {
        get { return bestScoreTimeLimit; }
        protected set { bestScoreTimeLimit = value; }
    }

    public bool isFirstLaunch { get; protected set; }
    protected bool isReadResumeData = true;

    public bool isTutorialFinished
    {
        get { return PlayerPrefs.GetInt(PrefKeys.IS_TUTORIAL_FINISHED, 0) != 0; }
        set { PlayerPrefs.SetInt(PrefKeys.IS_TUTORIAL_FINISHED, value == true ? 1 : 0); }
    }

    public bool isRankingImplemented { get; protected set; }

    [ShowInInspector] public EGameMode GameMode { get; set; }


    public bool IsPurchasedNoAds
    {
        get
        {
#if AD_CHECK
            if (Debug.isDebugBuild)
            {
                return false;
            }
#endif
            return PlayerPrefs.GetInt(PrefKeys.IS_PURCHASED_NO_ADS, 0) == 1;
        }
    }

    public static readonly System.DateTime BASE_TIME = new System.DateTime(2020, 1, 1);

    /* public bool IsBestScoreSended { get { return PlayerPrefs.GetInt( BEST_SCORE_BE_SENDED, 0 ) == 1; } } */
    /* public bool IsBestScoreTimeLimitSended { get { return PlayerPrefs.GetInt( BEST_SCORE_TIME_LIMIT_BE_SENDED, 0 ) == 1; } } */

    private bool isBestScoreUpdated = false;

    private const int cNumRanks = 12;
    private Tuple<string, int>[] rankInfos = null;
    private Tuple<string, int>[] rankInfos4 = null;
    private Tuple<string, int>[] rankInfosTimeLimit = null;

#if ENABLE_FIREBASE
    //Firebase
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
#endif

    private const int cMaxUsingBackStep = 2;
    public int numUsingBackStepCount = 0;

#if UNITY_EDITOR
    public SystemLanguage debugLanguage = SystemLanguage.English;
    [RangeAttribute(2017, 2020)] public int debugDateTimeYear = 2018;
    [RangeAttribute(1, 12)] public int debugDateTimeMonth = 1;
    [RangeAttribute(1, 31)] public int debugDateTimeDay = 1;

    public bool isDebugSkipFirstLaunch = false;

    public bool isDebugClearBest = false;

    /* public bool isDebugClearAllData = false; */
    public bool isDebugAdResult = false;
    public bool isDebugAskReview = false;
    public bool isDebugBirthCounter = false;
    public int debugBirthCounter = 0;
    public bool isDebugScore = false;
    public int debugScore = 0;
#endif
    public bool isDebugDustBinSoon = false;

    public bool IsTimeLimit { get; set; }

    [SerializeField] protected SkinSelectGroup skinSelectGroup;

    [SerializeField] protected ScriptableObject[] databases;

    public void SetPurchasedNoAds()
    {
        PlayerPrefs.SetInt(PrefKeys.IS_PURCHASED_NO_ADS, 1);
    }

    public bool IsSubscribedNoAds
    {
        get
        {
            return (System.DateTime.UtcNow - BASE_TIME).TotalSeconds
                   - PlayerPrefs.GetInt(PrefKeys.EXPIRE_SEC_SUBSCRIPTION_NO_ADS, 0)
                   < 0;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        SaveData.LoadMain();

        Debug.unityLogger.logEnabled = Debug.isDebugBuild;


        //細長いスマホ対策
        //Zenfone AR(1440.0f : 2560.0f)よりアスペクト比が小さかったら
        if ((Screen.width / (float)Screen.height) < (1440.0f / 2560.0f))
        {
            canvasScaler.matchWidthOrHeight = 0.59f;
        }

        Config.androidAppId = "com.Kiversegame.divide";
        Config.iOSAppId = "1136604398";
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;
        stateCounter = 0;
        stateTimer = 0.0f;
        state = -1; //for enterIntroduction
        nextState = eStateIntroduction;

        version = PlayerPrefs.GetInt(PrefKeys.APP_VERSION, 0);
        PlayerPrefs.SetInt(PrefKeys.APP_VERSION, cAppVersion);

        GameMode = EGameMode.three;
        IsTimeLimit = false;

        /* #if UNITY_ANDROID */
        /*         Screen.fullScreen = false; */
        /* #endif */

        isFirstLaunch = PlayerPrefs.GetInt(PrefKeys.IS_FIRST_LAUNCH, 0) == 0;
        isReadResumeData = true;
        if (version != cAppVersion //ヴァージョンが変わった時はリセットする
            && !((version == 20170929)
                 && (cAppVersion == 20180105)
                 && (cAppVersion == 20191213)))
        {
            //このバージョン変化の時は、何もしなくて、そのままのデータでいける
            //isReadResumeData = false;
            //ResumeManager.Instance.Clear();
            PlayerPrefs.SetInt(PrefKeys.IS_RESUMABLE, 0);
        }

        bool is_resumable = PlayerPrefs.GetInt(PrefKeys.IS_RESUMABLE, 0) == 1;
        if (!is_resumable)
        {
            isReadResumeData = false;
            ResumeManager.Instance.Clear();
        }

        //ヴァージョンアップした人はチュートリアルスキップ
        //isTutorialFinished のセーブデータの名前を変えてしまった
        //ことによる不具合対処のため
        if ((version > 0) && version != cAppVersion)
        {
            isTutorialFinished = true;

            //セーブデータが変な状態になって
            //FirstLaunchがリセットされてしまうことがあるようなので
            //バージョンが変わってたら
            //isFirstLaunchをfalseにする
            isFirstLaunch = false;
            PlayerPrefs.SetInt(PrefKeys.IS_FIRST_LAUNCH, 1);
        }

        if (!isTutorialFinished)
        {
            ResumeManager.Instance.Clear();
        }

        isRankingImplemented = PlayerPrefs.GetInt(PrefKeys.IS_RANKING_IMPLEMENTED, 0) == 1;
        PlayerPrefs.SetInt(PrefKeys.IS_RANKING_IMPLEMENTED, 1);

        /* if(UnityEngine.Debug.isDebugBuild) */
        /* { */
        /*     Config.ChangeLanguage( SystemLanguage.ChineseSimplified ); */
        /* } */

#if UNITY_EDITOR
        Config.ChangeLanguage(debugLanguage);
        if (isDebugClearBest)
        {
            PlayerPrefs.SetInt(PrefKeys.BEST_SCORE_KEY, 0);
            PlayerPrefs.SetInt(PrefKeys.BEST_SCORE_V1_3_KEY, 0);
            PlayerPrefs.SetInt(PrefKeys.BEST_SCORE_4_KEY, 0);
            PlayerPrefs.SetInt(PrefKeys.BEST_SCORE_TIME_LIMIT_KEY, 0);
            PlayerPrefs.Save();
        }

        /* if( isDebugClearAllData ) */
        /* { */
        /*     PlayerPrefs.DeleteAll(); */
        /* } */
        if (isDebugSkipFirstLaunch)
        {
            isFirstLaunch = false;
        }
#endif

        /* if( Config.Language == SystemLanguage.Japanese ) */
        /* { */
        /*     for( int i = 0; i < catPainterLink.Length; ++i ) { catPainterLink[i].SetActive( true ); } */
        /*     for( int i = 0; i < solokusLink.Length; ++i ) { solokusLink[i].SetActive( false ); } */
        /* } */
        /* else */
        /* { */
        /*     for( int i = 0; i < catPainterLink.Length; ++i ) { catPainterLink[i].SetActive( false ); } */
        /*     for( int i = 0; i < solokusLink.Length; ++i ) { solokusLink[i].SetActive( true ); } */
        /* } */

        rankInfos = new Tuple<string, int>[cNumRanks];

        rankInfos[0] = new Tuple<string, int>("HHH", 0);
        rankInfos[1] = new Tuple<string, int>("H", 10);
        rankInfos[2] = new Tuple<string, int>("G", 150);
        rankInfos[3] = new Tuple<string, int>("F", 300);
        rankInfos[4] = new Tuple<string, int>("E", 500);
        rankInfos[5] = new Tuple<string, int>("D", 800);
        rankInfos[6] = new Tuple<string, int>("C", 1100);
        rankInfos[7] = new Tuple<string, int>("B", 1500);
        rankInfos[8] = new Tuple<string, int>("A", 4000);
        rankInfos[9] = new Tuple<string, int>("S", 8000);
        rankInfos[10] = new Tuple<string, int>("SS", 12000);
        rankInfos[11] = new Tuple<string, int>("SSS", 16000);

        rankInfos4 = new Tuple<string, int>[cNumRanks];
        rankInfos4[0] = new Tuple<string, int>("HHH", 0);
        rankInfos4[1] = new Tuple<string, int>("H", 100);
        rankInfos4[2] = new Tuple<string, int>("G", 500);
        rankInfos4[3] = new Tuple<string, int>("F", 1000);
        rankInfos4[4] = new Tuple<string, int>("E", 2000);
        rankInfos4[5] = new Tuple<string, int>("D", 3000);
        rankInfos4[6] = new Tuple<string, int>("C", 4000);
        rankInfos4[7] = new Tuple<string, int>("B", 8000);
        rankInfos4[8] = new Tuple<string, int>("A", 12000);
        rankInfos4[9] = new Tuple<string, int>("S", 20000);
        rankInfos4[10] = new Tuple<string, int>("SS", 30000);
        rankInfos4[11] = new Tuple<string, int>("SSS", 50000);

        rankInfosTimeLimit = new Tuple<string, int>[cNumRanks];
        rankInfosTimeLimit[0] = new Tuple<string, int>("HHH", 0);
        rankInfosTimeLimit[1] = new Tuple<string, int>("H", 10);
        rankInfosTimeLimit[2] = new Tuple<string, int>("G", 100);
        rankInfosTimeLimit[3] = new Tuple<string, int>("F", 150);
        rankInfosTimeLimit[4] = new Tuple<string, int>("E", 250);
        rankInfosTimeLimit[5] = new Tuple<string, int>("D", 350);
        rankInfosTimeLimit[6] = new Tuple<string, int>("C", 450);
        rankInfosTimeLimit[7] = new Tuple<string, int>("B", 600);
        rankInfosTimeLimit[8] = new Tuple<string, int>("A", 800);
        rankInfosTimeLimit[9] = new Tuple<string, int>("S", 1000);
        rankInfosTimeLimit[10] = new Tuple<string, int>("SS", 1500);
        rankInfosTimeLimit[11] = new Tuple<string, int>("SSS", 2000);
    }

    void Start()
    {
        int best_score = PlayerPrefs.GetInt(PrefKeys.BEST_SCORE_KEY, 0);
        int best_score_4 = PlayerPrefs.GetInt(PrefKeys.BEST_SCORE_4_KEY, 0);
        int best_score_time_limit = PlayerPrefs.GetInt(PrefKeys.BEST_SCORE_TIME_LIMIT_KEY, 0);
        currScore = 0;
        bestScore = best_score;
        bestScore4 = best_score_4;
        bestScoreTimeLimit = best_score_time_limit;
        isBestScoreUpdated = false;

        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        bestScoreText = GameObject.Find("BestText").GetComponent<Text>();
        scoreText.text = currScore.ToString();
        bestScoreText.text = bestScore.ToString();

        gameScene = GameObject.Find("Game").GetComponent<SimpleSceneObjectController>();
        menuScene = GameObject.Find("Menu").GetComponent<SlideInSceneObjController>();
        menuSkinScene = GameObject.Find("MenuSkin").GetComponent<SlideInSceneObjController>();
        purchaseScene = GameObject.Find("ConfirmPurchase").GetComponent<SlideInSceneObjController>();
        playAdsScene = GameObject.Find("ConfirmPlayAD").GetComponent<SlideInSceneObjController>();
        dustbinMenuScene = GameObject.Find("DustbinMenu").GetComponent<SlideInSceneObjController>();
        resultScene = GameObject.Find("Result").GetComponent<SimpleSceneObjectController>();
        updateDialog = GameObject.Find("UpdateFeatureDialog").GetComponent<SlideInSceneObjController>();
        /* introScene       = GameObject.Find( "Introduction" ).GetComponent< SimpleSceneObjectController >(); */

        gameScene.Init(true, true);
        menuScene.Init(false, false);
        menuSkinScene.Init(false, false);
        purchaseScene.Init(false, false);
        playAdsScene.Init(false, false);
        dustbinMenuScene.Init(false, false);
        resultScene.Init(false, false);
        updateDialog.Init(false, false);

        changeState(eStateLaunch);

        /* mainFieldCtrl = GameObject.Find( "MainField" ).GetComponent< MainFieldController >(); */
        dustbinFieldCtrl = GameObject.Find("DustbinField").GetComponent<DustbinFieldController>();
        moneySystem = GameObject.Find("MoneySystem").GetComponent<MoneySystem>();
        adsManager = GetComponent<AdsManager>();


        /* UnityAdListener.Instance.userCallbackOnFinished = OnCloseDustbinInterstitial; */
        /* UnityAdListener.Instance.userCallbackOnSkipped = OnFailedDustbinInterstitial; */
        /* UnityAdListener.Instance.userCallbackOnFailed = OnFailedDustbinInterstitial; */

        TranslateManager.Instance.Read("Text/texts");
        TranslateManager.Instance.ApplyAllUIText();
        tagProcessor = new TagProcessor();
        tagProcessor.Hour = (RecieveRankingSpan / (60 * 60)).ToString();
        TranslateManager.Instance.SetTagProcessor(tagProcessor);


        //if( IsPurchasedNoAds )
        // if( IsPurchasedNoAds || IsSubscribedNoAds )
        // {
        //     WK.AdListenerMax.Instance.HideBanner();
        // }
        Debug.unityLogger.logEnabled = true;
        print("IsNoAds:" + IsPurchasedNoAds + "," + IsSubscribedNoAds);
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;

        WK.AdListenerMax.Instance.IsNoAds = IsPurchasedNoAds || IsSubscribedNoAds;
        if (IsPurchasedNoAds || IsSubscribedNoAds)
        {
#if !AD_CHECK
            //すでにshowしてしまっていた場合のために一応Hideしておく
            //WK.AdListenerMax.Instance.HideBanner();
#endif
        }
#if AD_CHECK
        if (Debug.isDebugBuild)
        {
            WK.AdListenerMax.Instance.IsNoAds = false;
        }
#endif

        skinSelectGroup.SetSkinSelectFrame(SaveData.Data.skinIndex);

#if ENABLE_FIREBASE
        //Firebase
        dependencyStatus = FirebaseApp.CheckDependencies();
        if (dependencyStatus != DependencyStatus.Available) {
            FirebaseApp.FixDependenciesAsync().ContinueWith(task => {
                    dependencyStatus = FirebaseApp.CheckDependencies();
                    if (dependencyStatus == DependencyStatus.Available) {
                    InitializeFirebase();
                    } else {
                    Debug.LogError(
                            "Could not resolve all Firebase dependencies: " + dependencyStatus);
                    }
                    });
        } else {
            InitializeFirebase();
        }
#endif
    }

#if ENABLE_FIREBASE
  void InitializeFirebase() {
    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

    /* Debug.Log("Set user properties."); */
    /* // Set the user's sign up method. */
    /* FirebaseAnalytics.SetUserProperty( */
    /*   FirebaseAnalytics.UserPropertySignUpMethod, */
    /*   "Google"); */
    /* // Set the user ID. */
    /* FirebaseAnalytics.SetUserId("uber_user_510"); */
  }
#endif

    void Update()
    {
        ++stateCounter;
        stateTimer += Time.deltaTime;
        if (state != nextState)
        {
            state = nextState;
            stateCounter = 0;
            stateTimer = 0.0f;

            switch (state)
            {
                case eStateIntroduction:
                    enterIntroduction();
                    break;
                case eStateGame:
                    enterGame();
                    break;
                case eStateMenu:
                    enterMenu();
                    break;
                case eStateDustbinMenu:
                    enterDustbinMenu();
                    break;
                case eStateResult:
                    enterResult();
                    break;
                case eStateMenuFadeOut:
                    enterMenuFadeOut();
                    break;
                case eStateDustbinMenuFadeOut:
                    enterDustbinMenuFadeOut();
                    break;
                case eStateResultFadeOut:
                    enterResultFadeOut();
                    break;
                case eStateThrowAwayToTheDustbin:
                    enterThrowAwayToTheDustbin();
                    break;
                case eStateShowMovieForDustbin:
                    break;
                case eStateShowMovieForBackStep:
                    break;
            }
        }

        switch (state)
        {
            case eStateIntroduction:
                updateIntroduction();
                break;
            case eStateGame:
                updateGame();
                break;
            case eStateMenu:
                updateMenu();
                break;
            case eStateDustbinMenu:
                updateDustbinMenu();
                break;
            case eStateResult:
                updateResult();
                break;
            case eStateThrowAwayToTheDustbin:
                updateThrowAwayToTheDustbin();
                break;
            case eStateMenuFadeOut:
                updateMenuFadeOut();
                break;
            case eStateDustbinMenuFadeOut:
                updateDustbinMenuFadeOut();
                break;
            case eStateResultFadeOut:
                updateResultFadeOut();
                break;
            case eStateShowMovieForDustbin:
                updateShowMovieForDustbin();
                break;
            case eStateShowMovieForBackStep:
                updateShowMovieForBackStep();
                break;
            case eStateLaunch:
                updateLaunch();
                break;
            case eStateTitle:
                break;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (state != eStateConfirmQuit)
            {
                SoundManager.Instance.PlaySe("click");
                DialogSceneManager.Instance.EnterConfirmQuit();
                prevStateForDialog = state;
                changeState(eStateConfirmQuit);
            }
            else
            {
                BackToTheGameFromDialog();
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Break();
        }
#endif
    }

    //------------------------------------------------------------------------------
    private void enterGame()
    {
        if (isReadResumeData)
        {
            //resume
            ResumeManager.Instance.ReadLatest();
            numUsingBackStepCount = PlayerPrefs.GetInt(PrefKeys.NUM_USING_BACK_STEP_COUNT, 0);

            isReadResumeData = false;
        }
    }

    //------------------------------------------------------------------------------
    private void enterMenu()
    {
    }

    //------------------------------------------------------------------------------
    private void enterDustbinMenu()
    {
    }

    //------------------------------------------------------------------------------
    private void enterResult()
    {
        //念のためクリアしておく
        ResumeManager.Instance.Clear();
    }

    //------------------------------------------------------------------------------
    private void enterMenuFadeOut()
    {
    }

    //------------------------------------------------------------------------------
    private void enterDustbinMenuFadeOut()
    {
    }

    //------------------------------------------------------------------------------
    private void enterResultFadeOut()
    {
    }

    //------------------------------------------------------------------------------
    private void enterThrowAwayToTheDustbin()
    {
    }

    //------------------------------------------------------------------------------
    private void enterIntroduction()
    {
        // 1, 1 にしかドロップできないようにする
        /* mainFieldCtrl.SetDroppable( false ); */
        /* mainFieldCtrl.SetDroppableXY( true, 1, 1 ); */
    }

    //------------------------------------------------------------------------------
    private void updateGame()
    {
        if (IsTimeLimit)
        {
            if (timeBarCtrl.IsFinished)
            {
                FinishedSceneManager.Instance.StartFinished();
                changeState(eStateFinished);
            }
        }
        else
        {
            if (dustbinFieldCtrl.IsAppearable())
            {
                if (!dustbinFieldCtrl.IsAppearing())
                {
                    dustbinFieldCtrl.Appear();
                }
            }
            else // when network is disconnected
            {
                if (dustbinFieldCtrl.IsAppearing())
                {
                    dustbinFieldCtrl.Disappear();
                }
            }
        }

        updateBestScore();
    }

    //------------------------------------------------------------------------------
    private void updateMenu()
    {
    }

    //------------------------------------------------------------------------------
    private void updateDustbinMenu()
    {
    }

    //------------------------------------------------------------------------------
    private void updateResult()
    {
    }

    //------------------------------------------------------------------------------
    private void updateThrowAwayToTheDustbin()
    {
#if UNITY_EDITOR
        /* const float cWaitTime = 3.0f; */
        /* if( stateTimer > cWaitTime ) */
        /* { */
        /*     OnCloseDustbinInterstitial(); */
        /* } */
#endif
    }

    //------------------------------------------------------------------------------
    /* public void OnCloseDustbinInterstitial() */
    /* { */
    /*     Destroy( BlockController.blockInDustbin ); */
    /*     BlockController.blockInDustbin = null; */
    /*     BackToTheGameFromDustbinMenu(); */
    /*     dustbinFieldCtrl.Disappear(); */
    /*     dustbinFieldCtrl.ChangeToNextDustbinCount(); */
    /*     GameObject.Find( "StockField" ).GetComponent< StockFieldController >().FillStocks(); */
    /*     ResumeManager.Instance.RequestUpdate(); */
    /* } */

    //------------------------------------------------------------------------------
    /* public void OnFailedDustbinInterstitial() */
    /* { */
    /*     dustbinFieldCtrl.Failed( () => BackToTheGameFromDustbinMenu() ); */
    /* } */

    //------------------------------------------------------------------------------
    private void updateIntroduction()
    {
        updateBestScore();

        if (IntrodcutionManager.Instance.IsFinished())
        {
            isReadResumeData = false; //Introのときはreadしない
            PlayerPrefs.SetInt(PrefKeys.IS_FIRST_LAUNCH, 1);
            isFirstLaunch = false;
            changeState(eStateGame);
            PlayerPrefs.SetInt(PrefKeys.IS_RESUMABLE, 1);
        }
    }

    //------------------------------------------------------------------------------
    private void updateMenuFadeOut()
    {
        if (menuScene.IsStateIdle()) // SlideOutが終了したら
        {
            changeState(eStateGame);
            MainFieldController.Instance.GoToIdle();
        }
    }

    //------------------------------------------------------------------------------
    private void updateDustbinMenuFadeOut()
    {
        if (dustbinMenuScene.IsStateIdle()) // SlideOutが終了したら
        {
            changeState(eStateGame);
            MainFieldController.Instance.GoToIdle();
        }
    }

    //------------------------------------------------------------------------------
    private void updateResultFadeOut()
    {
        if (resultScene.IsStateIdle()) // FadeOutが終了したら
        {
            changeState(eStateGame);
            MainFieldController.Instance.GoToIdle();
        }
    }

    //------------------------------------------------------------------------------
    private void updateShowMovieForDustbin()
    {
        if (isRequestReward)
        {
            //WK.AdListenerMax.Instance.UpdatePrevInterstitialAdTiming();
            Destroy(BlockController.blockInDustbin);
            BlockController.blockInDustbin = null;
            BackToTheGameFromDustbinMenu();
            dustbinFieldCtrl.Disappear();
            dustbinFieldCtrl.ChangeToNextDustbinCount();
            StockFieldController.Instance.FillStocks();
            ResumeManager.Instance.RequestUpdate();
            CommonDialogManager.Instance.ExitDialog();
        }
    }

    //------------------------------------------------------------------------------
    private void updateShowMovieForBackStep()
    {
        if (isRequestReward)
        {
            //WK.AdListenerMax.Instance.UpdatePrevInterstitialAdTiming();
            ProgressBarCommonDialog.Instance.ExitDialog();
            goBackStep(numBackSteps[numUsingBackStepCount]);
            changeState(eStateGame);
        }

        if (isResetProgressBarDialog)
        {
            ProgressBarCommonDialog.Instance.ResetBar();
            isResetProgressBarDialog = false;
        }
    }

    //------------------------------------------------------------------------------
    private void updateLaunch()
    {
        if (stateCounter == 0)
        {
            if (isFirstLaunch)
            {
                IntrodcutionManager.Instance.StartIntroduction();
                //TitleSceneManager.Instance.Exit( false );
                TitleSceneManager.Instance.GoToNormalMode(false);
                MainFieldController.Instance.GoToIdle();
                updateModeLayout();
                changeState(eStateIntroduction);
            }
            else
            {
                IntrodcutionManager.Instance.gameObject.SetActive(false);

                bool is_resumable = PlayerPrefs.GetInt(PrefKeys.IS_RESUMABLE, 0) == 1;
                if (is_resumable)
                {
                    TitleSceneManager.Instance.Exit(false);
                    updateModeLayout();
                    changeState(eStateGame);
                }
                else
                {
                    changeState(eStateTitle);
                }
            }
        }
    }

    //------------------------------------------------------------------------------
    //------------------------------------------------------------------------------
    //------------------------------------------------------------------------------
    private void changeState(int st)
    {
        nextState = st;
    }

    //------------------------------------------------------------------------------
    private void updateBestScore()
    {
        currScore = MainFieldController.Instance.Score;
        scoreText.text = currScore.ToString();

#if UNITY_EDITOR
        if (isDebugScore)
        {
            return;
        }
#endif

        if (CalcWeekId() > getWeekBestWeek())
        {
            setWeekBestWeek(CalcWeekId());

            RankingSceneManager.Instance.ClearSendedScore();
            RankingSceneManager.Instance.ClearReservedSendedScore();
            setWeekBest(0);
            setTimeLimitWeekBest(0);
        }

        if (IsTimeLimit)
        {
            if (currScore > bestScoreTimeLimit)
            {
                isBestScoreUpdated = true;
                bestScoreTimeLimit = currScore;
                bestScoreText.text = bestScoreTimeLimit.ToString();

                PlayerPrefs.SetInt(PrefKeys.BEST_SCORE_TIME_LIMIT_KEY, currScore);
                /* PlayerPrefs.SetInt( BEST_SCORE_TIME_LIMIT_BE_SENDED, 0 ); */
                /* PlayerPrefs.Save(); */
            }

            if (currScore > getTimeLimitWeekBest())
            {
                setTimeLimitWeekBest(currScore);
            }
        }
        else if (GameMode == EGameMode.four)
        {
            if (currScore > bestScore4)
            {
                isBestScoreUpdated = true;
                bestScore4 = currScore;
                bestScoreText.text = bestScore4.ToString();

                PlayerPrefs.SetInt(PrefKeys.BEST_SCORE_4_KEY, currScore);
            }

            if (currScore > getWeekBest4())
            {
                setWeekBest4(currScore);
            }
        }
        else
        {
            if (currScore > bestScore)
            {
                isBestScoreUpdated = true;
                bestScore = currScore;
                bestScoreText.text = bestScore.ToString();

                PlayerPrefs.SetInt(PrefKeys.BEST_SCORE_KEY, currScore);
                PlayerPrefs.SetInt(PrefKeys.BEST_SCORE_V1_3_KEY, currScore);
                /* PlayerPrefs.SetInt( BEST_SCORE_BE_SENDED, 0 ); */
                /* PlayerPrefs.Save(); */
            }

            if (currScore > getWeekBest())
            {
                setWeekBest(currScore);
            }
        }
    }

    //------------------------------------------------------------------------------
    public void StartTimer()
    {
        timeBarCtrl.StartTimer();
    }

    //------------------------------------------------------------------------------
    public int GetBestScoreAfterVersion1_3()
    {
        return PlayerPrefs.GetInt(PrefKeys.BEST_SCORE_V1_3_KEY, 0);
    }

    //------------------------------------------------------------------------------
    public int GetCurrentScore()
    {
        return currScore;
    }

    //------------------------------------------------------------------------------
    /* public void SetBestScoreSended() */
    /* { */
    /*     PlayerPrefs.SetInt( BEST_SCORE_BE_SENDED, 1 ); */
    /* } */

    /* //------------------------------------------------------------------------------ */
    /* public void SetBestScoreTimeLimitSended() */
    /* { */
    /*     PlayerPrefs.SetInt( BEST_SCORE_TIME_LIMIT_BE_SENDED, 1 ); */
    /* } */

    //------------------------------------------------------------------------------
    public void goToResultImpl()
    {
        changeState(eStateResult);

        PlayerPrefs.SetInt(PrefKeys.IS_RESUMABLE, 0);
        RankingSceneManager.Instance.SetScore(IsTimeLimit, GameMode, currScore);
        Text result_score_label = GameObject.Find("ResultScoreLabel").GetComponent<Text>();
        modeLabel.text = GameMode == EGameMode.four ? "- 4 x 4 Mode -" : "- 3 x 3 Mode -";

        if (IsTimeLimit)
        {
            modeLabel.text = "- Time Limit Mode -";
        }

        if (isBestScoreUpdated)
        {
            result_score_label.text = "New Best Score";
            DebugConsole.Instance.Write("Report " + ((long)currScore).ToString(), 180);

            /* RankingManager.Instance.Report( (long)currScore ); */

            /* RankingManager.Instance.ReportBestScore(); */
        }
        else
        {
            result_score_label.text = "Score";
        }

        GameObject.Find("ResultScoreText").GetComponent<Text>().text = currScore.ToString();

        GameObject.Find("ResultRankText").GetComponent<Text>().text = getRankName(currScore);

        //moneySystem.AddMoney(getRewardScore(currScore));

        resultScene.FadeIn().OnComplete(() =>
        {
            //if( !IsPurchasedNoAds )
            if (!(IsPurchasedNoAds || IsSubscribedNoAds))
            {
                //WK.AdListenerMax.Instance.ShowInterstitial();
            }
        });
    }

    private string getRankName(int score)
    {
        string rank = "";
        if (IsTimeLimit)
        {
            foreach (var v in rankInfosTimeLimit)
            {
                if (currScore >= v.Item2)
                {
                    rank = v.Item1;
                }
                else
                {
                    break;
                }
            }
        }
        else if (GameMode == EGameMode.four)
        {
            foreach (var v in rankInfos4)
            {
                if (currScore >= v.Item2)
                {
                    rank = v.Item1;
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            foreach (var v in rankInfos)
            {
                if (currScore >= v.Item2)
                {
                    rank = v.Item1;
                }
                else
                {
                    break;
                }
            }
        }

        return rank;
    }

    //private int getRewardScore(int score)
    //{
    //    int reward = 0;
    //    if (IsTimeLimit)
    //    {
    //        foreach (var v in rankInfosTimeLimit)
    //        {
    //            if (currScore >= v.Item2)
    //            {
    //                reward = v.Item3;
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }
    //    }
    //    else if (GameMode == EGameMode.four)
    //    {
    //        foreach (var v in rankInfos4)
    //        {
    //            if (currScore >= v.Item2)
    //            {
    //                reward = v.Item3;
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        foreach (var v in rankInfos)
    //        {
    //            if (currScore >= v.Item2)
    //            {
    //                reward = v.Item3;
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }
    //    }

    //    return reward;
    //}

    //------------------------------------------------------------------------------
    private void goBackStep(int num_back_steps)
    {
        ResumeManager.Instance.ReadOldData(num_back_steps);
        ResumeManager.Instance.RequestUpdate();
        CommonDialogManager.Instance.SetExitCompleteCallback(() =>
        {
            StockFieldController.Instance.SetDraggable(true);
            MainFieldController.Instance.SetDroppable(true);
            MainFieldController.Instance.GoToIdle();
        }
        );
        CommonDialogManager.Instance.ExitDialog();

        numUsingBackStepCount++;
        //Debug.Log("numUsingBackStepCount++: " + numUsingBackStepCount );
        PlayerPrefs.SetInt(PrefKeys.NUM_USING_BACK_STEP_COUNT, numUsingBackStepCount);
    }

    //------------------------------------------------------------------------------
    // 結果画面へ
    [ContextMenu("GoToResult")]
    public void GoToResult()
    {
        //Debug.Log("GoToResult:" + numUsingBackStepCount );
        if ((numUsingBackStepCount < cMaxUsingBackStep)
            && ResumeManager.Instance.IsEnableBack(numBackSteps[numUsingBackStepCount])
            && !IsTimeLimit)
        {
            showAdConfirmForBackSteps();
            //CommonDialogManager.Instance.SetDialog( message
            //        , () => ShowRewardVideoForBackStep()
            //        , () => {
            //            CommonDialogManager.Instance.ExitDialog();
            //            goToResultImpl();
            //        }
            //        );
            //CommonDialogManager.Instance.EnterYesNoDialog();
        }
        else
        {
            goToResultImpl();
        }
    }

    //------------------------------------------------------------------------------
    private void showAdConfirmForBackSteps()
    {
        var msg_idx = 2300 + numUsingBackStepCount;
        var message = TranslateManager.Instance.GetText(msg_idx.ToString());

        bool is_enable_progress_bar = true;
        if (IsPurchasedNoAds || IsSubscribedNoAds)
        {
            int[] back_steps = new int[] { 10, 5 };
            tagProcessor.Num = back_steps[numUsingBackStepCount].ToString();
            message = TranslateManager.Instance.GetText("ask_retry");
            //is_enable_progress_bar = false;
        }

        ProgressBarCommonDialog.Instance.SetDialog(message
            , () => ShowRewardVideoForBackStep()
            , () =>
            {
                Debug.Log("ProgressBarCommonDialog.Instance.ExitDialog()");
                ProgressBarCommonDialog.Instance.ExitDialog();
                goToResultImpl();
            }
            , is_enable_progress_bar
        );
        ProgressBarCommonDialog.Instance.EnterYesNoDialog();
    }

    //------------------------------------------------------------------------------
    public void GoToStore()
    {
        SoundManager.Instance.PlaySe("click");
        prevStateForDialog = state;
        changeState(eStateStore);
        DialogSceneManager.Instance.EnterStore();
    }

    //------------------------------------------------------------------------------
    public void GoToMenu()
    {
        changeState(eStateMenu);
        menuScene.SlideIn();
    }

    //------------------------------------------------------------------------------
    public void GoToSkinMenu()
    {
        changeState(eStateMenu);
        menuSkinScene.SlideIn();
    }

    //------------------------------------------------------------------------------
    public void GoToPurchase()
    {
        changeState(eStateMenu);
        purchaseScene.SlideIn();
        var skin = GetCurrSkinInfo();

        messagePurchase.text = "Unlock skin " + skin.titleObjIndex.ToString();

        skinPrice.text = skin.price.ToString();
    }

    //------------------------------------------------------------------------------
    public void GoToPlayAds()
    {
        changeState(eStateMenu);
        playAdsScene.SlideIn();
    }

    public void UpdateDialog()
    {
        changeState(eStateMenu);
        updateDialog.SlideIn();
    }

    //------------------------------------------------------------------------------
    public SkinInfo GetCurrSkinInfo()
    {
        var index = SaveData.Data.skinIndex;
        return SkinInfoDatabase.Instance.SkinInfos[index];
    }

    public SkinInfoDatabase GetSkinInfo()
    {
        return SkinInfoDatabase.Instance;
    }

    //------------------------------------------------------------------------------
    public void GoToDustbinMenu()
    {
        changeState(eStateMenu);
        /* dustbinMenuScene.SlideIn(); */
    }

    //------------------------------------------------------------------------------
    public void GoToTitle()
    {
        SoundManager.Instance.PlaySe("click");
        changeState(eStateTitle);
        menuScene.SlideOut();
        TitleSceneManager.Instance.Enter(
            () =>
            {
                resultScene.SetInteractableAndBlock(false);
                resultScene.SetVisible(false);
            }
        );
    }

    //------------------------------------------------------------------------------
    private void updateModeLayout()
    {
        var keep_pos = KeepFieldController.Instance.transform.localPosition;
        var stock_pos = StockFieldController.Instance.transform.localPosition;
        var main_pos = MainFieldController.Instance.transform.localPosition;
        if (IsTimeLimit)
        {
            keep_pos.y = -780.0f;
            stock_pos.y = -780.0f;
            main_pos.y = 200.0f;
            timeBarCtrl.gameObject.SetActive(true);
            timeLimitBg.SetActive(true);
        }
        else
        {
            keep_pos.y = -620.0f;
            stock_pos.y = -620.0f;
            main_pos.y = 360.0f;
            timeBarCtrl.gameObject.SetActive(false);
            timeLimitBg.SetActive(true);
        }

        KeepFieldController.Instance.transform.localPosition = keep_pos;
        StockFieldController.Instance.transform.localPosition = stock_pos;
        MainFieldController.Instance.transform.localPosition = main_pos;
    }

    //------------------------------------------------------------------------------
    private void ResetParams(int grid_size)
    {
        SoundManager.Instance.PlaySe("click");
        KeepFieldController.Instance.ResetParams();
        StockFieldController.Instance.ResetParams();
        StockFieldController.Instance.FillStocksImidiately();
        scoreText.text = "0";
        MainFieldController.Instance.Ready(grid_size);

        currScore = 0;
        isBestScoreUpdated = false;
        numUsingBackStepCount = 0;
        PlayerPrefs.SetInt(PrefKeys.NUM_USING_BACK_STEP_COUNT, numUsingBackStepCount);
        MFSlotController.ClearDropCounter();
        dustbinFieldCtrl.ResetParams();
        ResumeManager.Instance.Clear();

        InvalidateBestScoreText();

        updateModeLayout();
    }

    public void InvalidateBestScoreText()
    {
        if (IsTimeLimit)
        {
            bestScoreText.text = bestScoreTimeLimit.ToString();
        }
        else if (GameMode == EGameMode.four)
        {
            bestScoreText.text = bestScore4.ToString();
        }
        else
        {
            bestScoreText.text = bestScore.ToString();
        }
    }

    // 再プレイ処理
    public void TryAgain()
    {
        int grid_size = GameMode == EGameMode.three ? 3 : 4;
        ResetParams(grid_size);
        resultScene.FadeOut().OnComplete(() =>
        {
            AskReviewSceneManager.Instance.IncrementAskCounter();
            if (!IsTimeLimit)
            {
                if (AskReviewSceneManager.Instance.IsAskTiming())
                {
                    AskReviewSceneManager.Instance.AskToReview();
                }
            }
        }
        );
        changeState(eStateResultFadeOut);
        if (IsTimeLimit)
        {
            timeBarCtrl.ResetTimer();
            CountDownSceneManager.Instance.StartCountDown();
        }
    }

    //------------------------------------------------------------------------------
    // やり直し処理
    public void Restart()
    {
        int grid_size = GameMode == EGameMode.three ? 3 : 4;
        ResetParams(grid_size);
        PlayerPrefs.SetInt(PrefKeys.IS_RESUMABLE, IsTimeLimit ? 0 : 1);
        menuScene.SlideOut();
        changeState(eStateMenuFadeOut);
        //Debug.Log( "Restart time limit : " + IsTimeLimit );
        if (IsTimeLimit)
        {
            timeBarCtrl.ResetTimer();
            CountDownSceneManager.Instance.StartCountDown();
        }
    }

    //------------------------------------------------------------------------------
    // ゲームに戻る処理
    public void BackToTheGame()
    {
        SoundManager.Instance.PlaySe("click");
        menuScene.SlideOut();
        changeState(eStateMenuFadeOut);
    }

    //------------------------------------------------------------------------------
    // ゲームに戻る処理
    public void BackToTheMainMenu()
    {
        SoundManager.Instance.PlaySe("click");
        menuSkinScene.SlideOut();
        changeState(eStateMenuFadeOut);
    }

    //------------------------------------------------------------------------------
    // ゲームに戻る処理
    public void BackToTheSkinMenu()
    {
        SoundManager.Instance.PlaySe("click");
        purchaseScene.SlideOut();
        playAdsScene.SlideOut();
        changeState(eStateMenuFadeOut);
    }

    //------------------------------------------------------------------------------
    // ゲームに戻る処理
    public void BackToTheGameFromDialog()
    {
        SoundManager.Instance.PlaySe("click");
        DialogSceneManager.Instance.Exit(() => { changeState(prevStateForDialog); }
        );
    }

    public void CloseDialog()
    {
        SoundManager.Instance.PlaySe("click");
        updateDialog.SlideOut();
        changeState(eStateMenuFadeOut);
    }

    //------------------------------------------------------------------------------
    // ゲームに戻る処理
    public void BackToTheGameFromDustbinMenu()
    {
        SoundManager.Instance.PlaySe("click");
        /* dustbinMenuScene.SlideOut(); */
        /* CommonDialogManager.Instance.ExitDialog(); */
        if (BlockController.blockInDustbin != null) //削除されている場合もある
        {
            //@todo BackFromDustbinこの処理を見直し。処理が遅い時に壊れる？
            BlockController.blockInDustbin.GetComponent<BlockController>().BackFromDustbin(
                () => CommonDialogManager.Instance.ExitDialog()
            );
            BlockController.blockInDustbin = null;
        }
        else
        {
            CommonDialogManager.Instance.ExitDialog();
        }

        changeState(eStateDustbinMenuFadeOut);
    }

    //------------------------------------------------------------------------------
    // ゲームに戻る処理
    /* public void ShowMovieAdAndDeletePanel() */
    /* { */
    /* SoundManager.Instance.PlaySe( "click" ); */
    /* /1* AdListener.Instance.ShowInterstitialOnlyMovie(); *1/ */
    /* UnityAdListener.Instance.Show(); */
    /* 	changeState( eStateThrowAwayToTheDustbin ); */
    /* } */
    //------------------------------------------------------------------------------
    public void ShowRewardVideoForBackStep()
    {
        ProgressBarCommonDialog.Instance.StopBar();
        bool has_seen_video = false;
        isRequestReward = false;
        isResetProgressBarDialog = false;

        //WK.AdListenerMax.Instance.userCallbackOnReward = () =>
        adsManager.interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            has_seen_video = true;
            Debug.Log("Mở Ads");
        };
        //WK.AdListenerMax.Instance.userCallbackOnRewardClose = () =>
        adsManager.interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            if (has_seen_video)
            {
                isRequestReward = true;
                Debug.Log("Đã xem Ads");
            }
            else
            {
                isResetProgressBarDialog = true;
                Debug.Log("Chưa xem Ads");
            }
        };

        //if( IsPurchasedNoAds )
        if (IsPurchasedNoAds || IsSubscribedNoAds)
        {
            //WK.AdListenerMax.Instance.userCallbackOnReward();
            //WK.AdListenerMax.Instance.userCallbackOnRewardClose();
            adsManager.RegisterEventHandlers(adsManager.interstitialAd);
            changeState(eStateShowMovieForBackStep);
            return;
        }

        /* AdListener.Instance.userCallbackOnRewardFailed = () => { */
        /*     var message = TranslateManager.Instance.GetText( "6202" ); */
        /*     CommonDialogManager.Instance.SetDialog( message, null ); */
        /*     CommonDialogManager.Instance.EnterNotationDialog(); */

        /*     dustbinFieldCtrl.Failed( () => BackToTheGameFromDustbinMenu() ); */
        /* }; */
        /* AdListener.Instance.StartToLoadRewardVideo(); */
        //bool is_successed = WK.AdListenerMax.Instance.ShowRewardBasedVideo();
        bool is_successed = adsManager.ShowAd();
        if (!is_successed)
        {
            string message = TranslateManager.Instance.GetText("2200");
            CommonDialogManager.Instance.SetDialog(message, null);
            CommonDialogManager.Instance.EnterNotationDialog();
            CommonDialogManager.Instance.SetExitCompleteCallback(() => { Invoke("GoToResult", 0.1f); }
            );
        }
        else
        {
            changeState(eStateShowMovieForBackStep);
        }

        /* CommonDialogManager.Instance.EnterLoadingDialog(); */
    }


    //------------------------------------------------------------------------------
    public void ShowRewardVideoForDustbin()
    {
        bool has_seen_video = false;
        isRequestReward = false;

        //WK.AdListenerMax.Instance.userCallbackOnReward
        adsManager.interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            has_seen_video = true;
            Debug.Log("Mở Ads");
        };
        //WK.AdListenerMax.Instance.userCallbackOnRewardClose = () =>
        adsManager.interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            if (has_seen_video)
            {
                isRequestReward = true;
                Debug.Log("Đã xem Ads");
            }
            else
            {
                dustbinFieldCtrl.Failed(() => BackToTheGameFromDustbinMenu());
                Debug.Log("Chưa xem Ads");
            }
        };

        //if( IsPurchasedNoAds )
        if (IsPurchasedNoAds || IsSubscribedNoAds)
        {
            //WK.AdListenerMax.Instance.userCallbackOnReward();
            //WK.AdListenerMax.Instance.userCallbackOnRewardClose();
            adsManager.RegisterEventHandlers(adsManager.interstitialAd);
            changeState(eStateShowMovieForDustbin);
            return;
        }

        /* AdListener.Instance.userCallbackOnRewardFailed = () => { */
        /*     var message = TranslateManager.Instance.GetText( "6202" ); */
        /*     CommonDialogManager.Instance.SetDialog( message, null ); */
        /*     CommonDialogManager.Instance.EnterNotationDialog(); */

        /*     dustbinFieldCtrl.Failed( () => BackToTheGameFromDustbinMenu() ); */
        /* }; */
        /* AdListener.Instance.StartToLoadRewardVideo(); */

        //bool is_successed = WK.AdListenerMax.Instance.ShowRewardBasedVideo();
        bool is_successed = adsManager.ShowAd();
        if (!is_successed)
        {
            string message = TranslateManager.Instance.GetText("2200");
            CommonDialogManager.Instance.SetDialog(message, null);
            CommonDialogManager.Instance.EnterNotationDialog();
            dustbinFieldCtrl.Failed(() => BackToTheGameFromDustbinMenu());
        }
        else
        {
            changeState(eStateShowMovieForDustbin);
        }

        /* CommonDialogManager.Instance.EnterLoadingDialog(); */
    }

    //------------------------------------------------------------------------------
    public void PauseTimer()
    {
        if (IsTimeLimit)
        {
            timeBarCtrl.PauseTimer();
        }
    }

    //------------------------------------------------------------------------------
    public void ResumeTimer()
    {
        if (IsTimeLimit)
        {
            timeBarCtrl.ResumeTimer();
        }
    }

    //------------------------------------------------------------------------------
    public void BonusTimer(float bonus)
    {
        if (IsTimeLimit)
        {
            timeBarCtrl.BonusTimer(bonus);
        }
    }

    //------------------------------------------------------------------------------
    [ContextMenu("Dump")]
    private void Dump()
    {
        Debug.Log("DropCounter : " + MFSlotController.GetDropCounter());
        Debug.Log("Next Dust : " + DustbinFieldController.Instance.NextDustbinEnableCount);
        Debug.Log("WeekBestWeek : " + getWeekBestWeek());
        Debug.Log("WeeklyBest : " + getWeekBest());
    }

    //------------------------------------------------------------------------------
    [ContextMenu("ClearSaveData")]
    private void ClearSaveData()
    {
        PlayerPrefs.DeleteAll();
    }

    //------------------------------------------------------------------------------
    private System.DateTime getDateTimeNow()
    {
#if UNITY_EDITOR
        return new System.DateTime(
            debugDateTimeYear,
            debugDateTimeMonth,
            debugDateTimeDay);
#else
        return System.DateTime.Now;
#endif
    }

    //------------------------------------------------------------------------------
    public int CalcWeekId()
    {
        System.TimeSpan span = getDateTimeNow() - new System.DateTime(2018, 1, 1);
        const int cDaysPerWeek = 7;
        int week = span.Days / cDaysPerWeek;
        return week;
    }

    //------------------------------------------------------------------------------
    private int getWeekBestWeek()
    {
        return PlayerPrefs.GetInt(PrefKeys.WEEK_BEST_WEEK_KEY, 0);
    }

    //------------------------------------------------------------------------------
    private void setWeekBestWeek(int week)
    {
        PlayerPrefs.SetInt(PrefKeys.WEEK_BEST_WEEK_KEY, week);
    }

    //------------------------------------------------------------------------------
    private int getWeekBest()
    {
        return PlayerPrefs.GetInt(PrefKeys.WEEK_BEST_KEY, 0);
    }

    //------------------------------------------------------------------------------
    private void setWeekBest(int best)
    {
        PlayerPrefs.SetInt(PrefKeys.WEEK_BEST_KEY, best);
    }

    /* //------------------------------------------------------------------------------ */
    /* private int getTimeLimitWeekBestWeek() { */
    /*     return PlayerPrefs.GetInt(PrefKeys.WEEK_TIME_LIMIT_BEST_WEEK_KEY,0); */
    /* } */

    /* //------------------------------------------------------------------------------ */
    /* private void setTimeLimitWeekBestWeek(int week) { */
    /*     PlayerPrefs.SetInt(PrefKeys.WEEK_TIME_LIMIT_BEST_WEEK_KEY,week); */
    /* } */

    //------------------------------------------------------------------------------
    private int getWeekBest4()
    {
        return PlayerPrefs.GetInt(PrefKeys.WEEK_BEST4_KEY, 0);
    }

    //------------------------------------------------------------------------------
    private void setWeekBest4(int best)
    {
        PlayerPrefs.SetInt(PrefKeys.WEEK_BEST4_KEY, best);
    }

    //------------------------------------------------------------------------------
    private int getTimeLimitWeekBest()
    {
        return PlayerPrefs.GetInt(PrefKeys.WEEK_TIME_LIMIT_BEST_KEY, 0);
    }

    //------------------------------------------------------------------------------
    private void setTimeLimitWeekBest(int best)
    {
        PlayerPrefs.SetInt(PrefKeys.WEEK_TIME_LIMIT_BEST_KEY, best);
    }

    /* void OnApplicationPause( bool is_pause ) { */
    /*     if( is_pause ) */
    /*     { */
    /*         Debug.Log("OnApplicationPause"); */
    /*         PlayerPrefs.Save(); */
    /*     } */
    /* } */

    public void AddRewardGP()
    {
        if (adsManager.interstitialAd == null || !adsManager.interstitialAd.CanShowAd())
        {
            adsManager.LoadInterstitialAd();
        }

        if (adsManager.ShowAd())
            moneySystem.AddMoney(25);

        BackToTheSkinMenu();
    }

    public void PurchaseSkin()
    {
        var skinList = GetSkinInfo();
        var skin = GetCurrSkinInfo();

        skinList.UpdateSkinPurchasedStatus(skin.titleObjIndex);

        BackToTheSkinMenu();
    }
}