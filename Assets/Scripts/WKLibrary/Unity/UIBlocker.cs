using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace WK { namespace Unity {

public class UIBlocker : MonoBehaviour {
    public RectTransform target = null;

    private RectTransform rectTransform;
    private Image image;

	public void SetTarget( RectTransform rt ) {
        target = rt;
        if( rt != null ) { transform.SetParent( rt.parent ); }
    }

    public bool EnableBlock {
        get { return image.raycastTarget; }
        set { image.raycastTarget = value; }
    }

	// Use this for initialization
	void Awake () {
        rectTransform = GetComponent< RectTransform >();
        image = GetComponent< Image >();
        Debug.Assert( rectTransform != null );
        Debug.Assert( image != null );
	}
	
	// Update is called once per frame
	void Update () {
        if( target != null )
        {
            rectTransform.rect.Set( target.rect.x, target.rect.y, target.rect.width, target.rect.height );
            rectTransform.sizeDelta = target.sizeDelta;
            rectTransform.localScale = target.localScale;
            rectTransform.localRotation = target.localRotation;
            rectTransform.localPosition = target.localPosition;
        }
	}
}

}}
