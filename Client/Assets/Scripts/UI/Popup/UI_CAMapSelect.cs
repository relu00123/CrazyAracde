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

    enum GameObjects
    {
        Maps,
    }

    enum GridPanels
    {
        Maps,
    }


    private UI_RaycastBlock BlockPopup;

    [SerializeField] private GameObject MapPreviewItem;  

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<GridLayoutGroup>(typeof(GridPanels));


        BlockPopup = Managers.UI.ShowPopupUI<UI_RaycastBlock>();
        Managers.UI.SetCanvas(gameObject, true);

        GetButton((int)Buttons.Confirm).gameObject.BindEvent(SelectMap);
        GetButton((int)Buttons.Cancel).gameObject.BindEvent(ClosePopup);


        Sprite[] mapImages = Resources.LoadAll<Sprite>("Textures/GameRoom/MapPreview");

        GridLayoutGroup gridtest = GetGridPanel((int)GridPanels.Maps);
         

        foreach (Sprite mapImage in mapImages)
        {
            GameObject newPreviewItem = Instantiate(MapPreviewItem, GetGridPanel((int)GridPanels.Maps).gameObject.transform);
            Image ItemImage = newPreviewItem.GetComponent<Image>();

            if (ItemImage != null )
            {
                ItemImage.sprite = mapImage;
                newPreviewItem.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public void ClosePopup(PointerEventData evt)
    {
        ClosePopupUI();

        if (BlockPopup != null)
            BlockPopup.ClosePopupUI();
    }

    public void SelectMap(PointerEventData evt)
    {
        // Scene의 UI에 해당 맵을 저장해 놓자.
        // GameStart할때 저장한 맵을 생성해달라고 요청할 것임. 
    }
}
