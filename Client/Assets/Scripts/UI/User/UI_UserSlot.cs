using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



enum RawImages
{
    Character,
}



public class UI_UserSlot : UI_Base
{
    //public RectTransform slot; // 슬롯의 RectTransform

    [SerializeField] Texture2D AnimBaseTexture;

    [SerializeField] private RectTransform characterImage;


    [SerializeField] private Transform CharacterObjectImage;
   // public RectTransform characterImage; // 캐릭터 이미지의 RectTransform
   // public SpriteRenderer characterSpriteRenderer; // 캐릭터 스프라이트 렌더러


    //[SerializeField] private TextMeshProUGUI Itemname;
    //[SerializeField] private TextMeshProUGUI ItemPrice;
    //[SerializeField] private Image ItemImage;
    //[SerializeField] private Button BuyButton;


    public override void Init()
    {
        Bind<RawImage>(typeof(RawImages));

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


        // Image를 사용하는 경우 
        //RectTransform 설정
        characterImage.pivot = new Vector2(0.5f, 0); // 발 위치를 피벗으로 설정
        characterImage.anchorMin = new Vector2(0.4f, 0.1f);
        characterImage.anchorMax = new Vector2(0.4f, 0.1f);
        characterImage.anchoredPosition = new Vector2(0, 0);
        characterImage.sizeDelta = new Vector2(characterWidth, characterHeight);
        characterImage.localScale = Vector3.one; // 스케일을 1로 설정


        //RectTransform characterObjectRectTransform = CharacterObjectImage as RectTransform;

        //// Object를 사용하는 경우
        //characterObjectRectTransform.pivot = new Vector2(0.5f, 0); // 발 위치를 피벗으로 설정
        //characterObjectRectTransform.anchorMin = new Vector2(0.4f, 0.1f);
        //characterObjectRectTransform.anchorMax = new Vector2(0.4f, 0.1f);
        //characterObjectRectTransform.anchoredPosition = new Vector2(0, 0);
        //characterObjectRectTransform.sizeDelta = new Vector2(characterWidth, characterHeight);
        //characterObjectRectTransform.localScale = Vector3.one; // 스케일을 1로 설정
    }


    public void TestFunc(int num)
    {
        Debug.Log($"TestFunc Called : {num} ");
    }


    public void AdjustCharacterUV(int num)
    {
        float x = (num % 4) * 0.25f; // 가로 방향 위치 (0, 0.25, 0.5, 0.75)
        float y = 1 - ((num / 4 + 1) * 0.5f);  // 세로 방향 위치 (0, 0.5)


        GetRawImage((int)RawImages.Character).uvRect = new Rect(x, y, 0.25f, 0.5f);
    }
}
