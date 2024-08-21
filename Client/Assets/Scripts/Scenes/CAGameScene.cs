using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class CAGameScene : BaseScene
{

    [SerializeField] InGameCamera InGameCameraPrefab;
    public InGameCamera InGameCamera { get; private set; }

    public UI_CAGameScene _sceneUI { get; set; }
   
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.CAGameScene;

        InGameCamera = Instantiate(InGameCameraPrefab);
        InGameCamera.gameObject.SetActive(true);

        _sceneUI = Managers.UI.ShowSceneUI<UI_CAGameScene>();

        

        Managers.InGame.SetCurrentGameRoomScene(this);
    }

    public override void Clear()
    {

    }

    
}
