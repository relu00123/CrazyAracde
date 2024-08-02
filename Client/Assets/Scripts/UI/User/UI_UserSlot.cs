using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_UserSlot : UI_Base
{
    //public RectTransform slot; // ������ RectTransform

    [SerializeField] Texture2D AnimBaseTexture;

    [SerializeField] private RectTransform characterImage;  
   // public RectTransform characterImage; // ĳ���� �̹����� RectTransform
   // public SpriteRenderer characterSpriteRenderer; // ĳ���� ��������Ʈ ������


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

        // ĳ���� ��������Ʈ�� ũ��
        Vector2 spriteSize = new Vector2(AnimBaseTexture.width, AnimBaseTexture.height);
   
        //Vector2 spriteSize = characterSpriteRenderer.sprite.bounds.size;

        // ������ ũ��
        Vector2 slotSize = GetComponent<RectTransform>().rect.size;

        // ĳ������ ���� ���̸� ������ �������� ����
        float characterWidth = slotSize.x / 2;
        // ���� ���̴� ��������Ʈ ������ ���� ����
        float characterHeight = characterWidth * (spriteSize.y / spriteSize.x);

        // RectTransform ����
        characterImage.pivot = new Vector2(0.5f, 0); // �� ��ġ�� �ǹ����� ����
        characterImage.anchorMin = new Vector2(0.4f, 0.1f);
        characterImage.anchorMax = new Vector2(0.4f, 0.1f);
        characterImage.anchoredPosition = new Vector2(0, 0);
        characterImage.sizeDelta = new Vector2(characterWidth, characterHeight);
        characterImage.localScale = Vector3.one; // �������� 1�� ����
    }

}
