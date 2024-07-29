﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.DB;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

class PacketHandler
{
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;

		//Console.WriteLine($"C_Move ({movePacket.PosInfo.PosX}, {movePacket.PosInfo.PosY})");

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleMove, player, movePacket);
	}

	public static void C_SkillHandler(PacketSession session, IMessage packet)
	{
		C_Skill skillPacket = packet as C_Skill;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleSkill, player, skillPacket);
	}

	public static void C_CreateRoomHandler(PacketSession session, IMessage packet)
	{
		C_CreateRoom createroomPacket = packet as C_CreateRoom;
		ClientSession clientSession = session as ClientSession;

		// Game Room 생성해주고 Player State 바꿔주는 등의 행동을 해야한다. 
		Console.WriteLine("Create Room Handler Called From Server");

		return;
	}



    public static void C_LoginHandler(PacketSession session, IMessage packet)
	{
		C_Login loginPacket = packet as C_Login;
		ClientSession clientSession = session as ClientSession;
		clientSession.HandleLogin(loginPacket);
	}

	public static void C_EnterGameHandler(PacketSession session, IMessage packet)
	{
		C_EnterGame enterGamePacket = (C_EnterGame)packet;
		ClientSession clientSession = (ClientSession)session;
		clientSession.HandleEnterGame(enterGamePacket);
	}

	public static void C_CreatePlayerHandler(PacketSession session, IMessage packet)
	{
		C_CreatePlayer createPlayerPacket = (C_CreatePlayer)packet;
		ClientSession clientSession = (ClientSession)session;
		clientSession.HandleCreatePlayer(createPlayerPacket);
	}

	public static void C_EquipItemHandler(PacketSession session, IMessage packet)
	{
		C_EquipItem equipPacket = (C_EquipItem)packet;
		ClientSession clientSession = (ClientSession)session;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleEquipItem, player, equipPacket);
	}

	public static void C_PongHandler(PacketSession session, IMessage packet)
	{
		ClientSession clientSession = (ClientSession)session;
		clientSession.HandlePong();
	}

    public static void C_ChattingHandler(PacketSession session, IMessage packet)
    {
        ClientSession clientSession = (ClientSession)session;
		C_Chatting chattingPacket = (C_Chatting)packet;

		ChattingManager.Instance.HandleChattingPacket(chattingPacket);
    }

	public static void C_EnterStoreHandler(PacketSession session, IMessage packet)
	{
		ClientSession clientSession = (ClientSession)session;
		C_EnterStore enterstorePacket = (C_EnterStore)packet;


		Console.WriteLine("C_EnterStore Packet Received!");

		// ServerState를 Store로 변경한다. 
		clientSession.HandleServerStateChange(PlayerServerState.ServerStateStore);
	}

	public static void C_EnterLobbyHandler(PacketSession session, IMessage packet)
	{
		ClientSession clientSession = (ClientSession)session;
		C_EnterLobby EnterLobbyPacket = (C_EnterLobby)packet;

		Console.WriteLine("C_EnterLobby Packet Received!");

		// ServerState를 Lobby로 변경한다.
		clientSession.HandleServerStateChange(PlayerServerState.ServerStateLobby);
	}


	public static void C_AddMoneyHandler(PacketSession session, IMessage packet)
	{
		Console.WriteLine("Add Money Received From Client!");

		ClientSession clientSession = (ClientSession)session;
		C_AddMoney AddMoneyPacket = (C_AddMoney)packet;
 
		// DB로부터 돈을 반영한다.
		// Client에게 돈을 보내는 Packet을 작성하는 이유는 Inventory 동기화 및 Money 동기화를 위해서.\
		// 돈을 추가해야하는 것은 StoreManager에서 해야할 일이 아니라 InventoryManager에서 해야할일..
		// StoreManager에서는 Item만 인벤토리에 추가를 위임한다.
		StoreManager.Instance.AddMoney(clientSession, AddMoneyPacket.Moneyamount);

		// Client가 돈을 체워달라고 요청. 돈을 체워준다. 
		// 일단은 DB무시하고 진행. 
		// 어디서 돈을 체워줘야 할까... StoreManager? 
		//FreeMoneyPacket.Moneyamount;
	}
}
