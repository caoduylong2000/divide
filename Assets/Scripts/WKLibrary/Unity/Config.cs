using UnityEngine;
using System.IO;
using WK.Utils;

namespace WK { namespace Unity {

public class Config : Singleton< Config >
{
    public static string androidAppId;

    public static string iOSAppId;

	private static SystemLanguage _language = Application.systemLanguage;

    public static SystemLanguage Language { get { return _language; } }

	public Config () {
	}

    // if you want to change language setting, please use this
	public static void ChangeLanguage ( SystemLanguage lang ) {
        _language = lang;
    }

    public static bool IsJP()
    {
        return _language == SystemLanguage.Japanese;
    }

    public static bool IsUseCircleCorrect()
    {
        return ( _language == SystemLanguage.Japanese );
    }
}

}}
