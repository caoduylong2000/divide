using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

//------------------------------------------------------------------------------
//KeepFieldSlotController
//------------------------------------------------------------------------------
public class KFSlotController : SlotController {
    void Awake()
    {
        base.Awake();
        SetIndex( -1, -1 );
    }

	public override void OnDrop (PointerEventData eventData)
	{
		if( !block && BlockController.blockBeginDragged )
		{
            GameObject stock_field = GameObject.Find("StockField");
            StockFieldController stock_field_ctrl = stock_field.GetComponent<StockFieldController>();
            stock_field_ctrl.FillStocks();
            ResumeManager.Instance.RequestUpdate();
        }

        /* Debug.Log( "on dropping" ); */
		base.OnDrop( eventData );
		Assert.IsNotNull( block );
    }
}
