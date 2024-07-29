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
    }

    public void AddRoom(int roomNumber, RoomInfo roomInfo)
    {
        if (!Rooms.ContainsKey(roomNumber))
        {
            Rooms[roomNumber] = roomInfo;
            OrderedRoomNumber.Add(roomNumber);
            OrderedRoomNumber.Sort();
            UpdateRoomUI();
        }
    }


    public void ChangeRoomInfo(int roomNumber, RoomInfo roomInfo)
    {
        if (Rooms.ContainsKey(roomNumber))
        {
            Rooms[roomNumber] = roomInfo;
            UpdateRoomUI();
        }
    }


    public void RemoveRoom(int roomNumber)
    {
        if (Rooms.ContainsKey(roomNumber))
        {
            Rooms.Remove(roomNumber);
            OrderedRoomNumber.Remove(roomNumber);
            UpdateRoomUI();
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

    public void PreviousRoomsBtnClicked()
    {
        Debug.Log("PreviousRoomsBtnClicked Called!");

        if (curPage > 1)
        {
            curPage--;
            UpdateRoomUI();
        }

    }

    public void NextRoomsBtnClicked()
    {
        Debug.Log("NextRoomsBtnClicked Called!");

        int totalPages = Mathf.CeilToInt(OrderedRoomNumber.Count / (float)roomsPerPage);

        if (curPage < totalPages)
        {
            curPage++;
            UpdateRoomUI();
        }
    }

    private void UpdateRoomUI()
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
}
