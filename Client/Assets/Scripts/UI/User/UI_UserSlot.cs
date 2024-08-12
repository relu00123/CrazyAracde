using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 



enum RawImages
{
    Character,
}

enum Images
{
    UserSlotBG,
}


public class UI_UserSlot : UI_Base
{
    //public RectTransform slot; // 슬롯의 RectTransform

    [SerializeField] Texture2D AnimBaseTexture;

    //[SerializeField] private RectTransform characterImage;


    [SerializeField] private Transform CharacterObjectImage;
    // public RectTransform characterImage; // 캐릭터 이미지의 RectTransform
    // public SpriteRenderer characterSpriteRenderer; // 캐릭터 스프라이트 렌더러


    //[SerializeField] private TextMeshProUGUI Itemname;
    //[SerializeField] private TextMeshProUGUI ItemPrice;
    //[SerializeField] private Image ItemImage;
    //[SerializeField] private Button BuyButton;

    [SerializeField] private Sprite NormalImage;
    [SerializeField] private Sprite HighlightedImage;

    [SerializeField] private Sprite OpenSlotTexture;
    [SerializeField] private Sprite CloseSlotTexture;

    private int slotIdx = -1;


    public override void Init()
    {
        Bind<RawImage>(typeof(RawImages));
        Bind<Image>(typeof(Images));

        AdjustCharacterSizeAndPosition();
        SetupHoverEvents();
       
    }

    public void SetupHoverEvents()
    {
        // 자신이 Host일때만 Highlight -> Normal을 변경할 수 있어야 한다. 

        EventTrigger trigger = GetComponent<Image>().gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => {
            if (Managers.Room.host == true)
                GetComponent<Image>().sprite = HighlightedImage;
        });

        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => {
            GetComponent<Image>().sprite = NormalImage;
        });

        trigger.triggers.Add(entryExit);


        EventTrigger.Entry entryclick = new EventTrigger.Entry();
        entryclick.eventID = EventTriggerType.PointerClick;
        entryclick.callback.AddListener((data) =>
        {
            if (Managers.Room.host == true)
            {
                Debug.Log($"Kick Player!!!! {slotIdx}");
                C_KickPlayer kickpacket = new C_KickPlayer();
                kickpacket.Slotidx = slotIdx;
                Managers.Network.Send(kickpacket);
            }
        });

        trigger.triggers.Add((entryclick));
    }

 


    private void AdjustCharacterSizeAndPosition()
    {
        GetRawImage((int)RawImages.Character).rectTransform.anchorMin = new Vector2(0, 0);
        GetRawImage((int)RawImages.Character).rectTransform.anchorMax = new Vector2(0, 0);
        GetRawImage((int)RawImages.Character).rectTransform.pivot = new Vector2(0.20f, -0.05f);
        GetRawImage((int)RawImages.Character).rectTransform.sizeDelta =
            new Vector2(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.width);
    }

    public void AdjustCharacterUV(int num)
    {
        // 크아 게임은 종횡비가 4: 3이다.
        float cameraWidth = 8f;
        float cameraHeight = 6f;

        // 이는 UI Camera의 Size로 인해서 결정. Size는 하드코딩되어 있음.
        float unitWidth = 2f;
        float unitHeight = 2f;

        // 각 캐릭터의 UV 크기
        float scale_x = unitWidth / cameraWidth;
        float scale_y = unitHeight / cameraHeight;

        float start_x = (num % 4) * scale_x;
        float start_y = (num < 4) ? 0.5f : 0.0f;

        GetRawImage((int)RawImages.Character).uvRect = new Rect(start_x, start_y, scale_x, scale_y);
    }

    public void SetSlotIndex(int idx)
    {
        slotIdx = idx;
    }

    public void CloseSlot()
    {
        GetImage((int)Images.UserSlotBG).sprite = CloseSlotTexture;
    }

    public void OpenSlot()
    {
        GetImage((int)Images.UserSlotBG).sprite = OpenSlotTexture;
    }
}
