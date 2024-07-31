using Google.Protobuf.Protocol;
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


    // Room들을 관리하고 있을 자료구조 
    private Dictionary<int, RoomInfo> Rooms = new Dictionary<int, RoomInfo>();
    private List<int> OrderedRoomNumber = new List<int>();

    private int curPage = 1;
    private const int roomsPerPage = 8;


    private GridLayoutGroup gridLayoutGroup   { get; set; }
    private List<UI_RoomItem> ShownRoomList = new List<UI_RoomItem>();

    
    public override void Init()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        RectTransform rectTransform = GetComponent<RectTransform>();

        float screenWidth = CanvasRect.rect.width;
        float screenHeight = CanvasRect.rect.height;

        //Debug.Log($"Screen Width :  {screenWidth}");
        //Debug.Log($"Screen Height :  {screenHeight}");

        Vector2 Diff = rectTransform.anchorMax - rectTransform.anchorMin;

        float CellWidth = rectTransform.rect.width / 2;
        float CellHeight = rectTransform.rect.height / 4;

        CellWidth = screenWidth * Diff.x / 2;
        CellHeight = screenHeight * Diff.y / 4;

        gridLayoutGroup.cellSize = new Vector2(CellWidth, CellHeight);

        for (int i = 0; i < roomsPerPage; ++i)
        {
            UI_RoomItem item = Instantiate(roomInfo_UI, gridLayoutGroup.transform);
            ShownRoomList.Add(item);
        }

        RoomInfo roominfo = new RoomInfo();
        roominfo.RoomNumber = 10;
        roominfo.RoomName = "TestRoom1";
        roominfo.CurPeopleCnt = 2;
        roominfo.MaxPeopleCnt = 6;
        roominfo.RoomState = RoomStateType.Waiting;
        roominfo.TeamMode = TeamModeType.MannerMode;
        roominfo.GameMode = GameModeType.NormalMode;
        roominfo.IsSecretRoom = true;

        //AddRoom(1, roominfo);
        //AddRoom(2, roominfo);
        //AddRoom(3, roominfo);
        //AddRoom(4, roominfo);
        //AddRoom(5, roominfo);
        //AddRoom(6, roominfo);
        //AddRoom(7, roominfo);
        //AddRoom(8, roominfo);
        //AddRoom(9, roominfo);
        //AddRoom(10, roominfo);
        //AddRoom(11, roominfo);
        //AddRoom(12, roominfo);
        //AddRoom(13, roominfo);
        //AddRoom(14, roominfo);
        //AddRoom(15, roominfo);
        //AddRoom(16, roominfo);
        //AddRoom(17, roominfo);
        //AddRoom(18, roominfo);
        //AddRoom(19, roominfo);
        //AddRoom(20, roominfo);


    }

    public void AddRoom(RoomInfo roomInfo)
    {
        int TargetRoomNumber = roomInfo.RoomNumber;

        if (!Rooms.ContainsKey(TargetRoomNumber))
        {
            Rooms[TargetRoomNumber] = roomInfo;
            OrderedRoomNumber.Add(TargetRoomNumber);
            OrderedRoomNumber.Sort();
            UpdateShowingRoomsUI();
        }
    }


    public void ChangeRoomInfo(RoomInfo roomInfo)
    {
        int TargetRoomNumber = roomInfo.RoomNumber;

        if (Rooms.ContainsKey(TargetRoomNumber))
        {
            Rooms[TargetRoomNumber] = roomInfo;
            UpdateShowingRoomsUI();
        }
    }


    public void RemoveRoom(int roomNumber)
    {
        if (Rooms.ContainsKey(roomNumber))
        {
            Rooms.Remove(roomNumber);
            OrderedRoomNumber.Remove(roomNumber);
            UpdateShowingRoomsUI();
        }
    }

    public List<RoomInfo> GetRoomsForPage(int pageNumber, int roomsPerPage) 
    {
        // pageNumber 는 1부터 시작한다고 가정 
        List<RoomInfo> roomsForPage = new List<RoomInfo>();

        int startIndex = (pageNumber - 1) * roomsPerPage;
        int endIndex = startIndex + roomsPerPage;

        for (int i = startIndex; i < endIndex && i < OrderedRoomNumber.Count; ++i)
        {
            int roomNumber = OrderedRoomNumber[i];
            roomsForPage.Add(Rooms[roomNumber]);
        }

        return roomsForPage;
    }

    private void UpdateShowingRoomsUI()
    {
        List<RoomInfo> SelectedRooms = GetRoomsForPage(curPage, roomsPerPage);

        for (int i = 0; i < roomsPerPage; i++)
        {
            if (i < SelectedRooms.Count)
                ShownRoomList[i].SetRoomInfo(SelectedRooms[i]);
            else
                ShownRoomList[i].SetEmpty();
        }
    }

    #region ButtonCallBackFunctions
    public void PreviousRoomsBtnClicked()
    {
        Debug.Log("PreviousRoomsBtnClicked Called!");

        if (curPage > 1)
        {
            curPage--;
            UpdateShowingRoomsUI();
        }

    }

    public void NextRoomsBtnClicked()
    {
        Debug.Log("NextRoomsBtnClicked Called!");

        int totalPages = Mathf.CeilToInt(OrderedRoomNumber.Count / (float)roomsPerPage);

        if (curPage < totalPages)
        {
            curPage++;
            UpdateShowingRoomsUI();
        }
    }

    #endregion

}
