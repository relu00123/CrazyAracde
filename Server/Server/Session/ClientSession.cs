using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Server.Game;
using Server.Data;

namespace Server
{
	public partial class ClientSession : PacketSession
	{
		public PlayerServerState _serverState { get; private set; } = PlayerServerState.ServerStateLogin;

		public PlayerServerState ServerState
		{
			get => _serverState;
			set
			{
				if (_serverState != value) // 현재 상태와 다르다면
				{
					PlayerServerState previousServerState = _serverState;
					_serverState = value;
					HandleServerStateChange(this, previousServerState, value);
				}
			}
		}

		public CharacterType Character { get; set; } =  CharacterType.Dao;


		public Player MyPlayer { get; set; }

		public PlayerInfo MyPlayerInfo {  get; set; }

		public int SessionId { get; set; }

		public GameRoom BeloingRoom { get;  set; }

		public int SlotId { get; set; }


		object _lock = new object();
		List<ArraySegment<byte>> _reserveQueue = new List<ArraySegment<byte>>();

		// 패킷 모아 보내기
		int _reservedSendBytes = 0;
		long _lastSendTick = 0;

		long _pingpongTick = 0;
		public void Ping()
		{
			if (_pingpongTick > 0)
			{
				long delta = (System.Environment.TickCount64 - _pingpongTick);
				if (delta > 30 * 1000)
				{
					Console.WriteLine("Disconnected by PingCheck");
					Disconnect();
					return;
				}
			}

			S_Ping pingPacket = new S_Ping();
			Send(pingPacket);

			GameLogic.Instance.PushAfter(5000, Ping);
		}

		public void HandlePong()
		{
			_pingpongTick = System.Environment.TickCount64;
		}

		#region Network
		// 예약만 하고 보내지는 않는다
		public void Send(IMessage packet)
		{
			string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
			MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
			ushort size = (ushort)packet.CalculateSize();
			byte[] sendBuffer = new byte[size + 4];
			Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
			Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
			Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

			lock (_lock)
			{
				_reserveQueue.Add(sendBuffer);
				_reservedSendBytes += sendBuffer.Length;
			}
		}

		// 실제 Network IO 보내는 부분
		public void FlushSend()
		{
			List<ArraySegment<byte>> sendList = null;

			lock (_lock)
			{
				// 0.1초가 지났거나, 너무 패킷이 많이 모일 때 (1만 바이트)
				long delta = (System.Environment.TickCount64 - _lastSendTick);
				if (delta < 100 && _reservedSendBytes < 10000)
					return;

				// 패킷 모아 보내기
				_reservedSendBytes = 0;
				_lastSendTick = System.Environment.TickCount64;

				sendList = _reserveQueue;
				_reserveQueue = new List<ArraySegment<byte>>();
			}

			Send(sendList);
		}

		public override void OnConnected(EndPoint endPoint)
		{
			//Console.WriteLine($"OnConnected : {endPoint}");

			{
				S_Connected connectedPacket = new S_Connected();
				Send(connectedPacket);
			}

			GameLogic.Instance.PushAfter(5000, Ping);
		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			Console.WriteLine("OnDisconnected Called");

			GameLogic.Instance.Push(() =>
			{
				UserManager.Instance.Push(UserManager.Instance.Remove, MyPlayerInfo.PlayerDbId);
			});
			

			//GameLogic.Instance.Push(() =>
			//{
			//	if (MyPlayer == null)
			//		return;

			//	GameRoom room = GameLogic.Instance.Find(1);
			//	room.Push(room.LeaveGame, MyPlayer.Info.ObjectId);
			//});

			SessionManager.Instance.Remove(this);
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
		#endregion

		public void JoinRoom(GameRoom room)
		{
            ServerState = PlayerServerState.ServerStateRoom;
            BeloingRoom = room;
		}

		public void LeaveRoom()
		{
			if (BeloingRoom != null)
			{
				ServerState = PlayerServerState.ServerStateLobby;
				BeloingRoom.Push(BeloingRoom.RemoveClient, this);
				//BeloingRoom.RemoveClient(this); 
                //BeloingRoom = null;
                //SlotId = -1;
				
            }
		}

	}
}
