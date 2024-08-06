using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    
    public void HandleJoinRoom(S_JoinRoom joinRoomPacket)
    {
        switch (joinRoomPacket.Joinresult)
        {
            case JoinResultType.Success:

                SceneManager.sceneLoaded += Test;

                Managers.Scene.LoadScene(Define.Scene.CAGameRoom);

                break;

            case JoinResultType.RoomNotExist:

                break;

            case JoinResultType.GameAlreadyStarted:

                break;

            case JoinResultType.RoomFull:

                break;
        } 
    }

    public void Test(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Hello");
        SceneManager.sceneLoaded -= Test;
    }
}


 