using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_UserSlot : UI_Base
{
    //public RectTransform slot; // 슬롯의 RectTransform

    [SerializeField] Texture2D AnimBaseTexture;

    [SerializeField] private RectTransform characterImage;  
   // public RectTransform characterImage; // 캐릭터 이미지의 RectTransform
   // public SpriteRenderer characterSpriteRenderer; // 캐릭터 스프라이트 렌더러


    //[SerializeField] private TextMeshProUGUI Itemname;
    //[SerializeField] private TextMeshProUGUI ItemPrice;
    //[SerializeField] private Image ItemImage;
    //[SerializeField] private Button BuyButton;


    public override void Init()
    {
        AdjustCharacterSizeAndPosition();
    }

    

    private void AdjustCharacterSizeAndPosition()
    {
       // if (characterSpriteRenderer.sprite == null)
       //     return;

        // 캐릭터 스프라이트의 크기
        Vector2 spriteSize = new Vector2(AnimBaseTexture.width, AnimBaseTexture.height);
   
        //Vector2 spriteSize = characterSpriteRenderer.sprite.bounds.size;

        // 슬롯의 크기
        Vector2 slotSize = GetComponent<RectTransform>().rect.size;

        // 캐릭터의 가로 길이를 슬롯의 절반으로 설정
        float characterWidth = slotSize.x / 2;
        // 세로 길이는 스프라이트 비율에 따라 설정
        float characterHeight = characterWidth * (spriteSize.y / spriteSize.x);

        // RectTransform 설정
        characterImage.pivot = new Vector2(0.5f, 0); // 발 위치를 피벗으로 설정
        characterImage.anchorMin = new Vector2(0.4f, 0.1f);
        characterImage.anchorMax = new Vector2(0.4f, 0.1f);
        characterImage.anchoredPosition = new Vector2(0, 0);
        characterImage.sizeDelta = new Vector2(characterWidth, characterHeight);
        characterImage.localScale = Vector3.one; // 스케일을 1로 설정
    }

}
