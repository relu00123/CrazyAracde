using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CA_ResourceManager
{
    private static CA_ResourceManager _instance;

    private Dictionary<CAItemType, Sprite> _itemSpriteDict;


    public static CA_ResourceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CA_ResourceManager();
            }

            return _instance;
        }
    }

    private CA_ResourceManager()
    {
        _itemSpriteDict = new Dictionary<CAItemType, Sprite>();


        LoadAllItemSprites();
    }


    public void LoadAllItemSprites()
    {
        // 각 아이템에 대한 스프라이트를 리소스에서 로드하여 Dictionary에 추가
        _itemSpriteDict[CAItemType.CaRollerSkate] = Resources.Load<Sprite>("Textures/Item_InGame/Roller_Skate");
        _itemSpriteDict[CAItemType.CaStreamMax] = Resources.Load<Sprite>("Textures/Item_InGame/Stream_Max");
        _itemSpriteDict[CAItemType.CaStreamPotion] = Resources.Load<Sprite>("Textures/Item_InGame/Stream_Potion");
        _itemSpriteDict[CAItemType.CaWaterBomb] = Resources.Load<Sprite>("Textures/Item_InGame/Water_Bomb");
    }

    public Sprite GetItemSprite(CAItemType itemType)
    {
        if (_itemSpriteDict.ContainsKey(itemType))
        {
            return _itemSpriteDict[itemType];
        }
        else
        {
            Debug.LogError($"스프라이트가 정의되지 않았습니다: {itemType}");
            return null;
        }
    }

}