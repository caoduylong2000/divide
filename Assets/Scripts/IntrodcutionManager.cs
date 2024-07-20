using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using WK.Unity;

public class IntrodcutionManager : SingletonMonoBehaviour<IntrodcutionManager> {
    public GameObject[] introObjs;
    public GameObject[] explainTexts;

	protected SimpleSceneObjectController introScene    = null;
    protected MainFieldController         mainFieldCtrl = null;
    protected KeepFieldController         keepFieldCtrl = null;
    protected Button                      pauseButton   = null;

    protected int state        = 0;
    protected int nextState    = 0;
    protected int stateCounter = 0;
    protected float stateTimer = 0;

    protected const int cStateIdle                   = 0;
    protected const int cStateDrop1                  = 1;
    protected const int cStateDrop2                  = 2;
    protected const int cStateDivideWait1            = 3;
    protected const int cStateDrop3                  = 4;
    protected const int cStateDrop4                  = 5;
    protected const int cStateDivideWait2            = 6;
    protected const int cStateDivideWait3            = 7;
    protected const int cStateExplanationKeep        = 8;
    protected const int cStateExplanationEnd         = 9;
    protected const int cStateFinish                 = 10;
    protected const int cStateExplanationComplement0 = 11;
    protected const int cStateExplanationComplement1 = 12;
    protected const int cStateExplanationComplement2 = 13;

    protected const int explainTextsComplement0 = 5;
    protected const int explainTextsComplement1 = 6;
    protected const int explainTextsComplement2 = 7;

    //------------------------------------------------------------------------------
    public void StartIntroduction()
    {
        Debug.Log("StartIntroduction");
        changeState( cStateDrop1 );
    }

    //------------------------------------------------------------------------------
    public bool IsFinished()
    {
        return state == cStateFinish;
    }

    //------------------------------------------------------------------------------
	protected override void Awake () {
        base.Awake();

        stateCounter = 0;
        stateTimer   = 0.0f;
        state        = cStateIdle;
        nextState    = cStateIdle;
        
        introScene = GetComponent<SimpleSceneObjectController>();
    }

    //------------------------------------------------------------------------------
    void Start()
    {
        introScene.Init( false, false );
        foreach (var v in introObjs) {
            v.SetActive( false );
        }

        foreach (var v in explainTexts) {
            v.SetActive( false );
        }

        mainFieldCtrl = GameObject.Find( "MainField"       ).GetComponent< MainFieldController >();
        keepFieldCtrl = GameObject.Find( "KeepField"       ).GetComponent< KeepFieldController >();
        pauseButton   = GameObject.Find( "PauseButtonRoot" ).GetComponent< Button              >();
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
            case cStateIdle:
                break;
            case cStateDrop1:
                if( stateCounter == 0 )
                {
                    introObjs[0].SetActive( true );
                    introScene.Init( true, true );
                    mainFieldCtrl.SetDroppable( false ); 
                    mainFieldCtrl.SetDroppableXY( true, 1, 1 ); 
                    keepFieldCtrl.RequestSlotDragable( false );
                    pauseButton.interactable = false;
                }

                if( MFSlotController.GetDropCounter() == 1 )
                {
                    /* introObjs[0].SetActive( false ); */
                    /* introObjs[1].SetActive( true ); */
                    changeState(cStateExplanationComplement0);
                }
                break;
            case cStateDrop2:
                if( stateCounter == 0 )
                {
                    introObjs[0].SetActive( false );
                    introObjs[1].SetActive( true );
                    mainFieldCtrl.SetDroppable( false ); 
                    mainFieldCtrl.SetDroppableXY( true, 1, 2 ); 
                }

                if( MFSlotController.GetDropCounter() == 2 )
                {
                    explainTexts[explainTextsComplement0].SetActive( false );
                    changeState(cStateDivideWait1);
                }
                break;
            case cStateDivideWait1:
                {
                    if( stateCounter == 0 )
                    {
                        mainFieldCtrl.SetBlocksRaycast( false ); 
                        explainTexts[0].SetActive( true );
                        introScene.FadeOut();
                    }

                    const float cWaitTime = 3.0f;
                    if( stateTimer > cWaitTime )
                    {
                        explainTexts[0].SetActive( false );
                        changeState(cStateExplanationComplement1);
                    }
                }
                break;
            case cStateDrop3:
                if( stateCounter == 0 )
                {
                    introScene.FadeIn();
                    introObjs[1].SetActive( false );
                    introObjs[2].SetActive( true );
                    mainFieldCtrl.SetBlocksRaycast( true ); 
                    mainFieldCtrl.SetDroppable( false ); 
                    mainFieldCtrl.SetDroppableXY( true, 1, 2 ); 
                }

                if( MFSlotController.GetDropCounter() == 3 )
                {
                    explainTexts[explainTextsComplement1].SetActive( false );
                    changeState(cStateExplanationComplement2);
                }
                break;
            case cStateDrop4:
                if( stateCounter == 0 )
                {
                    introScene.FadeIn();
                    introObjs[2].SetActive( false );
                    introObjs[3].SetActive( true );
                    mainFieldCtrl.SetDroppable( false ); 
                    mainFieldCtrl.SetDroppableXY( true, 1, 0 ); 
                }

                if( MFSlotController.GetDropCounter() == 4 )
                {
                    explainTexts[explainTextsComplement2].SetActive( false );
                    changeState(cStateDivideWait2);
                }
                break;
            case cStateDivideWait2:
                {
                    if( stateCounter == 0 )
                    {
                        mainFieldCtrl.SetBlocksRaycast( false ); 
                        explainTexts[0].SetActive( false );
                        explainTexts[1].SetActive( true );
                        introScene.FadeOut();
                    }

                    const float cWaitTime = 0.8f;
                    if( stateTimer > cWaitTime )
                    {
                        changeState(cStateDivideWait3);
                    }
                }
                break;
            case cStateDivideWait3:
                {
                    if( stateCounter == 0 )
                    {
                        explainTexts[2].SetActive( true );
                    }

                    const float cWaitTime = 3.2f;
                    if( stateTimer > cWaitTime )
                    {
                        changeState(cStateExplanationKeep);
                    }
                }
                break;
            case cStateExplanationKeep:
                {
                    if( stateCounter == 0 )
                    {
                        explainTexts[1].SetActive( false );
                        explainTexts[2].SetActive( false );
                        explainTexts[3].SetActive( true );
                        keepFieldCtrl.RequestSlotDragable( true );
                    }

                    const float cWaitTime = 3.2f;
                    if( stateTimer > cWaitTime )
                    {
                        changeState(cStateExplanationEnd);
                    }
                }
                break;
            case cStateExplanationEnd:
                {
                    if( stateCounter == 0 )
                    {
                        explainTexts[3].SetActive( false );
                        explainTexts[4].SetActive( true );
                    }

                    const float cWaitTime = 3.2f;
                    if( stateTimer > cWaitTime )
                    {
                        changeState(cStateFinish);
                    }
                }
                break;
            case cStateFinish:
                if( stateCounter == 0 )
                {
                    explainTexts[4].SetActive( false );
                    mainFieldCtrl.SetBlocksRaycast( true ); 
                    changeState( cStateIdle );
                    gameObject.SetActive( false );
                    pauseButton.interactable = true;
                    GameSceneManager.Instance.isTutorialFinished = true;
                }
                break;
                //後から追加した説明
            case cStateExplanationComplement0:
                {
                    if( stateCounter == 0 )
                    {
                        explainTexts[explainTextsComplement0].SetActive( true );
                        changeState(cStateDrop2);
                    }
                }
                break;
            case cStateExplanationComplement1:
                {
                    if( stateCounter == 0 )
                    {
                        explainTexts[explainTextsComplement1].SetActive( true );
                        changeState(cStateDrop3);
                    }
                }
                break;
            case cStateExplanationComplement2:
                {
                    if( stateCounter == 0 )
                    {
                        explainTexts[explainTextsComplement2].SetActive( true );
                        changeState(cStateDrop4);
                    }
                }
                break;
        }

        /* switch( MFSlotController.GetDropCounter() ) */
        /* { */
        /*     case 0: */
        /*         { */
        /*         } */
        /*         break; */
        /*     case 1: */
        /*         { */
        /*             introObjs[0].SetActive( false ); */
        /*             introObjs[1].SetActive( true ); */
        /*         } */
        /*         break; */
        /*     case 2: */
        /*         { */
        /*             introObjs[1].SetActive( false ); */
        /*             introObjs[2].SetActive( true ); */
        /*         } */
        /*         break; */
        /*     case 3: */
        /*         { */
        /*             introObjs[1].SetActive( false ); */
        /*             introObjs[2].SetActive( true ); */
        /*         } */
        /*         break; */
        /* } */
    }

    //------------------------------------------------------------------------------
    private void changeState( int next_state )
    {
        nextState = next_state;
    }

    //------------------------------------------------------------------------------
    private void updateIdle()
    {
    }
}
