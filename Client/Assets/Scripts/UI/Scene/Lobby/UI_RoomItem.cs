using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RoomItem : UI_Base
{
    public UI_GameRoomGridPanel ParentGrid { get; set; }


    enum RoomInfoImages
    {
        EmptyRoom,
        RoomExist,
    }


    Dictionary<RoomInfoImages, string> SpritePath = new Dictionary<RoomInfoImages, string>()
    {
        { RoomInfoImages.EmptyRoom,  "Textures/GameRoom/Info/EmptyRoom" },
        { RoomInfoImages.RoomExist,     "Textures/GameRoom/Info/ExistringRoom"}
    };


    enum Objects
    {
        
    }
    enum Images
    {
        Highlighted,
        Normal,
        GameState,
        SecretMode,
        TeamMode
    }

    enum Texts
    {
        RoomNumber,
        RoomName,
        RoomCapacity,

    }

    enum Buttons
    {
        RoomStateBtn
    }


    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(Images));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(1, 1, rectTransform.localScale.z);

        ParentGrid = GetComponentInParent<UI_GameRoomGridPanel>();

        Vector2 cellSize = ParentGrid.GetComponent<GridLayoutGroup>().cellSize;

        Debug.Log($"Normal Name : {GetObject((int)Images.Normal).name}");
        Debug.Log($"Normal Name : {GetObject((int)Images.Highlighted).name}");

        RectTransform normalRectTransform = GetObject((int)Images.Normal).GetComponent<RectTransform>();
        normalRectTransform.sizeDelta = new Vector2(cellSize.x, cellSize.y);

        RectTransform HighlightedRectTransform = GetObject((int)Images.Highlighted).GetComponent<RectTransform>();
        HighlightedRectTransform.sizeDelta = new Vector2(cellSize.x, cellSize.y);

        Debug.Log("ItemTest!");
    }

    public void SetEmpty()
    {
        // Set Empty Function Called!
        GetButton((int)Buttons.RoomStateBtn).GetComponent<Image>().sprite = Resources.Load<Sprite>(SpritePath[RoomInfoImages.EmptyRoom]);
    }



    
}
