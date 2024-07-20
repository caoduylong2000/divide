using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class LevelUpParticleController : MonoBehaviour {
    private Text text;
    private Color textColor;
    private Transform cachedTransform;
    public float speed = 1.0f;
    public float alphaSpeed = 2.0f;
    private float targetOffset = 100.0f;
    private Vector3 targetPos;

    protected int state        = 0;
    protected int nextState    = 0;
    protected int stateCounter = 0;
    protected float stateTimer = 0;

    protected const int cStateSlideIn = 0;
    protected const int cStateFadeOut = 1;

    //------------------------------------------------------------------------------
    void Awake()
    {
        stateCounter = 0;
        stateTimer   = 0.0f;
        state        = cStateSlideIn;
        nextState    = cStateSlideIn;

        cachedTransform = transform;
        text = GetComponent<Text>();
    }

    //------------------------------------------------------------------------------
    public void Init( Vector3 pos )
    {
        cachedTransform.localPosition = pos;
        cachedTransform.localScale = Vector3.one;
        targetPos = pos;
        targetPos.y += targetOffset;

        textColor = text.color;
        textColor.a = 0.0f;
        text.color = textColor;
    }

    //------------------------------------------------------------------------------
    void Update()
    {
        ++stateCounter; 
        stateTimer += Time.deltaTime;
        if( state != nextState )
        {
            state        = nextState;
            stateCounter = 0;
            stateTimer   = 0.0f;
        }

        switch( state )
        {
            case cStateSlideIn:
                updateSlideIn();
                break;
            case cStateFadeOut:
                updateFadeOut();
                break;
        }
    }

    //------------------------------------------------------------------------------
    private void changeState( int next_state )
    {
        nextState = next_state;
    }

    //------------------------------------------------------------------------------
    private void updateSlideIn()
    {
        bool result;
        textColor.a =
            WK.Math.Mathf.Approach( out result
                    , 1.0f
                    , textColor.a 
                    , alphaSpeed * Time.deltaTime 
                    , 0.01f
                    );
        text.color = textColor;

        cachedTransform.localPosition =
            WK.Math.Mathf.Approach( out result
                    , targetPos
                    , cachedTransform.localPosition
                    , speed * Time.deltaTime 
                    , 0.1f
                    );

        if( result )
        {
            changeState( cStateFadeOut );
        }
    }

    //------------------------------------------------------------------------------
    private void updateFadeOut()
    {
        float cWatiTime = 2.0f;
        if( stateTimer > cWatiTime )
        {
            bool result;
            textColor.a =
                WK.Math.Mathf.Approach( out result
                        , 0.0f
                        , textColor.a 
                        , alphaSpeed * Time.deltaTime 
                        , 0.01f
                        );
            text.color = textColor;

            if( result )
            {
                Destroy( gameObject );
            }
        }
    }
}
