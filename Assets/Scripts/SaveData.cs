using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

//@memo
//当初PlayerPrefsを使っていたが
//データが大きくなってきたので
//こちらをサブとして利用する
public class SaveData : WK.Unity.SingletonScriptableObject<SaveData>
{
    [SerializeField]
    protected bool[] skinOpened;

    public SaveMainData data;
    public static SaveMainData Data { get { return Instance.data; } }

    static public readonly string FILE_PATH = "tonton";
    static public readonly string MAIN_DATA_KEY = "main";

    public enum ELoadResult {
          success
        , failedToFind
        , failedToLoad
        , undefined
    };

    //現在新しい方を示す
    private static Dictionary<string,int> panDict = new Dictionary<string, int>();

    static public void SaveMain()
    {
        Instance.WriteToMainData();

        Data.saveDateTimeString = DateTime.UtcNow.ToString();
        Debug.Log("SaveMain.....");

        var file_path = FILE_PATH;

        if(!panDict.ContainsKey(FILE_PATH))
        {
            panDict[FILE_PATH] = 1;
        }
        file_path += 1 - panDict[FILE_PATH];

        ES3.Save<SaveMainData>( MAIN_DATA_KEY, Data, file_path );

        panDict[FILE_PATH] = 1 - panDict[FILE_PATH];
    }

    //@memo 本当はさらに起動時にバックアップをとるような仕組みにしたい
    static public void LoadMain()
    {
        Instance.Initialize();

        var result = load( false );
        if( result != ELoadResult.success )
        {
            result = load( true );//一つ前のものを読み込む
            if( result != ELoadResult.success )
            {
                //@todo
                //エラーメッセージ
                //バックアップからの復活にも失敗しました。
            }
            else
            {
                //@todo
                //エラーメッセージ
                //一つ前のデータを読み込みました。
            }
        }

        Instance.ReadFromMainData();
    }

    static private ELoadResult load( bool is_read_old )
    {
        string file_path = FILE_PATH;
        var file_path0 = FILE_PATH + "0";
        var file_path1 = FILE_PATH + "1";

        if (!ES3.FileExists(file_path0) && !ES3.FileExists(file_path1))//どちらも存在しない
        {
            return ELoadResult.failedToFind;
            //return null;
        }
        else if (ES3.FileExists(file_path0) && ES3.FileExists(file_path1))//どちらも存在する
        {
            if( !is_read_old )
            {
                //どちらも存在したら新しい方
                if( ES3.GetTimestamp(file_path0) >= ES3.GetTimestamp(file_path1) )
                {
                    file_path = file_path0;
                    panDict[FILE_PATH] = 0;
                }
                else
                {
                    file_path = file_path1;
                    panDict[FILE_PATH] = 1;
                }
            }
            else
            {
                //どちらも存在したら古い方
                if( ES3.GetTimestamp(file_path0) >= ES3.GetTimestamp(file_path1) )
                {
                    file_path = file_path1;
                    panDict[FILE_PATH] = 1;
                }
                else
                {
                    file_path = file_path0;
                    panDict[FILE_PATH] = 0;
                }
            }
        }
        else if(ES3.FileExists(file_path0))//0だけ存在する
        {
            file_path = file_path0;
            panDict[FILE_PATH] = 0;
        }
        else if(!ES3.FileExists(file_path1))//1だけ存在する
        {
            file_path = file_path1;
            panDict[FILE_PATH] = 1;
        }
        else
        {
            return ELoadResult.undefined;
        }

        try
        {
            Instance.data = ES3.Load<SaveMainData>( MAIN_DATA_KEY, file_path );
        }
        catch( Exception e )
        {
            Debug.Log( e.Message );
            return ELoadResult.failedToLoad;
        }

        return ELoadResult.success;
    }

    //------------------------------------------------------------------------------
    public bool GetSkinOpened( int skin_index )
    {
        return skinOpened[skin_index];
    }

    public bool SetSkinOpened( int skin_index, bool opened )
    {
        return skinOpened[skin_index] = opened;
    }

    public int GetSkinOpenedNum()
    {
        return skinOpened.Count( x => x == true );
    }

    //------------------------------------------------------------------------------
    public void Initialize()
    {
        data = new SaveMainData();

        skinOpened = new bool[Parameters.MaxSkin];
    }

    public void ReadFromMainData()
    {
        //update skin opened
        var str_length = data.skinOpenedStr.Length;
        for( int k = 0; k < Parameters.MaxSkin; ++k )
        {
            bool is_opened = false;
            if( k < str_length )
            {
                is_opened = data.skinOpenedStr[k] == '1';
            }
            skinOpened[k] = is_opened;
        }
    }

    public void WriteToMainData()
    {
        StringBuilder sb = new StringBuilder("");
        //string str = new string( '0', Parameters.MAX_LEVEL * Parameters.MAX_STAGE );
        data.skinOpenedStr = "";
        for( int k = 0; k < Parameters.MaxSkin; ++k )
        {
            bool is_opened = skinOpened[k];
            sb.Append( is_opened ? '1' : '0' );
        }
        data.skinOpenedStr = sb.ToString();
    }
}

[Serializable]
public class SaveMainData
{
    public string guid;
    public string saveDateTimeString;
    public bool   hasConfirmedTermsOfService;
    public int    skinIndex;
    public float  playTime;
    public string skinOpenedStr;//stringの方がboolより小さくて済むし長さの変更にも強い
    public int    purchasedSubscriptionCount;//サブスク購入回数(ExpireDateで何かミスったとき、これがある人だけ補償する用)

    public SaveMainData()
    {
        guid = System.Guid.NewGuid().ToString();
        saveDateTimeString = "";
        hasConfirmedTermsOfService = false;
        skinIndex = 1;
        playTime = 0.0f;
        skinOpenedStr = "11";
        purchasedSubscriptionCount = 0;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(SaveData))]
public class MeshModifierEditor : Editor
{
    public override void OnInspectorGUI()
    {

        if (target.name == "SaveData" && GUILayout.Button("Delete Files"))
        {
            var saveData = (SaveData)target;
            //@todo
            ES3.DeleteFile( SaveData.FILE_PATH + "0" );
            ES3.DeleteFile( SaveData.FILE_PATH + "1" );
        }

        if (target.name == "SaveData" && GUILayout.Button("Save"))
        {
            var saveData = (SaveData)target;
            SaveData.SaveMain();
        }
        base.DrawDefaultInspector();
    }
}
#endif


