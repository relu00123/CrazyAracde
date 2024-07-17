using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SevenSegment : UI_Base
{
    [SerializeField] public int digitCount; // �ڸ���
    public Sprite emptyZero; // �� ������ 0���� ä�� 0 ����
    public Sprite commaSprite; // �޸� ����
    public Sprite emptyCommaSprite; // �� �޸� ����
    public List<Sprite> numberSprites; // �� ���ڿ� ���� ������ ( 0 - 9 )

    private List<Image> digitImages; // �ڸ����� ǥ���� Image ����Ʈ


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
            digitObj.transform.SetParent(numberPanel.transform, false); // ���� �ڵ� ��¦ ���ذ� �ȵ�.

            // 3ĭ���� �޸��� �ִ´�.  �̰� ���� ǥ���ϴ� �Ŷ� �ڿ��� ���� ������ ��?
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
