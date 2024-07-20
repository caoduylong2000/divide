using UnityEngine;
using System.Collections;

public class FrameController : MonoBehaviour {
    private float scale = 1.2f;

	void Awake () {
        scale = 1.3f;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float diff = ( 1.0f - scale );
        if( Mathf.Abs( diff ) > 0.05f )
        {
            float s = diff * Time.deltaTime * 10.0f;
            scale += s;
            if( Mathf.Abs( diff ) < 0.01f )
            {
                scale = 1.0f;
            }
            transform.localScale = new Vector3( scale, scale, scale );
        }
	}
}
