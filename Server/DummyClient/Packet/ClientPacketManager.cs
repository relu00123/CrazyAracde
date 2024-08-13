using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
		
	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<S_EnterGame>);
		_handler.Add((ushort)MsgId.SEnterGame, PacketHandler.S_EnterGameHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveGame, MakePacket<S_LeaveGame>);
		_handler.Add((ushort)MsgId.SLeaveGame, PacketHandler.S_LeaveGameHandler);		
		_onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
		_handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);		
		_onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
		_handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
		_handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);		
		_onRecv.Add((ushort)MsgId.SSkill, MakePacket<S_Skill>);
		_handler.Add((ushort)MsgId.SSkill, PacketHandler.S_SkillHandler);		
		_onRecv.Add((ushort)MsgId.SChangeHp, MakePacket<S_ChangeHp>);
		_handler.Add((ushort)MsgId.SChangeHp, PacketHandler.S_ChangeHpHandler);		
		_onRecv.Add((ushort)MsgId.SDie, MakePacket<S_Die>);
		_handler.Add((ushort)MsgId.SDie, PacketHandler.S_DieHandler);		
		_onRecv.Add((ushort)MsgId.SConnected, MakePacket<S_Connected>);
		_handler.Add((ushort)MsgId.SConnected, PacketHandler.S_ConnectedHandler);		
		_onRecv.Add((ushort)MsgId.SLogin, MakePacket<S_Login>);
		_handler.Add((ushort)MsgId.SLogin, PacketHandler.S_LoginHandler);		
		_onRecv.Add((ushort)MsgId.SCreatePlayer, MakePacket<S_CreatePlayer>);
		_handler.Add((ushort)MsgId.SCreatePlayer, PacketHandler.S_CreatePlayerHandler);		
		_onRecv.Add((ushort)MsgId.SItemList, MakePacket<S_ItemList>);
		_handler.Add((ushort)MsgId.SItemList, PacketHandler.S_ItemListHandler);		
		_onRecv.Add((ushort)MsgId.SAddItem, MakePacket<S_AddItem>);
		_handler.Add((ushort)MsgId.SAddItem, PacketHandler.S_AddItemHandler);		
		_onRecv.Add((ushort)MsgId.SEquipItem, MakePacket<S_EquipItem>);
		_handler.Add((ushort)MsgId.SEquipItem, PacketHandler.S_EquipItemHandler);		
		_onRecv.Add((ushort)MsgId.SChangeStat, MakePacket<S_ChangeStat>);
		_handler.Add((ushort)MsgId.SChangeStat, PacketHandler.S_ChangeStatHandler);		
		_onRecv.Add((ushort)MsgId.SPing, MakePacket<S_Ping>);
		_handler.Add((ushort)MsgId.SPing, PacketHandler.S_PingHandler);		
		_onRecv.Add((ushort)MsgId.SMyPlayerEnterLobby, MakePacket<S_MyPlayerEnterLobby>);
		_handler.Add((ushort)MsgId.SMyPlayerEnterLobby, PacketHandler.S_MyPlayerEnterLobbyHandler);		
		_onRecv.Add((ushort)MsgId.SOtherPlayerEnterLobby, MakePacket<S_OtherPlayerEnterLobby>);
		_handler.Add((ushort)MsgId.SOtherPlayerEnterLobby, PacketHandler.S_OtherPlayerEnterLobbyHandler);		
		_onRecv.Add((ushort)MsgId.SPlayerLeaveLobby, MakePacket<S_PlayerLeaveLobby>);
		_handler.Add((ushort)MsgId.SPlayerLeaveLobby, PacketHandler.S_PlayerLeaveLobbyHandler);		
		_onRecv.Add((ushort)MsgId.SChatting, MakePacket<S_Chatting>);
		_handler.Add((ushort)MsgId.SChatting, PacketHandler.S_ChattingHandler);		
		_onRecv.Add((ushort)MsgId.SAddMoney, MakePacket<S_AddMoney>);
		_handler.Add((ushort)MsgId.SAddMoney, PacketHandler.S_AddMoneyHandler);		
		_onRecv.Add((ushort)MsgId.SAlterRoom, MakePacket<S_AlterRoom>);
		_handler.Add((ushort)MsgId.SAlterRoom, PacketHandler.S_AlterRoomHandler);		
		_onRecv.Add((ushort)MsgId.SJoinRoom, MakePacket<S_JoinRoom>);
		_handler.Add((ushort)MsgId.SJoinRoom, PacketHandler.S_JoinRoomHandler);		
		_onRecv.Add((ushort)MsgId.SJoinRoomBroadcast, MakePacket<S_JoinRoomBroadcast>);
		_handler.Add((ushort)MsgId.SJoinRoomBroadcast, PacketHandler.S_JoinRoomBroadcastHandler);		
		_onRecv.Add((ushort)MsgId.SExitRoomBroadcast, MakePacket<S_ExitRoomBroadcast>);
		_handler.Add((ushort)MsgId.SExitRoomBroadcast, PacketHandler.S_ExitRoomBroadcastHandler);		
		_onRecv.Add((ushort)MsgId.SGameroomCharState, MakePacket<S_GameroomCharState>);
		_handler.Add((ushort)MsgId.SGameroomCharState, PacketHandler.S_GameroomCharStateHandler);		
		_onRecv.Add((ushort)MsgId.SAlterHost, MakePacket<S_AlterHost>);
		_handler.Add((ushort)MsgId.SAlterHost, PacketHandler.S_AlterHostHandler);		
		_onRecv.Add((ushort)MsgId.SChangeScene, MakePacket<S_ChangeScene>);
		_handler.Add((ushort)MsgId.SChangeScene, PacketHandler.S_ChangeSceneHandler);		
		_onRecv.Add((ushort)MsgId.SChangeSlotStateBroadcast, MakePacket<S_ChangeSlotStateBroadcast>);
		_handler.Add((ushort)MsgId.SChangeSlotStateBroadcast, PacketHandler.S_ChangeSlotStateBroadcastHandler);		
		_onRecv.Add((ushort)MsgId.SCharacterSelectResponse, MakePacket<S_CharacterSelectResponse>);
		_handler.Add((ushort)MsgId.SCharacterSelectResponse, PacketHandler.S_CharacterSelectResponseHandler);		
		_onRecv.Add((ushort)MsgId.SCharacterSelectBroadcast, MakePacket<S_CharacterSelectBroadcast>);
		_handler.Add((ushort)MsgId.SCharacterSelectBroadcast, PacketHandler.S_CharacterSelectBroadcastHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

		if (CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}