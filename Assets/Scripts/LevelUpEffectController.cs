using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using WK.Audio;

public class LevelUpEffectController : SerializedMonoBehaviour {
    [SerializeField]
    private Text levelText = null;

	[AssetsOnly]
    [SerializeField]
    private GameObject levelUpParticle = null;

    [SerializeField]
    private GameObject levelUpEffect;

    [SerializeField]
    private Text levelUpEffectText;

    [SerializeField]
    private GameObject confetti;

    void Start()
    {
        levelUpEffect.GetComponent<WK.CurveAnimator>().OnComplete.AddListener( () => {
                    levelUpEffect.SetActive( false );
                    //@memo This place is good for inserting interstitial Ads.
                    if(!GameSceneManager.Instance.IsTimeLimit)
                    {
                        if(! ( GameSceneManager.Instance.IsPurchasedNoAds
                                || GameSceneManager.Instance.IsSubscribedNoAds ) )
                        {
                            WK.AdListenerMax.Instance.ShowInterstitial();
                            //for debug
                            //if(UnityEngine.Debug.isDebugBuild)
                            //{
                            //    AdListener.Instance.ShowInterstitial();
                            //}
                        }
                    }
                }
                );
        levelUpEffect.SetActive( false );
    }

	//------------------------------------------------------------------------------
    //[ContextMenu("StartSmallEffectTest")]
    [Button]
    public void StartSmallEffectTest()
    {
        StartSmallEffect( 2 );
    }

	//------------------------------------------------------------------------------
    public void StartSmallEffect( int level )
    {
        levelText.text = ( level + 1 ).ToString();
        GameObject obj = Instantiate( levelUpParticle );
        obj.transform.SetParent( levelText.gameObject.transform );
        obj.GetComponent<LevelUpParticleController >().Init( new Vector3( 100.0f, 0.0f, 0.0f ) );
        SoundManager.Instance.PlaySe( "levelup" );
    }

	//------------------------------------------------------------------------------
    //[ContextMenu("StartLargeEffectTest")]
    [Button]
    public void StartLargeEffectTest()
    {
        StartLargeEffect( 2 );
    }

	//------------------------------------------------------------------------------
    public void StartLargeEffect( int level )
    {
        levelUpEffect.SetActive(true);
        levelUpEffectText.text = "Level " + ( level + 1 ).ToString();
        StartCoroutine( this.DelayMethod( 0.2f, () => {
                        SoundManager.Instance.PlaySe( "levelup" );
                        levelText.text = ( level + 1 ).ToString();
                    } ) );

        confetti.SetActive( true );

        const int NUM_VIBRATION = 5;
        for( int i = 0; i < NUM_VIBRATION; ++i )
        {
            StartCoroutine( this.DelayMethod( 0.1f * i, () => VibrationManager.Instance.Vibrate() ) );
        }

        StartCoroutine( this.DelayMethod( 5.0f
                    , () => {
                        confetti.SetActive( false );
                        Debug.Log( "confetti, level up" );
                    }
                    )
        );
    }
}

