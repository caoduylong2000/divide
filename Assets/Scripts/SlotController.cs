using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WK.Unity;

public class SlotController : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
    //public Color colorMouseOver;
    //protected Color colorNormal;
	public int X { get; protected set; }
	public int Y { get; protected set; }

	public static float lastDroppedTime;
    protected Image image;
    protected Transform cachedTransform;

	//------------------------------------------------------------------------------
    protected void Awake()
    {
        image = GetComponent< Image >();
        //image.color = ColorManager.Instance.SlotColor;
        UpdateSkin();
        cachedTransform = transform;
    }

	//------------------------------------------------------------------------------
	public void SetIndex( int x, int y )
	{
		X = x;
		Y = y;
	}

	public GameObject block {
		get {
			if( cachedTransform.childCount > 0 )
			{
                Transform blk_tfm = cachedTransform.Find( "Block(Clone)" );
                if( blk_tfm )
                {
                    return blk_tfm.gameObject;
                }
                return null;
			}
			else
			{
				return null;
			}
		}
	}

	public virtual void OnDrop (PointerEventData eventData)
	{
		if( !block && BlockController.blockBeginDragged )
		{
            Transform blockBeginDraggedTransform = BlockController.blockBeginDragged.transform;
			blockBeginDraggedTransform.SetParent( cachedTransform );
			blockBeginDraggedTransform.localPosition = new Vector3( 0.0f, 0.0f, 0.0f );

			MainFieldController.Instance.RequestCheck();

			DebugLog.Write( block.GetComponent<BlockController>().number.ToString() + " " + X.ToString() + " " + Y.ToString() );

			lastDroppedTime = Time.time;
		}
	}

    public virtual void OnPointerEnter( PointerEventData eventData )
    {
		if( !block && BlockController.blockBeginDragged )
        {
            image.color = ColorManager.Instance.SlotMouseOverColor;
            //Debug.Log( "OnPointerEnter : " + X + "," + Y );
            MainFieldController.Instance.ClearDividableColor();

            //ブロックをドロップ前にハイライトする場合以下を有効に
			//MainFieldController.Instance.ChangeColorDividable(
            //        BlockController.blockBeginDragged.GetComponent<BlockController>().number
            //        , X
            //        , Y
            //        );
        }
    }

    public void SetColorNormal()
    {
        image.color = ColorManager.Instance.SlotColor;
    }

    public virtual void OnPointerExit( PointerEventData eventData )
    {
        SetColorNormal();

        //連鎖中のDividableColorが、これでリセットされてしまうが
        //黙認
		if( !block && BlockController.blockBeginDragged )
        {
            MainFieldController.Instance.ClearDividableColor();
        }
    }

    public virtual void UpdateSkin()
    {
        image.color = ColorManager.Instance.SlotColor;
        image.sprite = ColorManager.Instance.SlotSprite;
    }
}
