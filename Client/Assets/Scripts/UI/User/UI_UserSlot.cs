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
    //public RectTransform slot; // ������ RectTransform

    [SerializeField] Texture2D AnimBaseTexture;

    //[SerializeField] private RectTransform characterImage;


    [SerializeField] private Transform CharacterObjectImage;
   // public RectTransform characterImage; // ĳ���� �̹����� RectTransform
   // public SpriteRenderer characterSpriteRenderer; // ĳ���� ��������Ʈ ������


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
       

       // ���̻� ������.
       //// if (characterSpriteRenderer.sprite == null)
       ////     return;

       // // ĳ���� ��������Ʈ�� ũ��
       // Vector2 spriteSize = new Vector2(AnimBaseTexture.width, AnimBaseTexture.height);
   
       // //Vector2 spriteSize = characterSpriteRenderer.sprite.bounds.size;

       // // ������ ũ��
       // Vector2 slotSize = GetComponent<RectTransform>().rect.size;

       // // ĳ������ ���� ���̸� ������ �������� ����
       // float characterWidth = slotSize.x / 2;
       // // ���� ���̴� ��������Ʈ ������ ���� ����
       // float characterHeight = characterWidth * (spriteSize.y / spriteSize.x);


       // // Image�� ����ϴ� ��� 
       // //RectTransform ����
       // characterImage.pivot = new Vector2(0.5f, 0); // �� ��ġ�� �ǹ����� ����
       // characterImage.anchorMin = new Vector2(0.4f, 0.1f);
       // characterImage.anchorMax = new Vector2(0.4f, 0.1f);
       // characterImage.anchoredPosition = new Vector2(0, 0);
       // characterImage.sizeDelta = new Vector2(characterWidth, characterHeight);
       // characterImage.localScale = Vector3.one; // �������� 1�� ����


         

        
        GetRawImage((int)RawImages.Character).rectTransform.anchorMin = new Vector2(0, 0);
        GetRawImage((int)RawImages.Character).rectTransform.anchorMax = new Vector2(0, 0);
        GetRawImage((int)RawImages.Character).rectTransform.pivot = new Vector2(0.20f, -0.05f);
        GetRawImage((int)RawImages.Character).rectTransform.sizeDelta =
            new Vector2(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.width);
        


    }

    public void AdjustCharacterUV(int num)
    {
        //float x = (num % 4) * 0.25f; // ���� ���� ��ġ (0, 0.25, 0.5, 0.75)
        //float y = 1 - ((num / 4 + 1) * 0.5f);  // ���� ���� ��ġ (0, 0.5)


        //GetRawImage((int)RawImages.Character).uvRect = new Rect(x, y, 0.25f, 0.5f);


        // ī�޶� ĸó�ϴ� ����
        //float cameraWidth = 8f;
        //float cameraHeight = 6f;

        //// �� ĳ������ ���� ��ǥ (1, 0)���� (3, 2)���� ���
        //float unitWidth = cameraWidth / 4; // ���� �������� 4�� ĳ����
        //float unitHeight = cameraHeight / 2; // ���� �������� 2�� ĳ����

        //float x = (num % 4) * unitWidth / cameraWidth;
        //float y = 1 - ((num / 4 + 1) * unitHeight / cameraHeight);

        //GetRawImage((int)RawImages.Character).uvRect = new Rect(x, y, unitWidth / cameraWidth, unitHeight / cameraHeight);


        // ũ�� ������ ��Ⱦ�� 4: 3�̴�.
        float cameraWidth = 8f;
        float cameraHeight = 6f;

        // �̴� UI Camera�� Size�� ���ؼ� ����. Size�� �ϵ��ڵ��Ǿ� ����.
        float unitWidth = 2f;
        float unitHeight = 2f;

        // �� ĳ������ UV ũ��
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
