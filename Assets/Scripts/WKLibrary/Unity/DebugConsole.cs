using UnityEngine;
using System.Collections.Generic;
using System.Text;
using WK.Utils;

namespace WK { namespace Unity {

[ExecuteInEditMode ()]
public class DebugConsole : SingletonMonoBehaviour< DebugConsole >
{
    public bool show                    = true;
    public bool showInEditor            = false;
    public int x = 5;
    public int y = 5;
    public int width = 200;
    public int height = 100;

    private Dictionary<string,int> dict = new Dictionary< string, int >();
    public const int cLifeTime          = 180;

    private StringBuilder text = new StringBuilder ();

	override protected void Awake () {
        base.Awake();
	}
    
    /* public static void AutoResize(int screenWidth, int screenHeight) */
    /* { */
    /*     Vector2 resizeRatio = new Vector2((float)Screen.width / screenWidth, (float)Screen.height / screenHeight); */
    /*     GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(resizeRatio.x, resizeRatio.y, 1.0f)); */
    /* } */
	void Update () {
        if (!UnityEngine.Debug.isDebugBuild) return;

		if (!show || (!Application.isPlaying && !showInEditor)) {
			return;
		}

        /* text.Clear();//There is no Clear method on .Net Framework2.0 */
        text.Length = 0;//This substitutes for Clear()
        text.Capacity = 0;//This substitutes for Clear()

        //life管理処理は1フレームに1回しか呼びたくないのでUpdateで行う
        var removals = new List<string>();
        List<string> keys = new List<string>(dict.Keys);
        foreach( var key in keys )
        {
            dict[key] -= 1;
            if( dict[key] < 0 )
            {
                removals.Add( key );
            }
            else
            {
                text.Append ( key );
                text.Append ( "\n" );
            }
        }

        foreach( var key in removals )
        {
            dict.Remove( key );
        }
    }

    //こやつは1フレームに複数回呼ばれる
	public void OnGUI () {
        if (!UnityEngine.Debug.isDebugBuild) return;

		if (!show || (!Application.isPlaying && !showInEditor)) {
			return;
		}

        /* AutoResize(2048, 2732); */
        /* AutoResize(1024, 1366); */
        /* AutoResize(320, 640); */

        const int cMergin = 5;
		GUI.Box (new Rect (x,y,width,height),"");
		GUI.Label (new Rect (x+cMergin,y+cMergin,width - cMergin*2,height - cMergin*2),text.ToString ());
    }

	public void Write( string str )
	{
        if (!UnityEngine.Debug.isDebugBuild) return;
        dict[ str ] = cLifeTime;
	}

	public void Write( string str, int life_time )
	{
        if (!UnityEngine.Debug.isDebugBuild) return;
        dict[ str ] = life_time;
	}

	public void Dump()
	{
	}
}

}
}
