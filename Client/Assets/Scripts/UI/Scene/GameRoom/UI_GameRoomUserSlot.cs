using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameRoomUserSlot : UI_Base
{
    enum Images
    {
        SlotBackGround
    }

    public UI_UsersGridPanel ParentGrid { get; set; }

    public override void Init()
    {
        Bind<Image>(typeof(Images));

        // Scale (0, 0) 잡혀있는 것 (1, 1)로 초기화
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(1, 1, rectTransform.localScale.z);

        // 현재 Resolution에 맞게 Item Size 조절하기
        ParentGrid = GetComponentInParent<UI_UsersGridPanel>();

        Vector2 cellSize = ParentGrid.GetComponent<GridLayoutGroup>().cellSize;

        RectTransform SlotBGTransform = GetImage((int)Images.SlotBackGround).GetComponent<RectTransform>();
        SlotBGTransform.sizeDelta = new Vector2(cellSize.x, cellSize.y);
    }

}
