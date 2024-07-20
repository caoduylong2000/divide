using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using WK.Audio;

public class SkinSelectButton : MonoBehaviour {
    static readonly Color COLOR_SELECTED = WK.Utils.Utils.GetRgbColor(0xEEDD99FF);
    static readonly Color COLOR_UNSELECTED = WK.Utils.Utils.GetRgbColor(0x222222FF);

    [SerializeField]
    protected int skinIndex;

    [SerializeField]
    protected Image skinImage;

    //[SerializeField]
    //protected GameObject lockObj;

    [SerializeField]
    protected Image selectedFrame;

    [SerializeField]
    protected SkinSelectGroup skinSelectGroup;

    //[SerializeField]
    //protected Image lockedImage;

    private GameSceneManager gameSceneManager;
    private MoneySystem moneySystem;
    private GameObject lockSkin;

    void Start()
    {
        gameSceneManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
        lockSkin = transform.Find("LockPurchase").gameObject;
    }

    public void Init( int skin_index )
    {
        skinIndex = skin_index;
        //skinImage.sprite = Parameters.Instance.skinImageSprites[ skin_index ];
        //selectedFrame.gameObject.SetActive( false );
        //var palette = Parameters.Instance.palettes[ skinIndex ];
        //int rare_index = (int)( palette.rare - 1.0f );
        //lockedImage.color = Parameters.Instance.rarityColors[ rare_index ];
    }

    //public void ShowSkinImage( bool is_show )
    //{
    //    skinImage.gameObject.SetActive( is_show );
    //    lockObj.SetActive( !is_show );
    //}

    private void Update()
    {
        if (SkinInfoDatabase.Instance.SkinInfos[skinIndex].isPurchased == true)
            if (lockSkin != null)
                lockSkin.SetActive(false);
    }

    public void ShowSelectFrame( bool is_show )
    {
        selectedFrame.color = is_show ? COLOR_SELECTED : COLOR_UNSELECTED;
    }

    public void OnClick()
    {
        SaveData.Data.skinIndex = skinIndex;
        if(SkinInfoDatabase.Instance.SkinInfos[skinIndex].isPurchased == true)
        {
            WK.Audio.SoundManager.Instance.PlaySe("click");
            SaveData.SaveMain();
            Debug.Log(SaveData.Data);
            ColorManager.Instance.UpdateSkin();
            skinSelectGroup.SetSkinSelectFrame(skinIndex);
        }
        else
        {
            GoToPurchase();
        }
    }

    public void GoToPurchase()
    {
        SoundManager.Instance.PlaySe("click");
        gameSceneManager.GoToPurchase();
    }

    public void BuySkin(int price) {

        price = SkinInfoDatabase.Instance.SkinInfos[skinIndex].price;
        moneySystem.SubtractMoney(price);
        //int index = SkinInfoDatabase.Instance.SkinInfos[skinIndex].titleObjIndex;
        gameSceneManager.BackToTheSkinMenu();
    }

}

