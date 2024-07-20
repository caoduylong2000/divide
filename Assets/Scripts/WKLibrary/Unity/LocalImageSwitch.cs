using UnityEngine;
using System.Collections;
using WK.Unity;

public class LocalImageSwitch : MonoBehaviour {
    public GameObject imageJP;
    public GameObject imageGlb;

	// Use this for initialization
	void Start () {
        imageJP.SetActive( false );
        imageGlb.SetActive( false );
        if( Config.Language == SystemLanguage.Japanese )
        {
            imageJP.SetActive( true );
        }
        else
        {
            imageGlb.SetActive( true );
        }
	}
}
