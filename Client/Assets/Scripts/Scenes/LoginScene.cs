using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginScene : BaseScene
{
    UI_LoginScene _sceneUI;

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Login;

        Managers.Web.BaseUrl = "http://192.168.123.103:5000/api";
        // "http://192.168.219.109:5000/api";  //"https://localhost:5001/api";

        Screen.SetResolution(640, 480, false);

        _sceneUI = Managers.UI.ShowSceneUI<UI_LoginScene>();
    }

    public override void Clear()
    {
        
    }
}
