using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CAMapSelect : UI_Popup
{
    enum Buttons
    {
        Cancel,
        Confirm,
    }

    private UI_RaycastBlock BlockPopup;

    [SerializeField] private List<UI_MapPreviewItem> MapPreviewItems;

    public MapType CurrentlySelectedMap;


    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        //Bind<GameObject>(typeof(GameObjects));
        //Bind<GridLayoutGroup>(typeof(GridPanels));


        BlockPopup = Managers.UI.ShowPopupUI<UI_RaycastBlock>();
        Managers.UI.SetCanvas(gameObject, true);

       // GetButton((int)Buttons.Confirm).gameObject.BindEvent(SelectMap);
        GetButton((int)Buttons.Cancel).gameObject.BindEvent(ClosePopup);
        GetButton((int)Buttons.Confirm).gameObject.BindEvent(MapChange);

        for (int i = 0; i < MapPreviewItems.Count; i++)
        {
            Sprite MapImage = Managers.Data.MapPreviewImageDict[MapPreviewItems[i].GetMap()];
            MapPreviewItems[i].SetMapImage(MapImage);
            MapPreviewItems[i].ParentObj = this;
        }

        // ó����  Select�� Map���� �ѹ� SelectMap�Լ� ȣ������� ��. 
        SelectMap(Managers.Room.SelectedMap);

    }

    public void MapChange(PointerEventData evt)
    {
        // Server���ٰ� ������ ������ �����ϵ��� ��û�� �ؾ� �Ѵ�. 
        C_MapSelect MapSelectPacket = new C_MapSelect();
        MapSelectPacket.Maptype = CurrentlySelectedMap;

        Managers.Network.Send(MapSelectPacket);

        ClosePopupUI();

        if (BlockPopup != null)
            BlockPopup.ClosePopupUI();

    }

    public void ClosePopup(PointerEventData evt)
    {
        ClosePopupUI();

        if (BlockPopup != null)
            BlockPopup.ClosePopupUI();
    }

    public void SelectMap(MapType type)
    {
        CurrentlySelectedMap = type;

        for (int i = 0; i < MapPreviewItems.Count; ++i)
        {
            if (MapPreviewItems[i].GetMap() == type)
            {
                MapPreviewItems[i].IsImageSelected(true);
            }

            else
            {
                MapPreviewItems[i].IsImageSelected(false);
            }
        }
    }
}
