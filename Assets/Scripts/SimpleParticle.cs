using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleParticle : MonoBehaviour {
    public  float   lifeTime = 3.0f;
    private Vector3 pos;
    public  Vector3 vel;
    public  float   damping = 1.0f;
    public  float   velAlpha;
    private CanvasGroup   canvasGroup;
    private Image         image;

	void Awake () {
        pos = new Vector3( 0.0f, 0.0f, 0.0f );
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponentInChildren<Image>();
        if( canvasGroup == null )
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
	}

	public void SetPos ( Vector3 p ) {
        pos = p;
    }

	public void SetSprite ( Sprite s ) {
        if( image != null )
        {
            image.sprite = s;
        }
    }

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
        lifeTime -= Time.deltaTime;
        if( lifeTime < 0.0f )
        {
            Destroy( gameObject );
        }
        else
        {
            vel *= damping;
            pos += vel * Time.deltaTime;
            gameObject.transform.localPosition = pos;
            canvasGroup.alpha += velAlpha * Time.deltaTime;
        }
	}
}
