using UnityEngine;
using UnityEngine.EventSystems;

//------------------------------------------------------------------------------
//StockFieldSlotController
//------------------------------------------------------------------------------
public class SFSlotController : SlotController {
    void Awake()
    {
        base.Awake();
        SetIndex( -1, -1 );
    }
}
