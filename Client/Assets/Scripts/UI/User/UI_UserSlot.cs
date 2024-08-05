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

    [SerializeField] private RectTransform characterImage;


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
       


       // if (characterSpriteRenderer.sprite == null)
       //     return;

        // ĳ���� ��������Ʈ�� ũ��
        Vector2 spriteSize = new Vector2(AnimBaseTexture.width, AnimBaseTexture.height);
   
        //Vector2 spriteSize = characterSpriteRenderer.sprite.bounds.size;

        // ������ ũ��
        Vector2 slotSize = GetComponent<RectTransform>().rect.size;

        // ĳ������ ���� ���̸� ������ �������� ����
        float characterWidth = slotSize.x / 2;
        // ���� ���̴� ��������Ʈ ������ ���� ����
        float characterHeight = characterWidth * (spriteSize.y / spriteSize.x);


        // Image�� ����ϴ� ��� 
        //RectTransform ����
        characterImage.pivot = new Vector2(0.5f, 0); // �� ��ġ�� �ǹ����� ����
        characterImage.anchorMin = new Vector2(0.4f, 0.1f);
        characterImage.anchorMax = new Vector2(0.4f, 0.1f);
        characterImage.anchoredPosition = new Vector2(0, 0);
        characterImage.sizeDelta = new Vector2(characterWidth, characterHeight);
        characterImage.localScale = Vector3.one; // �������� 1�� ����


        //RectTransform characterObjectRectTransform = CharacterObjectImage as RectTransform;

        //// Object�� ����ϴ� ���
        //characterObjectRectTransform.pivot = new Vector2(0.5f, 0); // �� ��ġ�� �ǹ����� ����
        //characterObjectRectTransform.anchorMin = new Vector2(0.4f, 0.1f);
        //characterObjectRectTransform.anchorMax = new Vector2(0.4f, 0.1f);
        //characterObjectRectTransform.anchoredPosition = new Vector2(0, 0);
        //characterObjectRectTransform.sizeDelta = new Vector2(characterWidth, characterHeight);
        //characterObjectRectTransform.localScale = Vector3.one; // �������� 1�� ����
    }


    public void TestFunc(int num)
    {
        Debug.Log($"TestFunc Called : {num} ");
    }


    public void AdjustCharacterUV(int num)
    {
        float x = (num % 4) * 0.25f; // ���� ���� ��ġ (0, 0.25, 0.5, 0.75)
        float y = 1 - ((num / 4 + 1) * 0.5f);  // ���� ���� ��ġ (0, 0.5)


        GetRawImage((int)RawImages.Character).uvRect = new Rect(x, y, 0.25f, 0.5f);
    }
}
