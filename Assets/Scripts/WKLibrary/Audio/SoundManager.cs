using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WK.Collections;
using WK.Unity;

namespace WK { namespace Audio {

[System.Serializable]
public class SoundVolume{
    public float bgm   = 1.0f;
    public float se    = 1.0f;

    public bool  mute  = false;

    public void Reset(){
        bgm  = 1.0f;
        se   = 1.0f;
        mute = false;
    }
}

public class SoundManager : SingletonMonoBehaviour< SoundManager > {
    [SerializeField]
    private SoundVolume volume = new SoundVolume();
    public SoundVolume Volume { 
        get { return volume; }
        set { volume = value; } 
    }

	private AudioClip[] seClips;
	private AudioClip[] bgmClips;

	private Dictionary<string,int> seIndexes = new Dictionary<string,int>();
	private Dictionary<string,int> bgmIndexes = new Dictionary<string,int>();

    const int cNumChannel = 16;
    private AudioSource bgmSource;
    private AudioSource[] seSources = new AudioSource[cNumChannel];

	Queue<int> seRequestQueue = new Queue< int >();

    private const string cSaveMuteKey = "SoundManagerDataMuteKey";
    private const string cSaveSeVolumeKey = "SoundManagerDataSeVolumeKey";
    private const string cSaveBgmVolumeKey = "SoundManagerDataBgmVolumeKey";

	//------------------------------------------------------------------------------
    public void Save()
    {
        PlayerPrefs.SetInt( cSaveMuteKey, volume.mute ? 1 : 0 );
        PlayerPrefs.SetFloat( cSaveSeVolumeKey, volume.se );
        PlayerPrefs.SetFloat( cSaveBgmVolumeKey, volume.bgm );
        PlayerPrefs.Save();
    }

	//------------------------------------------------------------------------------
    public void Read()
    {
        volume.mute = PlayerPrefs.GetInt( cSaveMuteKey, 0 ) == 1;
        volume.se = PlayerPrefs.GetFloat( cSaveSeVolumeKey, 1.0f );
        volume.bgm = PlayerPrefs.GetFloat( cSaveBgmVolumeKey, 1.0f );
    }

	//------------------------------------------------------------------------------
	protected override void Awake () {
        base.Awake();

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;

        for(int i = 0 ; i < seSources.Length ; i++ ){
            seSources[i] = gameObject.AddComponent<AudioSource>();
        }

        seClips  = Resources.LoadAll<AudioClip>("Audio/SE");
        bgmClips = Resources.LoadAll<AudioClip>("Audio/BGM");

        for( int i = 0; i < seClips.Length; ++i )
        {
            seIndexes[seClips[i].name] = i;
        }

        for( int i = 0; i < bgmClips.Length; ++i )
        {
            bgmIndexes[bgmClips[i].name] = i;
        }

        Read();

        /* Debug.Log("se ========================"); */
        /* foreach(var ac in seClips ) { Debug.Log( ac.name ); } */
        /* Debug.Log("bgm ========================"); */
        /* foreach(var ac in bgmClips ) { Debug.Log( ac.name ); } */
	}

	//------------------------------------------------------------------------------
	void Update()
	{
        bgmSource.mute = volume.mute;
        foreach(var source in seSources ){
            source.mute = volume.mute;
        }

        bgmSource.volume = volume.bgm;
        foreach(var source in seSources ){
            source.volume = volume.se;
        }

		int count = seRequestQueue.Count;
		if( count != 0 )
		{
			int sound_index = seRequestQueue.Dequeue();
            playSeImpl( sound_index );
		}
	}

	//------------------------------------------------------------------------------
    private void playSeImpl( int index )
    {
        if( 0 > index || seClips.Length <= index ){
            return;
        }

        foreach(AudioSource source in seSources){
            if( false == source.isPlaying ){
                source.clip = seClips[index];
                source.Play();
                return;
            }
        }  
    }

	//------------------------------------------------------------------------------
	public int GetSeIndex( string name )
	{
        return seIndexes[name];
	}

	//------------------------------------------------------------------------------
	public int GetBgmIndex( string name )
	{
        return bgmIndexes[name];
	}

    //------------------------------------------------------------------------------
    public void PlayBgm( string name ){
        int index = bgmIndexes[name];
        PlayBgm( index );
    }

    //------------------------------------------------------------------------------
    public void PlayBgm( int index ){
        if( 0 > index || bgmClips.Length <= index ){
            return;
        }

        if( bgmSource.clip == bgmClips[index] ){
            return;
        }

        bgmSource.Stop();
        bgmSource.clip = bgmClips[index];
        bgmSource.Play();
    }

    //------------------------------------------------------------------------------
    public void StopBgm(){
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    //------------------------------------------------------------------------------
    public bool IsBgmPlaying{ get{ return bgmSource.isPlaying; } }

	//------------------------------------------------------------------------------
	public void PlaySe( string name )
    {
        PlaySe( GetSeIndex( name ) );
    }
   
    //一旦queueに溜め込んで重複を回避しているので
    //再生が1frame遅れる時がある
	//------------------------------------------------------------------------------
	public void PlaySe( int index )
    {
        if( !seRequestQueue.Contains( index ) )
        {
        	seRequestQueue.Enqueue( index );
        }
        //@todo
        //ここでhandleを返して
        //それに後からaudioSourceとかを入れといて、
        //再生が終わったかどうかを見られるようにしたい
	}

    //------------------------------------------------------------------------------
    public void StopSe(){
        ClearAllSeRequest();
        foreach(AudioSource source in seSources){
            source.Stop();
            source.clip = null;
        }  
    }

	//------------------------------------------------------------------------------
	public void ClearAllSeRequest()
	{
        seRequestQueue.Clear();
	}
}

}}
