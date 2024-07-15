using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UserInfoItem : UI_Base
{
    string LevelImagePath = "Textures/Level/";

    enum Texts
    {
        UserName
    }

    enum Images
    {
        UserLevel
    }
    

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
    }

    public void SetLobbyPlayerInfo(LobbyPlayerInfo info)
    {
         
        GetTextMeshPro((int)Texts.UserName).text = info.Name;


        string FullLevelImagePath = LevelImagePath + info.LevelInfo.Level.ToString();
        GetImage((int)Images.UserLevel).sprite = Resources.Load<Sprite>(FullLevelImagePath); 
    }
}
