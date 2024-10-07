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

    [SerializeField] private Image RoomState;


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
        { RoomInfoImages.Playing,    "Textures/GameRoom/Info/Playing_transparent"},
        { RoomInfoImages.Full,       "Textures/GameRoom/Info/Full_transparent"},
        { RoomInfoImages.Waiting,    "Textures/GameRoom/Info/Waiting_transparent"},
    };

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
    RoomInfo roomInfo { get; set; }


    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        // Scale (0, 0) �����ִ� �� (1, 1)�� �ʱ�ȭ
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(1, 1, rectTransform.localScale.z);

        // ���� Resolution�� �°� Item Size �����ϱ�
        ParentGrid = GetComponentInParent<UI_GameRoomGridPanel>();

        Vector2 cellSize = ParentGrid.GetComponent<GridLayoutGroup>().cellSize;

        RectTransform normalRectTransform = GetImage((int)Images.Normal).GetComponent<RectTransform>();
        normalRectTransform.sizeDelta = new Vector2(cellSize.x, cellSize.y);

        RectTransform HighlightedRectTransform = GetImage((int)Images.Highlighted).GetComponent<RectTransform>();
        HighlightedRectTransform.sizeDelta = new Vector2(cellSize.x, cellSize.y);


        // Click �� �ƴ϶� ����Ŭ���� �̺�Ʈ �߻��ϵ��� �ٲ� ���� 
        GetImage((int)Images.Normal).gameObject.BindEvent(EnterRoom);


        OutlineInitialize();
        SetEmpty();
    }

    public void EnterRoom(PointerEventData evt)
    {
        if (isRoomItemActive)
        {
            Debug.Log($"Entering Room ... Room Number : {roomInfo.RoomNumber}");

            C_JoinRoom joinPacket = new C_JoinRoom();
            joinPacket.Roomid = roomInfo.RoomNumber;
            Managers.Network.Send(joinPacket);
        }
          
    }

    public void OnDoubleClickTest(PointerEventData evt)
    {
        if (isRoomItemActive)
            Debug.Log("On Double Click Test Function Called!!!!!!!!!!!");
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

    public void SetRoomInfo(RoomInfo _roominfo)
    {
        isRoomItemActive = true;
        roomInfo = _roominfo;
   
        UpdateRoomInfoUI();
    }

    public void UpdateRoomInfoUI()
    {
        GetTextMeshPro((int)Texts.RoomNumber).text = roomInfo.RoomNumber.ToString();
        GetTextMeshPro((int)Texts.RoomName).text = roomInfo.RoomName;

        // ��ģ�ڵ�.
        GetImage((int)Images.GameState).gameObject.SetActive(true);
        GetImage((int)Images.MapImage).gameObject.SetActive(true);

        // MapImage ���ľ���.
        // �̺κ� ���߿� Json ���� �������� ����� ������ 
        //var LoadedImage = Resources.Load<Sprite>(roomInfo.MapImagePath);
        //if (LoadedImage != null)
        //{
        //    GetImage((int)Images.MapImage).sprite = LoadedImage;
        //}

        Sprite mapImage = Managers.Data.MapPreviewImageDict[roomInfo.MapType];
        GetImage((int)Images.MapImage).sprite = mapImage;


        GetTextMeshPro((int)Texts.RoomCapacity).text = MakeCurPeopleInfo(roomInfo.CurPeopleCnt, roomInfo.MaxPeopleCnt);

        // RoomState
        string stateString = roomInfo.RoomState.ToString();
        if (RoomInfoImages.TryParse(stateString, out RoomInfoImages roominfoimage))
        {
            Sprite roomstatesprite = Resources.Load<Sprite>(SpritePath[roominfoimage]);

            if (roomstatesprite == null)
            {
                Debug.LogError("Cannot Find Room State Image!");
            }

            else 
            GetImage((int)Images.GameState).sprite = Resources.Load<Sprite>(SpritePath[roominfoimage]);
        }

        // TeamMode
        string TeamModeString = roomInfo.TeamMode.ToString();
        if (RoomInfoImages.TryParse(TeamModeString, out RoomInfoImages teammodeinfoimage))
        {
            GetImage((int)Images.TeamMode).sprite = Resources.Load<Sprite>(SpritePath[teammodeinfoimage]);
        }

        GetImage((int)Images.SecretMode).gameObject.SetActive(roomInfo.IsSecretRoom);
    }

    private string MakeCurPeopleInfo(int curppl, int maxppl)
    {
        return $"{curppl}/{maxppl}";
    }

    public void SetRoomStateImage(RoomStateType roomstate)
    {
        if (RoomInfoImages.TryParse(roomstate.ToString(), out RoomInfoImages roomStateImage))
        {
           RoomState.sprite = Resources.Load<Sprite>(SpritePath[roomStateImage]);
        }
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
