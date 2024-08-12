using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


	public void LoadScene(Define.Scene type)
    {
        Managers.Clear();

        SceneManager.LoadScene(GetSceneName(type));
    }

    public void LoadScene(GameSceneType type)
    {
        Managers.Clear();

        string SceneTypeAsString = type.ToString();

        SceneManager.LoadScene(SceneTypeAsString);
    }

    public void OnSceneLoaded(Scene scene,  LoadSceneMode mode)
    {
        if (Enum.TryParse(scene.name, out GameSceneType sceneType))
        {
            C_ChangeSceneCompleted packet = new C_ChangeSceneCompleted();
            packet.Scene = sceneType;
            Managers.Network.Send(packet);
        }
    }


    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
