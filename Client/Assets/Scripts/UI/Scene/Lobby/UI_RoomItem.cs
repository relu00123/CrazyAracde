using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_RoomItem : UI_Base
{
    public UI_GameRoomGridPanel ParentGrid { get; set; }


    enum RoomInfoImages
    {
        EmptyRoom,
        RoomExist,
        Playing,
        Full,
        Waiting,
    }


    Dictionary<RoomInfoImages, string> SpritePath = new Dictionary<RoomInfoImages, string>()
    {
        { RoomInfoImages.EmptyRoom,  "Textures/GameRoom/Info/EmptyRoom" },
        { RoomInfoImages.RoomExist,  "Textures/GameRoom/Info/ExistringRoom"},
        { RoomInfoImages.Playing,    "Textures/GameRoom/Info/ExistringRoom"},
        { RoomInfoImages.Full,       "Textures/GameRoom/Info/ExistringRoom"},
        { RoomInfoImages.Waiting,    "Textures/GameRoom/Info/ExistringRoom"},
    };


    enum Objects
    {
        
    }
    enum Images
    {
        MapImage,
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


    bool isRoomItemActive { get; set; }


    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(Images));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        // Scale (0, 0) 잡혀있는 것 (1, 1)로 초기화
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(1, 1, rectTransform.localScale.z);

        // 현재 Resolution에 맞게 Item Size 조절하기
        ParentGrid = GetComponentInParent<UI_GameRoomGridPanel>();

        Vector2 cellSize = ParentGrid.GetComponent<GridLayoutGroup>().cellSize;

        RectTransform normalRectTransform = GetObject((int)Images.Normal).GetComponent<RectTransform>();
        normalRectTransform.sizeDelta = new Vector2(cellSize.x, cellSize.y);

        RectTransform HighlightedRectTransform = GetObject((int)Images.Highlighted).GetComponent<RectTransform>();
        HighlightedRectTransform.sizeDelta = new Vector2(cellSize.x, cellSize.y);

        OutlineInitialize();
        SetEmpty();
    }

    public void SetEmpty()
    {
        isRoomItemActive = false;
        GetImage((int)Images.Highlighted).gameObject.SetActive(false);
        GetButton((int)Buttons.RoomStateBtn).GetComponent<Image>().sprite = Resources.Load<Sprite>(SpritePath[RoomInfoImages.EmptyRoom]);

        GetTextMeshPro((int)Texts.RoomName).text = "";
        GetTextMeshPro((int)Texts.RoomNumber).text = "";
        GetTextMeshPro((int)Texts.RoomCapacity).text = "";
        GetImage((int)Images.MapImage).gameObject.SetActive(false);
        GetImage((int)Images.TeamMode).gameObject.SetActive(false);
        GetImage((int)Images.SecretMode).gameObject.SetActive(false);
        GetImage((int)Images.GameState).gameObject.SetActive(false);
    }

    public void SetActive(RoomInfo roominfo)
    {
        // Todo..
        isRoomItemActive = true;

        GetTextMeshPro((int)Texts.RoomNumber).text = roominfo.RoomNumber.ToString();
        GetTextMeshPro((int)Texts.RoomName).text = roominfo.RoomName;

        // 이부분 나중에 Json 으로 변경할지 고려해 봐야함 
        GetImage((int)Images.MapImage).sprite = Resources.Load<Sprite>(roominfo.MapImagePath);
        GetTextMeshPro((int)Texts.RoomCapacity).text = MakeCurPeopleInfo(roominfo.CurPeopleCnt, roominfo.MaxPeopleCnt);
        
        // RoomState
        string stateString = roominfo.RoomState.ToString();
        if (RoomInfoImages.TryParse(stateString, out RoomInfoImages roominfoimage))
        {
            GetImage((int)Images.GameState).sprite = Resources.Load<Sprite>(SpritePath[roominfoimage]);
        }

        // TeamMode
        string TeamModeString = roominfo.TeamMode.ToString();
        if (RoomInfoImages.TryParse(TeamModeString, out RoomInfoImages teammodeinfoimage))
        {
            GetImage((int)Images.TeamMode).sprite = Resources.Load<Sprite>(SpritePath[teammodeinfoimage]);
        }

        GetImage((int)Images.SecretMode).gameObject.SetActive(roominfo.IsSecretRoom);
    }


    private string MakeCurPeopleInfo(int curppl, int maxppl)
    {
        string returnvalue = curppl.ToString() + "/" + maxppl.ToString();
        return returnvalue;
    }


    private void OutlineInitialize()
    {
        EventTrigger trigger = GetImage((int)Images.Normal).gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => {
            Debug.Log("Room Item Is Deactivate State!");
            if (!isRoomItemActive) return;
            GetImage((int)Images.Highlighted).gameObject.SetActive(true);
        });

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => {
            GetImage((int)Images.Highlighted).gameObject.SetActive(false);
        });

        trigger.triggers.Add(entryEnter);
        trigger.triggers.Add(entryExit);
    }
}
