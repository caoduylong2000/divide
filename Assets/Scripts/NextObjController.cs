using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WK.Unity;

public class NextObjController : MonoBehaviour {
    [SerializeField]
    protected Text text;
    [SerializeField]
    protected Image image;
    [SerializeField]
    protected Image highlightImage;
    [SerializeField]
    protected Image shadowImage;

    protected int number;

	//------------------------------------------------------------------------------
    protected void Awake()
    {
    }

    public void Init( int num )
    {
		number = num;
		text.text = number.ToString();
        updateSkin();
    }

	void updateSkin() {
        image.color = ColorManager.Instance.GetBlockColor( number );

        image.sprite = ColorManager.Instance.BlockSprite;
        highlightImage.sprite = ColorManager.Instance.BlockHighlightSprite;
        highlightImage.gameObject.SetActive( highlightImage.sprite != null );
        shadowImage.sprite = ColorManager.Instance.BlockShadowSprite;
        shadowImage.gameObject.SetActive( shadowImage.sprite != null );
    }
}
