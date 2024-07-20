using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class ComboParticleController : MonoBehaviour {
    private Text text;
    private Color textColor;
    private Transform cachedTransform;
    public float alphaSpeed = 3.0f;

    protected Vector3 scale       = Vector3.zero;
    protected Vector3 scaleVel    = Vector3.zero;
    protected float   scaleSpring = 300.0f;
    protected float   scaleDump   = 0.90f;

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
    public void Init( Vector3 pos, int num )
    {
        string str = num.ToString() + " Combo!";

        cachedTransform.localPosition = pos;
        cachedTransform.localScale = Vector3.one;
        text.text = str;

        text.fontSize = ComboEffectManager.Instance.GetComboEffectFontSize( num );
        text.color = ComboEffectManager.Instance.GetComboEffectFontColor( num );

        textColor = text.color;
        textColor.a = 0.0f;
        text.color = textColor;

        scale = Vector3.one * 0.9f;
        scaleVel = Vector3.zero;
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
        float deltaTime = Mathf.Min( Time.deltaTime, 0.1f );
        bool result;
        textColor.a =
            WK.Math.Mathf.Approach( out result
                    , 1.0f
                    , textColor.a 
                    , alphaSpeed * deltaTime 
                    , 0.01f
                    );
        text.color = textColor;

        Vector3 rel = Vector3.one - scale;
        scaleVel += rel * scaleSpring * Time.deltaTime;
        scale += scaleVel * Time.deltaTime;
        scaleVel *= scaleDump;
        cachedTransform.localScale = scale;

        if( result )
        {
            changeState( cStateFadeOut );
        }
    }

    //------------------------------------------------------------------------------
    private void updateFadeOut()
    {
        float cWatiTime = 0.3f;
        if( stateTimer > cWatiTime )
        {
            float deltaTime = Mathf.Min( Time.deltaTime, 0.1f );
            bool result;
            textColor.a =
                WK.Math.Mathf.Approach( out result
                        , 0.0f
                        , textColor.a 
                        , alphaSpeed * deltaTime 
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
