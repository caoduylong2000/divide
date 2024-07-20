using UnityEngine;
using UnityEngine.Assertions;
using WK.Audio;

public class SkinButtonController : MonoBehaviour
{
    private GameSceneManager gameSceneManager;
    private GameObject gp;

    // Use this for initialization
    void Start()
    {
        gameSceneManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GoToSkinMenu()
    {
        SoundManager.Instance.PlaySe("click");
        gameSceneManager.GoToSkinMenu();
    }
}
