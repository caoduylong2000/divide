using UnityEngine;
using System.IO;
using WK.Utils;

namespace WK { namespace Unity {

public class DebugLog : Singleton< DebugLog >
{
	public static string fileName = "Temp/LogData.txt";
	//private StreamWriter streamWriter;

	public DebugLog () {
#if UNITY_EDITOR
		StreamWriter sw = new StreamWriter( fileName, false ); //上書き
		sw.WriteLine("");
		sw.Flush();
		sw.Close();
#endif
	}

	public static void Write( string str )
	{
#if UNITY_EDITOR
		StreamWriter streamWriter = new StreamWriter( fileName, true ); //追記
		streamWriter.WriteLine( str );
		streamWriter.Flush();
		streamWriter.Close();
#endif
	}

	public static void Dump()
	{
	}
}

}
}
