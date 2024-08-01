using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UsersGridPanel : UI_Base
{
    [SerializeField]
    private UI_UserSlot userSlot_UI;

    [SerializeField]
    private RectTransform CanvasRect;

    private GridLayoutGroup gridLayoutGroup { get; set; }

    private const int UserSlotsPerPage = 8;

    private List<UI_UserSlot> UsersList = new List<UI_UserSlot>();

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
            UI_UserSlot item = Instantiate(userSlot_UI, gridLayoutGroup.transform);
            UsersList.Add(item);
        }
    }
}
