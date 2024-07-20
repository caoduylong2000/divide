using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using WK;

namespace WK {
namespace Unity {

[RequireComponent(typeof(Text))]
public class VersionObj : SingletonMonoBehaviour<VersionObj> {
    Text text;
    Color color;
    private string currentVersion;
    private string previousVersion;
    private const string cVersionSaveKey = "VERSION_SAVE_KEY";

    public string CurrentVersion  { get { return currentVersion;  } }
    public string PreviousVersion { get { return previousVersion; } }

	protected override void Awake () {
        base.Awake();

        currentVersion = CurrentBundleVersion.version;
        text = GetComponent<Text>();
        text.text = "v." + currentVersion;
        color = text.color;
        StartCoroutine( fadeOut() );
        previousVersion = PlayerPrefs.GetString( cVersionSaveKey, "" );
        if( previousVersion != currentVersion )
        {
            Debug.Log( "Save Version Info : " + currentVersion );
            PlayerPrefs.SetString( cVersionSaveKey, currentVersion );
            PlayerPrefs.Save();
        }
	}
	
	IEnumerator fadeOut () {
        int waitCounter = 300;
        while( waitCounter > 0 )
        {
            waitCounter--;
            yield return null;
        }

        while( color.a > 0.0f )
        {
            color.a = UnityEngine.Mathf.Max( color.a - 0.02f, 0.0f );
            text.color = color;
            yield return null;
        }
        gameObject.SetActive( false );
	}
}

}
}
