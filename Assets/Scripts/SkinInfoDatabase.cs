using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

[Serializable]
public class SkinInfo
{
    public bool isUseBlockColorTable;
    public bool isPurchased;
    public int price;
    public Color bg;
    public Sprite bgImage;
    public Color icon;
    public Color block;
    public Color tutorialBG;
    public Color slot;
    public Color slotMouseOver;
    public Color timeLimitBgColor;
    public Sprite blockSprite;
    public Sprite blockHighlightSprite;
    public Sprite blockShadowSprite;
    public Sprite slotSprite;
    public Sprite effectSprite;
    public int titleObjIndex;

    public void PurchaseSkin()
    {
        if (MoneySystem.Instance.SubtractMoney(price) > 0)
        {
            MoneySystem.Instance.SubtractMoney(price);
            isPurchased = true;
            SaveSkinInfo();
        }
        else
            Debug.Log("Not enough GP");
    }

    public void SaveSkinInfo()
    {
        ES3.Save<bool>("skin_" + titleObjIndex + "_isPurchased", isPurchased);
    }

    public void LoadSkinInfo()
    {
        isPurchased = ES3.Load<bool>("skin_" + titleObjIndex + "_isPurchased", isPurchased);
    }


};

public class SkinInfoDatabase : WK.Unity.SingletonScriptableObject<SkinInfoDatabase>
{
    [SerializeField]
    protected SkinInfo[] skinInfos;
    public SkinInfo[] SkinInfos { get { return skinInfos; } }

    private void Awake()
    {
        foreach (SkinInfo skinInfo in skinInfos)
        {
            skinInfo.LoadSkinInfo();
        }
    }

    public void UpdateSkinPurchasedStatus(int skinIndex)
    {
        if (skinIndex >= 0 && skinIndex < skinInfos.Length)
        {
            if (!skinInfos[skinIndex].isPurchased)
            {
                skinInfos[skinIndex].PurchaseSkin();
            }
        }
    }

};

