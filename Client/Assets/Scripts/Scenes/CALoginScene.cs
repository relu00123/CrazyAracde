using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CALoginScene : BaseScene
{
    UI_CALoginScene _sceneUI;

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Login;

        Managers.Web.BaseUrl = "https://localhost:5001/api";

        Screen.SetResolution(640, 480, false);

        _sceneUI = Managers.UI.ShowSceneUI<UI_CALoginScene>();
    }

    public override void Clear()
    {

    }
}
