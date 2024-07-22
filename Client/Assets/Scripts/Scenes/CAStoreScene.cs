using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAStoreScene : BaseScene
{
    public UI_CAStoreScene sceneUI { get; set; }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.CAStore;

        sceneUI = Managers.UI.ShowSceneUI<UI_CAStoreScene>();
    }

    public override void Clear()
    {
    }
}
