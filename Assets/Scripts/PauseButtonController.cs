using UnityEngine;
using UnityEngine.Assertions;
using WK.Audio;

public class PauseButtonController : MonoBehaviour {
    private GameSceneManager gameSceneManager;

	// Use this for initialization
	void Start () {
        gameSceneManager = GameObject.Find( "GameSceneManager" ).GetComponent< GameSceneManager >();
	}

	// Update is called once per frame
	void Update () {

	}

	public void GoToMenu() {
        SoundManager.Instance.PlaySe( "click" );
        gameSceneManager.GoToMenu();
	}
}
