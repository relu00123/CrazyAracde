using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UsersGridPanel : UI_Base
{
    [SerializeField]
    private UI_GameRoomUserSlot userSlot_UI;

    [SerializeField]
    private RectTransform CanvasRect;

    private GridLayoutGroup gridLayoutGroup { get; set; }

    private const int UserSlotsPerPage = 8;

    private UI_GameRoomUserSlot[] UsersList = new UI_GameRoomUserSlot[UserSlotsPerPage];
    //private List<UI_GameRoomUserSlot> UsersList = new List<UI_GameRoomUserSlot>();

    public override void Init()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        RectTransform rectTransform = GetComponent<RectTransform>();

        float screenWidth = CanvasRect.rect.width;
        float screenHeight = CanvasRect.rect.height;

        Vector2 Diff = rectTransform.anchorMax - rectTransform.anchorMin;

        float CellWidth = rectTransform.rect.width / 4;
        float CellHeight = rectTransform.rect.height / 2;

        CellWidth = screenWidth * Diff.x / 4;
        CellHeight = screenHeight * Diff.y / 2;

        gridLayoutGroup.cellSize = new Vector2(CellWidth, CellHeight);

        for (int i = 0; i < UserSlotsPerPage; ++i)
        {
            UI_GameRoomUserSlot item = Instantiate(userSlot_UI, gridLayoutGroup.transform);
            item.uiUserSlot.AdjustCharacterUV(i);
            item.SetSlotIndex(i);
            //UsersList.Add(item);
            UsersList[i] = item;
        }
    }

    public void SetName(int index, string name)
    {
        if (index >= 0  && index < UsersList.Length) 
            UsersList[index].SetName(name);
    }

    public void ClearSlot(int index)
    {
        if (index >= 0 && index < UsersList.Length)
            UsersList[index].ClearSlot();
    }

    public void SetCharState(int index , GameRoomCharacterStateType charState)
    {
        if (index >= 0 && index < UsersList.Length)
            UsersList[index].SetCharState(charState);
    }
}
