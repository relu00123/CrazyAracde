using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SevenSegment : UI_Base
{
    [SerializeField] public int digitCount; // 자릿수
    public Sprite emptyZero; // 빈 공간을 0으로 채울 0 사진
    public Sprite commaSprite; // 콤마 사진
    public Sprite emptyCommaSprite; // 빈 콤마 사진
    public List<Sprite> numberSprites; // 각 숫자에 대한 사진들 ( 0 - 9 )

    private List<Image> digitImages; // 자릿수를 표시할 Image 리스트


    [SerializeField] private GameObject numberPanel; // NumberPanel;
     

    public override void Init()
    {
        InitializeDisplay();
    }

    void InitializeDisplay()
    {
        digitImages = new List<Image>();

        for (int i = 0; i < digitCount; i++)
        {
            GameObject digitObj = new GameObject("Digit" + i);
            Image DigitImage = digitObj.AddComponent<Image>();
            DigitImage.sprite = emptyZero;
            digitImages.Add(DigitImage);
            digitObj.transform.SetParent(numberPanel.transform, false); // 여기 코드 살짝 이해가 안됨.

            // 3칸마다 콤마를 넣는다.  이게 돈을 표시하는 거라서 뒤에서 부터 세야할 듯?
            if (i % 3 == 2 && i != digitCount - 1)
            {
                GameObject commaObj = new GameObject("Comma" + i);
                Image commaImage = commaObj.AddComponent<Image>();
                commaImage.sprite = emptyCommaSprite;
                commaObj.transform.SetParent(numberPanel.transform, false);
            }
        }
    }

    public void UpdateDisplay(int number)
    {
        string numberStr = number.ToString().PadLeft(digitCount, '0');

        for (int i = 0; i < digitCount; i++)
        {
            int digit = int.Parse(numberStr[i].ToString());
            digitImages[i].sprite = numberSprites[digit];
        }
    }
}
