using UnityEngine;
using UnityEngine.UI;
using WK.Unity;
using Sirenix.OdinInspector;

public class ColorManager : SingletonMonoBehaviour< ColorManager > {
    //[SerializeField]
    //Image bg;
    [SerializeField]
    Camera mainCamera;

    [SerializeField]
    Image[] bgImages;

    [SerializeField]
    Image[] icons;

    [SerializeField]
    Text[] uiTexts;

    [SerializeField]
    Image[] tutorialBGs;

    [SerializeField]
    Color bgColor;

    [SerializeField]
    Color iconColor;

    [SerializeField]
    Color tutorialBGColor;

    [SerializeField]
    GameObject[] titleObjs;

    public Color IconColor { get { return iconColor; } }

    [SerializeField]
    Color slotColor;

    public Color SlotColor { get { return slotColor; } }

    [SerializeField]
    Color slotMouseOverColor;

    public Color SlotMouseOverColor { get { return slotMouseOverColor; } }

    //[ReadOnly]
    [SerializeField]
    Color[] blockColors;

    Color blockColor;
    GameObject bgCover;

    public Sprite BlockSprite { get { return GetCurrSkinInfo().blockSprite; } }
    public Sprite BlockHighlightSprite { get { return GetCurrSkinInfo().blockHighlightSprite; } }
    public Sprite BlockShadowSprite { get { return GetCurrSkinInfo().blockShadowSprite; } }
    public Sprite SlotSprite { get { return GetCurrSkinInfo().slotSprite; } }
    public Sprite EffectSprite { get { return GetCurrSkinInfo().effectSprite; } }

    [ReadOnly]
    [SerializeField]
    bool isUseBlockColorTable = true;

    [SerializeField]
    DFSlotController dustbinCtrl;

    [SerializeField]
    Image timeLimitBg;

    const int BLOCK_COLOR_TABLE_SIZE = 100;
    public Color GetBlockColor( int num )
    {
        if( isUseBlockColorTable )
        {
            return blockColors[ Mathf.Min( num, BLOCK_COLOR_TABLE_SIZE - 1 ) ];
        }
        else
        {
            return blockColor;
        }
    }

    [SerializeField]
    float saturation;

    [SerializeField]
    float brightnessValue;

    override protected void Awake()
    {
        base.Awake();
    }

    [ContextMenu("makeBlockColors")]
    private void makeColor()
    {
        //緑部分は少し明度を下げる
        const float MAX_HUE = 0.65f;
        blockColors = new Color[ BLOCK_COLOR_TABLE_SIZE ];
        float hue = 0.0f;
        const int DARKEN_CENTER = 40;
        for( int i = 0; i < BLOCK_COLOR_TABLE_SIZE; ++i )
        {
            hue = ( MAX_HUE / BLOCK_COLOR_TABLE_SIZE ) * i;
            //hue = MAX_HUE - hue;
            var bright = brightnessValue;
            var sat = saturation;
            bright -= 0.30f * Mathf.Max( 1.0f - Mathf.Abs( DARKEN_CENTER - i ) / (float)DARKEN_CENTER, 0.0f );
            //if( i > 24 && i < 54)
            //{
            //    bright -= 0.1f;
            //    sat += 0.1f;
            //}
            blockColors[i] = Color.HSVToRGB( hue, sat, bright );
        }
    }

    void Start()
    {
        UpdateSkin();
    }

    [Button]
    public void UpdateSkinTest()
    {
        SaveData.Data.skinIndex = 1 - SaveData.Data.skinIndex;
        UpdateSkin();
    }

    [Button]
    public void UpdateSkin()
    {
        //プレイ中以外はスキップ
        if( !Application.isPlaying )
        {
            return ;
        }

        //set skin
        var skin = GetCurrSkinInfo();
        bgColor = skin.bg;
        iconColor = skin.icon;
        tutorialBGColor = skin.tutorialBG;
        slotColor = skin.slot;
        slotMouseOverColor = skin.slotMouseOver;
        blockColor = skin.block;
        isUseBlockColorTable = skin.isUseBlockColorTable;

        mainCamera.backgroundColor = bgColor;

        var bg_color = bgColor;
        for( int i = 0; i < bgImages.Length; ++i )
        {
            bg_color.a = bgImages[i].color.a;
            bgImages[i].color = bg_color;
        }

        for( int i = 0; i < icons.Length; ++i )
        {
            icons[i].color = iconColor;
        }

        for( int i = 0; i < uiTexts.Length; ++i )
        {
            uiTexts[i].color = iconColor;
        }

        for( int i = 0; i < tutorialBGs.Length; ++i )
        {
            tutorialBGs[i].color = tutorialBGColor;
        }

        for( int i = 0; i < titleObjs.Length; ++i )
        {
            titleObjs[i].SetActive( i == skin.titleObjIndex );
        }

        MainFieldController.Instance.UpdateSkin();
        StockFieldController.Instance.UpdateSkin();
        KeepFieldController.Instance.UpdateSkin();
        dustbinCtrl.UpdateSkin();
        timeLimitBg.sprite = skin.bgImage;
        if (skin.bgImage != null)
        {
            timeLimitBg.color = Color.white;
        }    
        else
        {
            timeLimitBg.color = skin.bg;
        }    
            


    }

    public SkinInfo GetCurrSkinInfo()
    {
        var index = SaveData.Data.skinIndex;
        return SkinInfoDatabase.Instance.SkinInfos[ index ];
    }
}
