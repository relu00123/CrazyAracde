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

    //[SerializeField] private RectTransform characterImage;


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
       

       // 더이상 사용안함.
       //// if (characterSpriteRenderer.sprite == null)
       ////     return;

       // // 캐릭터 스프라이트의 크기
       // Vector2 spriteSize = new Vector2(AnimBaseTexture.width, AnimBaseTexture.height);
   
       // //Vector2 spriteSize = characterSpriteRenderer.sprite.bounds.size;

       // // 슬롯의 크기
       // Vector2 slotSize = GetComponent<RectTransform>().rect.size;

       // // 캐릭터의 가로 길이를 슬롯의 절반으로 설정
       // float characterWidth = slotSize.x / 2;
       // // 세로 길이는 스프라이트 비율에 따라 설정
       // float characterHeight = characterWidth * (spriteSize.y / spriteSize.x);


       // // Image를 사용하는 경우 
       // //RectTransform 설정
       // characterImage.pivot = new Vector2(0.5f, 0); // 발 위치를 피벗으로 설정
       // characterImage.anchorMin = new Vector2(0.4f, 0.1f);
       // characterImage.anchorMax = new Vector2(0.4f, 0.1f);
       // characterImage.anchoredPosition = new Vector2(0, 0);
       // characterImage.sizeDelta = new Vector2(characterWidth, characterHeight);
       // characterImage.localScale = Vector3.one; // 스케일을 1로 설정


         

        
        GetRawImage((int)RawImages.Character).rectTransform.anchorMin = new Vector2(0, 0);
        GetRawImage((int)RawImages.Character).rectTransform.anchorMax = new Vector2(0, 0);
        GetRawImage((int)RawImages.Character).rectTransform.pivot = new Vector2(0.20f, -0.05f);
        GetRawImage((int)RawImages.Character).rectTransform.sizeDelta =
            new Vector2(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.width);
        


    }

    public void AdjustCharacterUV(int num)
    {
        //float x = (num % 4) * 0.25f; // 가로 방향 위치 (0, 0.25, 0.5, 0.75)
        //float y = 1 - ((num / 4 + 1) * 0.5f);  // 세로 방향 위치 (0, 0.5)


        //GetRawImage((int)RawImages.Character).uvRect = new Rect(x, y, 0.25f, 0.5f);


        // 카메라가 캡처하는 영역
        //float cameraWidth = 8f;
        //float cameraHeight = 6f;

        //// 각 캐릭터의 유닛 좌표 (1, 0)부터 (3, 2)까지 계산
        //float unitWidth = cameraWidth / 4; // 가로 방향으로 4개 캐릭터
        //float unitHeight = cameraHeight / 2; // 세로 방향으로 2개 캐릭터

        //float x = (num % 4) * unitWidth / cameraWidth;
        //float y = 1 - ((num / 4 + 1) * unitHeight / cameraHeight);

        //GetRawImage((int)RawImages.Character).uvRect = new Rect(x, y, unitWidth / cameraWidth, unitHeight / cameraHeight);


        // 크아 게임은 종횡비가 4: 3이다.
        float cameraWidth = 8f;
        float cameraHeight = 6f;

        // 이는 UI Camera의 Size로 인해서 결정. Size는 하드코딩되어 있음.
        float unitWidth = 2f;
        float unitHeight = 2f;

        // 각 캐릭터의 UV 크기
        float scale_x = unitWidth / cameraWidth;
        float scale_y = unitHeight / cameraHeight;

        float start_x = (num % 4) * scale_x;

        float start_y = (num < 4) ? 0.5f : 0.0f;


        //float start_y = 1f / 6f;

        //float start_y = 0.5f - (num / 8f);
        //Debug.Log($" start_y : {start_y}");
        //float start_y = 1f - ((num / (float)4 + 1) * scale_y);


        GetRawImage((int)RawImages.Character).uvRect = new Rect(start_x, start_y, scale_x, scale_y);

    }
}
