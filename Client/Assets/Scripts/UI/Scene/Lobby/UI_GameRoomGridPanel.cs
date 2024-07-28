using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameRoomGridPanel : UI_Base
{
    [SerializeField]
    private UI_RoomItem roomInfo_UI;

    [SerializeField]
    private RectTransform CanvasRect;


    private List<UI_RoomItem> RoomInfoList = new List<UI_RoomItem>();

    private GridLayoutGroup gridLayoutGroup   { get; set; }

    

    public override void Init()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        RectTransform rectTransform = GetComponent<RectTransform>();

        float screenWidth = CanvasRect.rect.width;
        float screenHeight = CanvasRect.rect.height;

        Debug.Log($"Screen Width :  {screenWidth}");
        Debug.Log($"Screen Height :  {screenHeight}");

        Vector2 Diff = rectTransform.anchorMax - rectTransform.anchorMin;



        float CellWidth = rectTransform.rect.width / 2;
        float CellHeight = rectTransform.rect.height / 4;

        CellWidth = screenWidth * Diff.x / 2;
        CellHeight = screenHeight * Diff.y / 4;


        gridLayoutGroup.cellSize = new Vector2(CellWidth, CellHeight);

        for (int i = 0; i < 8; ++i)
        {
            UI_RoomItem item = Instantiate(roomInfo_UI, gridLayoutGroup.transform);
            
        }
    }

     
    public void UpdateItemSize()
    {

    }

    public void AddItem()
    {
        
    }
}
