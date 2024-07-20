using UnityEngine;
using System.Collections;
using WK.Unity;

public class ResumeManager : SingletonMonoBehaviour<ResumeManager>
{
    private const string CURR_STEP_INDEX = "currStepIndex";
    private const string MAIN_FIELD_NUMBER_KEY = "mainfFieldNumberKey";
    private const string STOCK_FIELD_NUMBER_KEY = "stockfFieldNumberKey";
    private const string KEEP_FIELD_NUMBER_KEY = "keepfFieldNumberKey";
    private const string CURR_SCORE_KEY = "currScoreKey";
    private const string CURR_DROP_COUNT_KEY = "currDropCountKey";
    private const string NEXT_DUSTBIN_ENABLE_COUNT_KEY = "nextDustbinEnableCountKey";

    private const string BIRTH_COUNTER_KEY = "birthCounterKey";
    /* private const string RANDOM_SEED                   = "randomSeed"; */

    private string mainFieldData;
    private string stockFieldData;
    private int keepFieldData;
    private int score;
    private int dropCounter;
    private int dustBinCount;
    private int birthCounter;
    private int currStepIndex = 0;
    private const int cMaxBackSteps = 11;
    private bool isRequestUpdate = false;

    [SerializeField] private UndoButtonController undoButton;

    //------------------------------------------------------------------------------
    private int getStepIndexForSave(int step_index)
    {
        return (step_index + cMaxBackSteps) % cMaxBackSteps;
    }

    //------------------------------------------------------------------------------
    public void RequestUpdate()
    {
        isRequestUpdate = true;
    }

    //------------------------------------------------------------------------------
    void Start()
    {
        clearResumeData();
    }

    //------------------------------------------------------------------------------
    // updateされるのはdrop時のみ
    void Update()
    {
        if (isRequestUpdate)
        {
            /* Debug.Log( "ResumeManager : isRequestUpdate " ); */
            /* DebugConsole.Instance.Write( "ResumeManager : isRequestUpdate " ); */
            if (MainFieldController.Instance.IsStableIdle() && StockFieldController.Instance.IsStable())
            {
                updateResumeData();
                PlayerPrefs.SetInt(CURR_STEP_INDEX, currStepIndex);
                write(currStepIndex);
                currStepIndex++;
                undoButton.FinishedUndo(false);
                undoButton.Invalidate();
                isRequestUpdate = false;
            }
        }
    }

    //------------------------------------------------------------------------------
    [ContextMenu("ReadOldData10")]
    public void ReadOldData10()
    {
        ReadOldData(10);
    }

    [ContextMenu("ReadOldData9")]
    public void ReadOldData9()
    {
        ReadOldData(9);
    }

    [ContextMenu("ReadOldData8")]
    public void ReadOldData8()
    {
        ReadOldData(8);
    }

    [ContextMenu("ReadOldData7")]
    public void ReadOldData7()
    {
        ReadOldData(7);
    }

    [ContextMenu("ReadOldData6")]
    public void ReadOldData6()
    {
        ReadOldData(6);
    }

    [ContextMenu("ReadOldData5")]
    public void ReadOldData5()
    {
        ReadOldData(5);
    }

    [ContextMenu("ReadOldData4")]
    public void ReadOldData4()
    {
        ReadOldData(4);
    }

    [ContextMenu("ReadOldData3")]
    public void ReadOldData3()
    {
        ReadOldData(3);
    }

    [ContextMenu("ReadOldData2")]
    public void ReadOldData2()
    {
        ReadOldData(2);
    }

    [ContextMenu("ReadOldData1")]
    public void ReadOldData1()
    {
        ReadOldData(1);
    }

    //------------------------------------------------------------------------------
    public bool IsEnableBack(int num_back_steps)
    {
        if (num_back_steps >= cMaxBackSteps)
        {
            return false;
        }

        int curr_step_index = PlayerPrefs.GetInt(CURR_STEP_INDEX, 0);
        int back_index = getStepIndexForSave(curr_step_index - num_back_steps);
        //Debug.Log( "IsEnableBack : " + back_index );
        stockFieldData = PlayerPrefs.GetString(STOCK_FIELD_NUMBER_KEY + back_index, "");
        return stockFieldData != "";
    }

    //------------------------------------------------------------------------------
    public void ReadOldData(int num_back_steps)
    {
        currStepIndex = PlayerPrefs.GetInt(CURR_STEP_INDEX, 0);
        int back_index = getStepIndexForSave(currStepIndex - num_back_steps);
        read(back_index);

        //戻った分はデータ消す
        clearResumeData(); //この中でcurrStepIndexもリセットされてしまっている
        for (int i = 0; i < num_back_steps; ++i)
        {
            var clear_index = getStepIndexForSave(back_index + i + 1);
            write(clear_index);
        }

        currStepIndex = back_index;
        PlayerPrefs.SetInt(CURR_STEP_INDEX, currStepIndex);
        currStepIndex++;

        undoButton.FinishedUndo(true);
        undoButton.Invalidate();
        //Debug.Log( "after currStepIndex : " + currStepIndex );
    }

    //------------------------------------------------------------------------------
    public void ReadLatest()
    {
        currStepIndex = PlayerPrefs.GetInt(CURR_STEP_INDEX, 0);
        //StockFieldController.Instance.ResetStocks();
        /* Debug.Log( "ReadLatest:" ); */
        read(currStepIndex);
        currStepIndex++;

        undoButton.FinishedUndo(true);
        undoButton.Invalidate();
    }

    //------------------------------------------------------------------------------
    protected void read(int step_index)
    {
        step_index = getStepIndexForSave(step_index);
        //Debug.Log( "Read save data : " + step_index );
        mainFieldData = PlayerPrefs.GetString(MAIN_FIELD_NUMBER_KEY + step_index,
            MainFieldController.Instance.GetClearSaveDataStr());
        stockFieldData = PlayerPrefs.GetString(STOCK_FIELD_NUMBER_KEY + step_index, "");
        keepFieldData = PlayerPrefs.GetInt(KEEP_FIELD_NUMBER_KEY + step_index, 0);
        score = PlayerPrefs.GetInt(CURR_SCORE_KEY + step_index, 0);
        dropCounter = PlayerPrefs.GetInt(CURR_DROP_COUNT_KEY + step_index, 0);
        dustBinCount = PlayerPrefs.GetInt(NEXT_DUSTBIN_ENABLE_COUNT_KEY + step_index,
            DustbinFieldController.cFirstNextDustbinEnableCount);
        birthCounter = PlayerPrefs.GetInt(BIRTH_COUNTER_KEY + step_index, 0);
        /* DebugConsole.Instance.Write( "r mfd" + step_index + ":" + mainFieldData ); */
        /* var seed       = PlayerPrefs.GetInt(    RANDOM_SEED                   + step_index, 0                                                   ); */
        /* StockFieldController.Instance.BirthRandomSeed = seed; */
        /* UnityEngine.Random.seed = seed; */
        /* Debug.Log( "Read random seed: " + seed ); */

        MainFieldController.Instance.SetSaveData(mainFieldData);
        StockFieldController.Instance.InvalidateGameMode(); //上でGameModeが4か3かが決まるのでここで呼ぶ
        StockFieldController.Instance.ResetParams(); //これ呼ばないとprenumberに数字が入らない
        StockFieldController.Instance.FillStocksImidiately();
        StockFieldController.Instance.SetSaveData(stockFieldData);
        KeepFieldController.Instance.SetSaveData(keepFieldData);
        MainFieldController.Instance.Score = score;
        MFSlotController.SetDropCounter(dropCounter);
        DustbinFieldController.Instance.NextDustbinEnableCount = dustBinCount;
        StockFieldController.Instance.BirthCounter = birthCounter;

        GameSceneManager.Instance.InvalidateBestScoreText();

        //Debug.Log("Read Main Field" + mainFieldData);
        //Debug.Log("Read Stock Field" + stockFieldData);
        //Debug.Log("Read Keep Data " + keepFieldData);
        //Debug.Log("Read Curr Score" + score);
        //Debug.Log("Read Curr Drop Count" + dropCounter);
        //Debug.Log("Read Next Dust Bin" + dustBinCount);
        //Debug.Log("Read Birth Counter" + birthCounter);
    }

    //ドロップした後、にRequestが呼ばれる
    void updateResumeData()
    {
        /* Debug.Log( "updateResumeData" ); */
        mainFieldData = MainFieldController.Instance.GetSaveData();
        stockFieldData = StockFieldController.Instance.GetSaveData();
        keepFieldData = KeepFieldController.Instance.GetSaveData();
        score = MainFieldController.Instance.Score;
        dropCounter = MFSlotController.GetDropCounter();
        dustBinCount = DustbinFieldController.Instance.NextDustbinEnableCount;
        birthCounter = StockFieldController.Instance.BirthCounter;
    }

    [ContextMenu("write")]
    public void write(int step_index)
    {
        step_index = getStepIndexForSave(step_index);
        //Debug.Log( "Write save data : " + step_index );
        PlayerPrefs.SetString(MAIN_FIELD_NUMBER_KEY + step_index, mainFieldData);
        PlayerPrefs.SetString(STOCK_FIELD_NUMBER_KEY + step_index, stockFieldData);
        PlayerPrefs.SetInt(KEEP_FIELD_NUMBER_KEY + step_index, keepFieldData);
        PlayerPrefs.SetInt(CURR_SCORE_KEY + step_index, score);
        PlayerPrefs.SetInt(CURR_DROP_COUNT_KEY + step_index, dropCounter);
        PlayerPrefs.SetInt(NEXT_DUSTBIN_ENABLE_COUNT_KEY + step_index, dustBinCount);
        PlayerPrefs.SetInt(BIRTH_COUNTER_KEY + step_index, birthCounter);
        /* DebugConsole.Instance.Write( "w mfd" + step_index + ":" + mainFieldData ); */
        /* PlayerPrefs.SetInt(    RANDOM_SEED                   + step_index, StockFieldController.Instance.BirthRandomSeed ); */

        /* Debug.Log("Write Main Field" + MainFieldController.Instance.GetSaveData()); */
        /* Debug.Log("Write Stock Field" + StockFieldController.Instance.GetSaveData()); */
        /* Debug.Log("Write Keep Data " + KeepFieldController.Instance.GetSaveData()); */
        /* Debug.Log("Write Curr Score" + MainFieldController.Instance.Score); */
        /* Debug.Log("Write Curr Drop Count" + MFSlotController.GetDropCounter()); */
        /* Debug.Log("Write Next Dust Bin" + DustbinFieldController.Instance.NextDustbinEnableCount); */
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        /* DebugConsole.Instance.Write( "ResumeManager.Clear" ); */
        clearResumeData();
        for (int i = 0; i < cMaxBackSteps; ++i)
        {
            write(i);
        }
    }

    private void clearResumeData()
    {
        mainFieldData = MainFieldController.Instance.GetClearSaveDataStr();
        stockFieldData = StockFieldController.Instance.GetClearSaveDataStr();
        keepFieldData = 0;
        score = 0;
        dropCounter = 0;
        dustBinCount = DustbinFieldController.cFirstNextDustbinEnableCount;
        birthCounter = 0;
        currStepIndex = 0;
    }
}