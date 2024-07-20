using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

//------------------------------------------------------------------------------
//MainFieldSlotController
//------------------------------------------------------------------------------
public class MFSlotController : SlotController {
    private static int sDropCounter = 0;

    public static int GetDropCounter()
    {
        return sDropCounter;
    }

    //for resume function
    public static void SetDropCounter( int count )
    {
        sDropCounter = count;
    }

    public static void ClearDropCounter()
    {
        sDropCounter = 0;
    }

	public override void OnDrop (PointerEventData eventData)
	{
		if( !block && BlockController.blockBeginDragged )
		{
            sDropCounter++;
            GameObject stock_field = GameObject.Find("StockField");
            StockFieldController stock_field_ctrl = stock_field.GetComponent<StockFieldController>();
            stock_field_ctrl.FillStocks();

            base.OnDrop( eventData );//ここでblockが更新される
            GameObject cachedBlock = block;
            Assert.IsNotNull( cachedBlock );
            cachedBlock.GetComponent< BlockController >().RequestDragable( false );
            int add_score = cachedBlock.GetComponent<BlockController>().number / 10;
			GameObject.Find ( "MainField" ).GetComponent< MainFieldController >().AddScore( add_score );
            ResumeManager.Instance.RequestUpdate();
        }
	}
}
