using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CAGameScene : UI_Scene
{
    enum RawImages
    {
        GameImage
    }

    public override void Init()
    {
        base.Init();

        Bind<RawImage>(typeof(RawImages));

        RawImage img = GetRawImage((int)RawImages.GameImage);

        RectTransform rectTransform = img.GetComponent<RectTransform>();

        Vector2 anchorMin = rectTransform.anchorMin;
        Vector2 anchorMax = rectTransform.anchorMax;

        // 차이 계산 및 14/13 배 적용
        float deltaY = anchorMax.y - anchorMin.y;
        float newMaxY = anchorMin.y + deltaY * (14f / 13f);

        // 새로운 Max.y 값 설정
        rectTransform.anchorMax = new Vector2(anchorMax.x, newMaxY);
    }
}
