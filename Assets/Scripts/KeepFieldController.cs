using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using WK.Unity;

public class KeepFieldController : SingletonMonoBehaviour<KeepFieldController> {
    [SerializeField]
    protected SlotController slot_;

    public SlotController slot { get { return slot_; } }
    private CanvasGroup canvasGroup;

	//------------------------------------------------------------------------------
	public virtual void ResetParams () {
        GameObject block = slot.block;
        if( block != null )
        {
            Destroy( block );
        }
	}

    //------------------------------------------------------------------------------
	protected override void Awake () {
        base.Awake();

        canvasGroup = GetComponent<CanvasGroup>();
    }

	//------------------------------------------------------------------------------
	// Use this for initialization
	void Start () {
        //slot = GetComponentInChildren<SlotController>();
	}

	//------------------------------------------------------------------------------
	// Update is called once per frame
	void Update () {
	}

	//------------------------------------------------------------------------------
	public void RequestSlotDragable( bool enable ) {
        canvasGroup.blocksRaycasts = enable;//event handler系はinteractableでは防げないみたい
	}

	//------------------------------------------------------------------------------
	public int GetSaveData() {
        GameObject block = slot.block;
        if( block )
        {
            return block.GetComponent<BlockController>().number;
        }
        return 0;
	}

	//------------------------------------------------------------------------------
	public void SetSaveData( int number ) {
        ResetParams();
        if( number != 0 )
        {
            GameObject block = StockFieldController.Instance.CreateBlock();
            var bctrl = block.GetComponent<BlockController>();
            bctrl.SetNumberImidiately( number );
            block.transform.SetParent( slot.transform );
            block.transform.localPosition = new Vector3( 0.0f, 0.0f, 0.0f );
            block.transform.localScale = new Vector3 ( 1.0f, 1.0f, 1.0f );
            bctrl.UpdateSkin();
        }
	}
	//------------------------------------------------------------------------------
    public void UpdateSkin()
    {
        slot.UpdateSkin();
        GameObject block = slot.block;
        if( block != null )
        {
            block.GetComponent<BlockController>().UpdateSkin();
        }
    }

	/* //------------------------------------------------------------------------------ */
    /* void OnApplicationFocus( bool focus_status ) */
    /* { */
        /* Debug.Log( "keep OnApplicationFocus " + focus_status ); */
    /* } */

    /* //------------------------------------------------------------------------------ */
    /* void OnApplicationPause( bool pause_status ) */
    /* { */
        /* Debug.Log( "keep OnApplicationPause " + pause_status ); */
    /* } */

    /* //------------------------------------------------------------------------------ */
    /* void OnApplicationQuit() */
    /* { */
        /* Debug.Log( "keep OnApplicationQuit" ); */
    /* } */

}
