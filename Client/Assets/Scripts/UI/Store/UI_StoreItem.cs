using Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class UI_StoreItem : UI_Base
{
    [SerializeField] private TextMeshProUGUI Itemname;
    [SerializeField] private TextMeshProUGUI ItemPrice;
    [SerializeField] private Image ItemImage;
    [SerializeField] private Button BuyButton;

    public int ItemId { get; private set; }


    public override void Init()
    {
        BuyButton.onClick.AddListener(OnBuyButtonClick);
    }

    void OnBuyButtonClick()
    {

    }

    public void SetInfo(CAItemData itemData)
    {
        ItemId = itemData.id;
        Itemname.text = itemData.itemName;

        Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
        ItemImage.sprite = icon;
        //        public int id;
        //public string itemName;
        //public CAStoreItemInfo storItemInfo;
        // 나중에 Item Image Path도 추가하자. 
    }
}
