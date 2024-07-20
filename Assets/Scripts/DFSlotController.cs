using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using WK.Unity;

//------------------------------------------------------------------------------
//DustbinFieldSlotController
//------------------------------------------------------------------------------
public class DFSlotController : SlotController {
    public Sprite openSprite;
    public Sprite closeSprite;
    private Image binImage;
    private Text  dustBinQuestionText;

    void Start()
    {
        binImage = transform.Find( "DustbinImage" ).GetComponent< Image >();
        dustBinQuestionText = GameObject.Find( "DustBinQuestionText" ).GetComponent< Text >();
    }

    public override void OnPointerEnter( PointerEventData eventData )
    {
		if( !block && BlockController.blockBeginDragged )
        {
            image.color = ColorManager.Instance.SlotMouseOverColor;
            binImage.sprite = openSprite;
        }
    }

    public override void OnPointerExit( PointerEventData eventData )
    {
        image.color = ColorManager.Instance.SlotColor;
        binImage.sprite = closeSprite;
    }

	public override void OnDrop (PointerEventData eventData)
	{
		if( !block && BlockController.blockBeginDragged )
        {
            GameSceneManager gsm = GameObject.Find( "GameSceneManager" ).GetComponent< GameSceneManager >();
            gsm.GoToDustbinMenu();

            Vector3 dust_bin_world_pos = binImage.GetComponent< RectTransform >().TransformPoint( new Vector3( 0, 0, 0 ) );
            /* Debug.Log( dust_bin_world_pos ); */
            /* dust_bin_world_pos = binImage.GetComponent< RectTransform >().InverseTransformPoint( dust_bin_world_pos ); */
            /* dust_bin_world_pos = GetComponent< RectTransform >().InverseTransformPoint( dust_bin_world_pos ); */
            /* dust_bin_world_pos = transform.parent.parent.GetComponent< RectTransform >().InverseTransformPoint( dust_bin_world_pos ); */
            /* dust_bin_world_pos = GameObject.Find( "SFSlot" ).GetComponent< RectTransform >().InverseTransformPoint( dust_bin_world_pos ); */
            /* Debug.Log( dust_bin_world_pos ); */
            BlockController.blockBeginDragged.GetComponent< BlockController >().GoToDustbin( dust_bin_world_pos );
            BlockController.blockInDustbin = BlockController.blockBeginDragged;

            var message = "";
            GameSceneManager.Instance.GetTagProcessor().Num
                = BlockController.blockInDustbin.GetComponent< BlockController >().number.ToString();

            message = WK.Translate.TranslateManager.Instance.GetText( "dustbin" );

            //if ( Config.Language == SystemLanguage.Japanese)
            //{
            //}
            //else
            //{
            //    message = "Will you throw away \"" +
            //        BlockController.blockInDustbin.GetComponent< BlockController >().number.ToString()
            //        + "\"?\n(Movie ads will start.)";
            //}
            dustBinQuestionText.text = message;
            CommonDialogManager.Instance.SetDialog( message
                    , () => GameSceneManager.Instance.ShowRewardVideoForDustbin()
                    , () => GameSceneManager.Instance.BackToTheGameFromDustbinMenu()
                    );
            CommonDialogManager.Instance.EnterYesNoDialog();
        }

		/* if( !block && BlockController.blockBeginDragged ) */
		/* { */
            /* GameObject stock_field = GameObject.Find("StockField"); */
            /* StockFieldController stock_field_ctrl = stock_field.GetComponent<StockFieldController>(); */
            /* stock_field_ctrl.FillStocks(); */
        /* } */

		/* base.OnDrop( eventData ); */
		/* Assert.IsNotNull( block ); */
    }

    public override void UpdateSkin()
    {
        //image.color = ColorManager.Instance.SlotColor;
        //image.sprite = ColorManager.Instance.SlotSprite;
    }
}
