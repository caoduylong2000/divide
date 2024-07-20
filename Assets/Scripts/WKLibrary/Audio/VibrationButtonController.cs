using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace WK {

public class VibrationButtonController : MonoBehaviour {
    public GameObject onImage;
    public GameObject offImage;

	void Start () {
#if UNITY_ANDROID
        gameObject.SetActive( true );
#endif
        setVibration( VibrationManager.Instance.IsEnableVibration, false );
    }

	public void ClickCallback () {
        setVibration( !VibrationManager.Instance.IsEnableVibration, true );
	}

	private void setVibration( bool enable, bool is_vibrate ) {
        onImage.SetActive( enable );
        offImage.SetActive( !enable );
        VibrationManager.Instance.SetEnableVibration( enable );
        if( is_vibrate )
        {
            VibrationManager.Instance.Vibrate();
        }
    }
}

}
