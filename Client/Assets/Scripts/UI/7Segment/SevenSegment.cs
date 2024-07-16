using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SevenSegment : UI_Base
{
    [SerializeField] private List<Sprite> DigitImages; // 0 ~ 9 까지의 숫자 이미지를 저장할 리스트
     
    [SerializeField] private Sprite commaSprite; // 콤마 이미지

    [SerializeField] private RectTransform DigitPanel;

    private List<Image> digitImages = new List<Image>();

     

    public override void Init()
    {
        InitializeDisplay();
    }

    private void InitializeDisplay()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject digitObject = new GameObject($"Digit_{i}");
            Image digitImage = digitObject.AddComponent<Image>();
            digitImage.transform.SetParent(DigitPanel, false);
            digitImages.Add(digitImage);

            if (i < 9 && (i + 1) % 3 == 0)
            {
                GameObject commaObject = new GameObject($"Comma_{i}");
                Image commaImage = commaObject.AddComponent<Image>();
                commaImage.sprite = commaSprite;
                commaImage.transform.SetParent(DigitPanel, false);
            }
        }
    }

    // 숫자를 설정하는 함수
    public void SetNumber(int number)
    {
        string numberStr = number.ToString().PadLeft(10, '0');

        for (int i = 0; i < 10; i++)
        {
            int digit = int.Parse(numberStr[i].ToString());
            //digitImages[i].sprite = digitSprites[digit];
        }
    }
}
