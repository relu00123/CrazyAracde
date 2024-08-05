using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DB;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
	public partial class ClientSession : PacketSession
	{
		public int AccountDbId { get; private set; }
		public List<LobbyPlayerInfo> LobbyPlayers { get; set; } = new List<LobbyPlayerInfo>();

		public void HandleLogin(ClientSession clientsession, C_Login loginPacket)
		{
			// TODO : 이런 저런 보안 체크
			if (ServerState != PlayerServerState.ServerStateLogin)
				return;

			LobbyPlayers.Clear();

			using (AppDbContext db = new AppDbContext())
			{
				AccountDb findAccount = db.Accounts
					.Include(a => a.Player)
					.Where(a => a.AccountName == loginPacket.UniqueId).FirstOrDefault();

				if (findAccount != null)
				{
					// AccountDbId 메모리에 기억
					AccountDbId = findAccount.AccountDbId;

					S_Login loginOk = new S_Login() { LoginOk = 1 };

					PlayerDb playerDb = findAccount.Player;

					LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
					{
						PlayerDbId = playerDb.PlayerDbId,
						Name = playerDb.PlayerName,

						LevelInfo = new LevelInfo()
						{
							Level = playerDb.Level,
							Curexp = playerDb.CurExp,
						}
							 
					};

					// 메모리에도 들고 있다
					LobbyPlayers.Add(lobbyPlayer);

					// 패킷에 넣어준다
					loginOk.Players.Add(lobbyPlayer);
					 
					// 단 하나의 Player만 LobbyPlayers에 들어간채로 loginOK패킷을 보낸다. 
					Send(loginOk);
					// 로비로 이동
					//ServerState = PlayerServerState.ServerStateLobby;
					HandleServerStateChange(clientsession, PlayerServerState.ServerStateLobby);
				}
				else
				{
					AccountDb newAccount = new AccountDb() { AccountName = loginPacket.UniqueId };
					db.Accounts.Add(newAccount);
					bool success = db.SaveChangesEx();
					if (success == false)
						return;

					// AccountDbId 메모리에 기억
					AccountDbId = newAccount.AccountDbId;

					S_Login loginOk = new S_Login() { LoginOk = 1 };

					// 이렇게 보내면 LobbyPlayer는 빈 상태로 패킷이 보내지므로
					// 케릭터가 생성이 안되었음을 알 수 있다.
					Send(loginOk);
                    // 로비로 이동
                    //ServerState = PlayerServerState.ServerStateLobby;
                    HandleServerStateChange(clientsession, PlayerServerState.ServerStateLobby);
                }
			}
		}

		public void HandleEnterGame(C_EnterGame enterGamePacket)
		{
			if (ServerState != PlayerServerState.ServerStateLobby)
				return;

			LobbyPlayerInfo playerInfo = LobbyPlayers.Find(p => p.Name == enterGamePacket.Name);
			if (playerInfo == null)
				return;

            // 여기서 부터는 Lobby에 플레이어를 입장 시켜야함

            Console.WriteLine("Lobby에 플레이어를 입장시킬 차례!");
            MyPlayerInfo= new PlayerInfo();
            MyPlayerInfo.PlayerDbId = playerInfo.PlayerDbId;
            MyPlayerInfo.name = playerInfo.Name;
            MyPlayerInfo.Level.MergeFrom(playerInfo.LevelInfo);
            MyPlayerInfo.Session = this;
            MyPlayerInfo.lobbyPlayerInfo = playerInfo;


            // UserManager에 Player를 추가해준다.
            // 여기서 Lobby에 Player입장까지 같이 해준다. 

            GameLogic.Instance.Push(() =>
			{
				UserManager.Instance.Push(UserManager.Instance.Add, MyPlayerInfo.PlayerDbId, MyPlayerInfo);

            });


			//MyPlayer = ObjectManager.Instance.Add<Player>();
			//{
			//	MyPlayer.PlayerDbId = playerInfo.PlayerDbId;
			//	MyPlayer.Info.Name = playerInfo.Name;
			//	MyPlayer.Info.PosInfo.State = CreatureState.Idle;
			//	MyPlayer.Info.PosInfo.MoveDir = MoveDir.Down;
			//	MyPlayer.Info.PosInfo.PosX = 0;
			//	MyPlayer.Info.PosInfo.PosY = 0;
			//	//MyPlayer.Stat.MergeFrom(playerInfo.LevelInfo);
			//	MyPlayer.Session = this;

			//	S_ItemList itemListPacket = new S_ItemList();

			//	// 아이템 목록을 갖고 온다
			//	using (AppDbContext db = new AppDbContext())
			//	{
			//		List<ItemDb> items = db.Items
			//			.Where(i => i.OwnerDbId == playerInfo.PlayerDbId)
			//			.ToList();

			//		foreach (ItemDb itemDb in items)
			//		{
			//			Item item = Item.MakeItem(itemDb);
			//			if (item != null)
			//			{
			//				MyPlayer.Inven.Add(item);

			//				ItemInfo info = new ItemInfo();
			//				info.MergeFrom(item.Info);
			//				itemListPacket.Items.Add(info);
			//			}
			//		}
			//	}

			//	Send(itemListPacket);
			//}

			//ServerState = PlayerServerState.ServerStateGame;

			//GameLogic.Instance.Push(() =>
			//{
				//GameRoom room = GameLogic.Instance.Find(1);
				//room.Push(room.EnterGame, MyPlayer, true);
			//});
		}

		public void HandleCreatePlayer(C_CreatePlayer createPacket)
		{
			// TODO : 이런 저런 보안 체크
			//if (ServerState != PlayerServerState.ServerStateLobby)
			//	return;

			using (AppDbContext db = new AppDbContext())
			{
				PlayerDb findPlayer = db.Players
					.Where(p => p.PlayerName == createPacket.Name).FirstOrDefault();

				if (findPlayer != null)
				{
					// 이름이 겹친다.
					// 이 상황은 일어나면 안된다. 왜냐하면 플레이어가 입력한 ID랑 Player이름이랑 1대1 매칭이기 때문
					Console.WriteLine("Client_prgeame.cs - HandleCreatePlayer() 오류");
					//Send(new S_CreatePlayer());
				}
				else
				{
					// 1레벨 스탯 정보 추출
					//StatInfo stat = null;
					//DataManager.StatDict.TryGetValue(1, out stat);

					// DB에 플레이어 만들어줘야 함
					PlayerDb newPlayerDb = new PlayerDb()
					{
						PlayerName = createPacket.Name,
						Level = 1,
						CurExp = 0,
						AccountDbId = AccountDbId
					};

					db.Players.Add(newPlayerDb);
					bool success = db.SaveChangesEx();
					if (success == false)
						return;

					// 메모리에 추가
					LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
					{
						PlayerDbId = newPlayerDb.PlayerDbId,
						Name = createPacket.Name,
						LevelInfo = new LevelInfo()
						{
							Level = 1,
							Curexp = 0,
						}
						//StatInfo = new StatInfo()
						//{
						//	Level = stat.Level,
						//	Hp = stat.Hp,
						//	MaxHp = stat.MaxHp,
						//	Attack = stat.Attack,
						//	Speed = stat.Speed,
						//	TotalExp = 0
						//}
					};

					// 메모리에도 들고 있다
					LobbyPlayers.Add(lobbyPlayer);

					// 클라에 전송
					S_CreatePlayer newPlayer = new S_CreatePlayer() { Player = new LobbyPlayerInfo() };
					newPlayer.Player.MergeFrom(lobbyPlayer);

					Send(newPlayer);
				}
			}
		}

		public void HandleServerStateChange(ClientSession clientsession, PlayerServerState state)
		{
			if (clientsession.ServerState == PlayerServerState.ServerStateRoom)
			{
				if (state == PlayerServerState.ServerStateLobby)
				{
					BeloingRoom.RemoveClient(clientsession);
				}
			}

			if (state == PlayerServerState.ServerStateLobby)
                RoomManager.Instance.EnterLobby(clientsession);

            // 나중에 함수가 수정되어야 할 수도 있음. 일단은 문제가 터지기 전까지 이 함수 사용. 
            ServerState = state; 
		}
	}
}
