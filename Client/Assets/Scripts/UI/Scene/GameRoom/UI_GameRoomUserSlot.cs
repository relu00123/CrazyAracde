using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameRoomUserSlot : UI_Base
{
    [SerializeField] public Sprite HostStateTexture;
    [SerializeField] public Sprite ReadyStateTexture;
    [SerializeField] public Sprite NotReadyStateTexture;
    [SerializeField] public Sprite StoreStateTexture;


    enum Images
    {
        SlotBackGround,
        CharacterState,
    }

    enum Texts
    {
        CharacterName
    }

    private int slotidx = -1;


    public UI_UsersGridPanel ParentGrid { get; set; }


    [SerializeField] private GameObject userSlotBG;
    public UI_UserSlot uiUserSlot { get; private set; }


    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        // Scale (0, 0) �����ִ� �� (1, 1)�� �ʱ�ȭ
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(1, 1, rectTransform.localScale.z);

        // ���� Resolution�� �°� Item Size �����ϱ�
        ParentGrid = GetComponentInParent<UI_UsersGridPanel>();

        Vector2 cellSize = ParentGrid.GetComponent<GridLayoutGroup>().cellSize;

        RectTransform SlotBGTransform = GetImage((int)Images.SlotBackGround).GetComponent<RectTransform>();
        SlotBGTransform.sizeDelta = new Vector2(cellSize.x, cellSize.y);


        uiUserSlot = userSlotBG.GetComponent<UI_UserSlot>();  
    }

    public void SetName(string name)
    {
        GetTextMeshPro((int)Texts.CharacterName).text = name;
    }

    public void ClearSlot()
    {
        // ToDo..
        // ���ȭ�� �ʱ�ȭ 
        // ���� �ʱ�ȭ

        // �̸� �ʱ�ȭ 
        GetTextMeshPro((int)Texts.CharacterName).text = "";
    }

    public void SetCharState(GameRoomCharacterStateType state)
    {
        switch (state)
        {
            case GameRoomCharacterStateType.Host:
                GetImage((int)Images.CharacterState).sprite = HostStateTexture;
                break;
            case GameRoomCharacterStateType.Ready:
                GetImage((int)Images.CharacterState).sprite = ReadyStateTexture;
                break;
            case GameRoomCharacterStateType.NotReady:
                GetImage((int)Images.CharacterState).sprite = NotReadyStateTexture;
                break;
            case GameRoomCharacterStateType.Store:
                GetImage((int)Images.CharacterState).sprite = StoreStateTexture;
                break;
        }
    }

    public void SetSlotIndex(int idx)
    {
        slotidx = idx;

        if (uiUserSlot != null)
            uiUserSlot.SetSlotIndex(idx);
    }

}
