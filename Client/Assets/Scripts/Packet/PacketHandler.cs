using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

class PacketHandler
{
	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
	}

	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGameHandler = packet as S_LeaveGame;
		Managers.Object.Clear();
	}

	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		foreach (ObjectInfo obj in spawnPacket.Objects)
		{
			Managers.Object.Add(obj, myPlayer: false);
		}
	}

	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		foreach (int id in despawnPacket.ObjectIds)
		{
			Managers.Object.Remove(id);
		}
	}

	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;


		// 임시 방편 테스트 
		InGameObject obj =  Managers.InGame._objectLayerManager.FindObjectbyId(movePacket.ObjectId);

		if (obj != null)
		{
			CABaseController controller = obj.UnityObject.GetComponent<CABaseController>();

			if (controller != null)
			{
				if (movePacket.PosInfo.MoveDir == MoveDir.MoveNone)
					Debug.Log("MoveDirNone Detected!!!!!!");

				controller.PosInfo = movePacket.PosInfo;
            }
				 
		}



		// 기존
		//Vector3 lastPos = (Vector3)obj.CurrentPos;


        //Console.Write($"CurPos : {obj.CurrentPos} ");
        //obj.CurrentPos = new Vector3(movePacket.PosInfo.PosX, movePacket.PosInfo.PosY, lastPos.z);
		 

		//GameObject go = Managers.Object.FindById(movePacket.ObjectId);
		//if (go == null)
		//	return;

		//if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
		//	return;

		//BaseController bc = go.GetComponent<BaseController>();
		//if (bc == null)
		//	return;

		//bc.PosInfo = movePacket.PosInfo;
	}

	public static void S_SkillHandler(PacketSession session, IMessage packet)
	{
		S_Skill skillPacket = packet as S_Skill;

		GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc != null)
		{
			cc.UseSkill(skillPacket.Info.SkillId);
		}
	}

	public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
	{
		S_ChangeHp changePacket = packet as S_ChangeHp;

		GameObject go = Managers.Object.FindById(changePacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc != null)
		{
			cc.Hp = changePacket.Hp;
		}
	}

	public static void S_DieHandler(PacketSession session, IMessage packet)
	{
		S_Die diePacket = packet as S_Die;

		GameObject go = Managers.Object.FindById(diePacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc != null)
		{
			cc.Hp = 0;
			cc.OnDead();
		}
	}

	public static void S_ConnectedHandler(PacketSession session, IMessage packet)
	{
		Debug.Log("S_ConnectedHandler");
		C_Login loginPacket = new C_Login();

		string path = Application.dataPath;

		// 이코드가 이상함.
		// 이렇게 하면 같은 컴퓨터면 계정이 달라도
		// UniqueID값이 같은걸로 뽑혀버림!
		// 따라서 플레이어가 ID창에 입력한 ID를 사용하도록 바꿨다. 
		//loginPacket.UniqueId = path.GetHashCode().ToString();

		loginPacket.UniqueId = Managers.Network.ID;
		Managers.Network.Send(loginPacket);
	}

	// 로그인 OK + 캐릭터 목록
	public static void S_LoginHandler(PacketSession session, IMessage packet)
	{
		S_Login loginPacket = (S_Login)packet;
		Debug.Log($"LoginOk({loginPacket.LoginOk})");

		// TODO : 로비 UI에서 캐릭터 보여주고, 선택할 수 있도록 (수업에서 진행x)
		// 기존에 Player가 없었으면 캐릭터를 생성해서 로그인해준다. 
		if (loginPacket.Players == null || loginPacket.Players.Count == 0)
		{
			C_CreatePlayer createPacket = new C_CreatePlayer();
			createPacket.Name = Managers.Network.ID;
			//createPacket.Name = $"Player_{Random.Range(0, 10000).ToString("0000")}";
			Managers.Network.Send(createPacket);
		}
		else
		{
			// 무조건 첫번째 로그인
			LobbyPlayerInfo info = loginPacket.Players[0];
			C_EnterGame enterGamePacket = new C_EnterGame();
			enterGamePacket.Name = info.Name;
			Managers.Network.Send(enterGamePacket);
		}
	}

	public static void S_CreatePlayerHandler(PacketSession session, IMessage packet)
	{
		S_CreatePlayer createOkPacket = (S_CreatePlayer)packet;

		// 수업시간에는 겹치는 ID가 왔을때 Handle하는 코드로 다뤘었는데,
		// 내가 쓰는 코드에서는 이 상황이 발생하지 않는다.
		//if (createOkPacket.Player == null)
		//{
		//	C_CreatePlayer createPacket = new C_CreatePlayer();
		//	createPacket.Name = $"Player_{Random.Range(0, 10000).ToString("0000")}";
		//	Managers.Network.Send(createPacket);
		//}
		//else
		//{
			C_EnterGame enterGamePacket = new C_EnterGame();
			enterGamePacket.Name = createOkPacket.Player.Name;
			Managers.Network.Send(enterGamePacket);
		//}
	}

    public static void S_MyPlayerEnterLobbyHandler(PacketSession session, IMessage packet)
    {
		S_MyPlayerEnterLobby MyPlayerInfo = (S_MyPlayerEnterLobby)packet;

		Managers.UserInfo.Add(MyPlayerInfo.Player, true);
		//Debug.Log("MyPlayer Added");
		//Debug.Log($"PlayerDB ID : {MyPlayerInfo.Player.PlayerDbId} !!");
		//Debug.Log($"PlayerName : {MyPlayerInfo.Player.Name}");  // Name에 로그인할때 입력한 ID가 들어오네

    }

    public static void S_OtherPlayerEnterLobbyHandler(PacketSession session, IMessage packet)
    {
		S_OtherPlayerEnterLobby OtherPlayerInfo = (S_OtherPlayerEnterLobby)packet;

		foreach (LobbyPlayerInfo p in OtherPlayerInfo.Otherplayers)
			Managers.UserInfo.Add(p);

		Debug.Log("OtherPlayers Added");
    }

    public static void S_PlayerLeaveLobbyHandler(PacketSession session, IMessage packet)
	{
		S_PlayerLeaveLobby LeavePlayerInfo = (S_PlayerLeaveLobby)packet;

		Managers.UserInfo.Remove(LeavePlayerInfo.Player);

		Debug.Log("Player Leaved");
	}

    public static void S_ItemListHandler(PacketSession session, IMessage packet)
	{
		S_ItemList itemList = (S_ItemList)packet;

		Managers.Inven.Clear();

		// 메모리에 아이템 정보 적용
		foreach (ItemInfo itemInfo in itemList.Items)
		{
			Item item = Item.MakeItem(itemInfo);
			Managers.Inven.Add(item);
		}

		if (Managers.Object.MyPlayer != null)
			Managers.Object.MyPlayer.RefreshAdditionalStat();
	}

	public static void S_AddItemHandler(PacketSession session, IMessage packet)
	{
		S_AddItem itemList = (S_AddItem)packet;

		// 메모리에 아이템 정보 적용
		foreach (ItemInfo itemInfo in itemList.Items)
		{
			Item item = Item.MakeItem(itemInfo);
			Managers.Inven.Add(item);
		}

		Debug.Log("아이템을 획득했습니다!");

		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		gameSceneUI.InvenUI.RefreshUI();
		gameSceneUI.StatUI.RefreshUI();

		if (Managers.Object.MyPlayer != null)
			Managers.Object.MyPlayer.RefreshAdditionalStat();
	}

	public static void S_EquipItemHandler(PacketSession session, IMessage packet)
	{
		S_EquipItem equipItemOk = (S_EquipItem)packet;

		// 메모리에 아이템 정보 적용
		Item item = Managers.Inven.Get(equipItemOk.ItemDbId);
		if (item == null)
			return;

		item.Equipped = equipItemOk.Equipped;
		Debug.Log("아이템 착용 변경!");

		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		gameSceneUI.InvenUI.RefreshUI();
		gameSceneUI.StatUI.RefreshUI();

		if (Managers.Object.MyPlayer != null)
			Managers.Object.MyPlayer.RefreshAdditionalStat();
	}

	public static void S_ChangeStatHandler(PacketSession session, IMessage packet)
	{
		S_ChangeStat itemList = (S_ChangeStat)packet;

		// TODO
	}

	public static void S_PingHandler(PacketSession session, IMessage packet)
	{
		C_Pong pongPacket = new C_Pong();
		Debug.Log("[Server] PingCheck");
		Managers.Network.Send(pongPacket);
	}

	public static void S_ChattingHandler(PacketSession session, IMessage packet)
	{
		Debug.Log("Chatting received");

		S_Chatting chatPacket = (S_Chatting)packet;

		Debug.Log(SceneManager.GetActiveScene().name);

		if (SceneManager.GetActiveScene().name == "CAMainLobby")
		{
			CAMainLobby mainLobby = GameObject.FindObjectOfType<CAMainLobby>();
			if (mainLobby != null)
			{
				mainLobby.HandleReceivedChatMessage(chatPacket); 
			}
		}
	}

	public static void S_AlterRoomHandler(PacketSession session, IMessage packet)
	{
		Debug.Log("Alter Room Packet Received!");

		if (SceneManager.GetActiveScene().name != "CAMainLobby")
		{
			Debug.Log("Current Scne is not Lobby Scene!"); // LobbyScene일때만 이 패킷이 도착애햐암
			return;
		}

		CAMainLobby mainLobby = GameObject.FindObjectOfType<CAMainLobby>();
		mainLobby.HandleAlterRoom((S_AlterRoom)packet);
	}

    public static void S_AddMoneyHandler(PacketSession session, IMessage packet)
	{
		Debug.Log("AddMoney Packet Received!");

		S_AddMoney AddMoneyPacket = (S_AddMoney)packet;

		// 만약에 현재 Scene 이 상점인 경우 상점 돈 UI에 추가해야 한다.
		// 플레이어의 돈을 다른 곳에서 관리하고 상점에서는 업데이트만 하도록 하자. 

		//SceneManager.GetActiveScene();
		string ActiveSceneName = SceneManager.GetActiveScene().name;

		Debug.Log($"ActiveSceneName : {ActiveSceneName}");

		if (ActiveSceneName == "CAStore")
		{
			// CAStoreScene에서 Script가져와야 함. 
			CAStoreScene storeSceneScript = GameObject.FindObjectOfType<CAStoreScene>();

			if (storeSceneScript == null)
			{
				Debug.Log("Store Scene Script is Missing!");
			}

			else
			{
				storeSceneScript.sceneUI.UpdateMoneyUI();
			}
		}
	}

	public static void S_JoinRoomHandler(PacketSession session, IMessage packet)
	{
		Debug.Log("Join Room Packet Received!");
		Managers.Room.HandleJoinRoom((S_JoinRoom)packet);
	}

	public static void S_JoinRoomBroadcastHandler(PacketSession session, IMessage packet)
	{
		Managers.Room.HandleJoinRoomBroadcast((S_JoinRoomBroadcast)packet);
	}

	public static void S_ExitRoomBroadcastHandler(PacketSession session, IMessage packet)
	{
		Managers.Room.HandleExitRoomBroadcast((S_ExitRoomBroadcast)packet);
	}

	public static void S_GameroomCharStateHandler(PacketSession session, IMessage packet)
	{
		Managers.Room.HandleGameroomCharState((S_GameroomCharState)packet);
	}

	public static void S_AlterHostHandler(PacketSession session, IMessage packet)
	{
		Managers.Room.HandleAlterHost((S_AlterHost)packet);
	}

	public static void S_ChangeSceneHandler(PacketSession session, IMessage packet)
	{
		Managers.Scene.LoadScene(((S_ChangeScene)packet).Scene);
    }

	public static void S_ChangeSlotStateBroadcastHandler(PacketSession session, IMessage packet)
	{
		Managers.Room.HandleChangeSlotState((S_ChangeSlotStateBroadcast)packet);

    }

	public static void S_CharacterSelectResponseHandler(PacketSession session, IMessage packet)
	{
		Managers.Room.HandleCharacterSelectResponse((S_CharacterSelectResponse)packet);
	}

	public static void S_CharacterSelectBroadcastHandler(PacketSession session, IMessage packet)
	{
		Managers.Room.HandleCharacterSelectBroadcast((S_CharacterSelectBroadcast)packet);
	}

    public static void S_StartGameResHandler(PacketSession session, IMessage packet)
    {
        Managers.Room.HandleStartGameRes((S_StartGameRes)packet);
    }

    public static void S_StartGameBroadcastHandler(PacketSession session, IMessage packet)
    {
        //Managers.Room.HandleStartGameBroadcast((S_StartGameBroadcast)packet);
		Managers.InGame.HandleStartGameBroadcast((S_StartGameBroadcast)packet);
    }

	public static void S_MapSelectBroadcastHandler(PacketSession session, IMessage packet)
	{
		Managers.Room.HandleMapSelectBroadcast((S_MapSelectBroadcast)packet);
	}

	public static void S_SpawnObjectHandler(PacketSession session, IMessage packet)
	{
		Debug.Log("Spawn Object Received!!");
		Managers.InGame.HandleSpawnObject((S_SpawnObject)packet);
		return;
	}

	public static void S_OwnPlayerInformHandler(PacketSession session, IMessage packet)
	{
		S_OwnPlayerInform pkt = (S_OwnPlayerInform)packet;
		InGameObject obj =  Managers.InGame._objectLayerManager.FindObjectbyId(pkt.Objid, pkt.Layerinfo);
		var controller = obj.UnityObject.AddComponent<CAMyPlayerController>();
		controller.InGameObj = obj;
		controller._animator = obj.UnityObject.GetComponentInChildren<Animator>();
		controller._spriteRenderer = controller._animator.GetComponent<SpriteRenderer>();
		controller._charType = pkt.Chartype;
		controller.Test();
	}
    public static void S_NotOwnPlayerInformHandler(PacketSession session, IMessage packet)
    {
        S_NotOwnPlayerInform pkt = (S_NotOwnPlayerInform)packet;
        InGameObject obj = Managers.InGame._objectLayerManager.FindObjectbyId(pkt.Objid, pkt.Layerinfo);
        var controller = obj.UnityObject.AddComponent<CAPlayerController>();
        controller.InGameObj = obj;
		controller._animator = obj.UnityObject.GetComponentInChildren<Animator>(true);
        controller._spriteRenderer = controller._animator.GetComponent<SpriteRenderer>();
        controller._charType = pkt.Chartype;
        controller.Test();
    }
}


