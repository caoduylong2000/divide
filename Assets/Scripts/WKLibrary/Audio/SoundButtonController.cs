using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace WK { namespace Audio {

public class SoundButtonController : MonoBehaviour {
    public GameObject onImage;
    public GameObject offImage;
    private Button button;

	void Awake () {
        button = GetComponent<Button>();
    }

	void Start () {
        setMute( SoundManager.Instance.Volume.mute );
    }

	public void ClickCallback () {
        setMute( !SoundManager.Instance.Volume.mute );
        SoundManager.Instance.Save();
	}

	private void setMute ( bool mute ) {
        onImage.SetActive( !mute );
        offImage.SetActive( mute );
        button.targetGraphic = mute ? offImage.GetComponent<Image>() : onImage.GetComponent<Image>();
        SoundManager.Instance.Volume.mute = mute;
    }
}

}}
