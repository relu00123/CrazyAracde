using Google.Protobuf.Protocol;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MapPreviewItem : UI_Base
{
    [SerializeField] private Image MapImage;
    [SerializeField] private Image HighlightEffect;

    [SerializeField] private MapType NormalMapType;
    [SerializeField] private MapType MonsterMapType;
    public UI_CAMapSelect ParentObj { get; set; }


    public override void Init()
    {
        HighlightEffect.gameObject.SetActive(false);

        MapImage.gameObject.BindEvent(OnMapImageClick);
    }

    public void IsImageSelected(bool state)
    {
        if (state)
            HighlightEffect.gameObject.SetActive(true);
        else
            HighlightEffect.gameObject.SetActive(false);
    }

    public void SetMapImage(Sprite sprite)
    {
        // Image testImg = GetImage((int)Images.MapImage);
        // GetImage((int)Images.MapImage).sprite = sprite;
        MapImage.sprite = sprite;
    }


    public MapType GetMap()
    {
        if (Managers.Room.GameMode == GameModeType.MonsterMode)
            return MonsterMapType;
        else if (Managers.Room.GameMode == GameModeType.NormalMode)
            return NormalMapType;
        else
            return NormalMapType;
    }

    public void OnMapImageClick(PointerEventData evt)
    {
        if (ParentObj != null)
        {
            ParentObj.SelectMap(GetMap());
        }
    }
}
