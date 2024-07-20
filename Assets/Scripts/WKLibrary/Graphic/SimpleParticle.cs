using UnityEngine;
using UnityEngine.UI;

namespace WK { namespace Graphic {

public class SimpleParticle : MonoBehaviour {
    public  float       lifeTime = 3.0f;
    public  Vector3     vel;
    public  float       damping  = 1.0f;
    public  float       velAlpha;
    private CanvasGroup canvasGroup;
    private Vector3     pos;
    private Transform   cachedTransform;

	void Awake () {
        pos = new Vector3( 0.0f, 0.0f, 0.0f );
        cachedTransform = gameObject.transform;
        canvasGroup = GetComponent<CanvasGroup>();
        if( canvasGroup == null )
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
	}

	public void SetPos ( Vector3 p ) {
        pos = p;
    }

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
            cachedTransform.localPosition = pos;
            canvasGroup.alpha += velAlpha * Time.deltaTime;
        }
	}
}

}}
