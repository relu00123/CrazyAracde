using Google.Protobuf;
using Google.Protobuf.Protocol;
using Microsoft.Extensions.Logging.Abstractions;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Server.Game
{
	public partial class GameRoom : JobSerializer
	{
		private class Slot
		{
			public bool IsAvailable { get; set; } = true;
			public ClientSession ClientSession { get; set; } = null;
			public GameRoomCharacterStateType CharacterState { get; set; } = GameRoomCharacterStateType.NotReady;
			public CharacterType CharType { get; set; } = CharacterType.CharacterNone;
		}

		public RoomInfo _roomInfo { get; private set; }
		public int _roomId { get; set; }

		private Slot[] _slots = new Slot[8];
		private int _hostIndex { get; set; } = -1;

		public bool _isClosed { get; private set; } = false; // 방이 더이상 존재하는 방인지 따질때 사용.



		// 강의때 사용했던 변수들 나는 쓰지 않음.
        public const int VisionCells = 5;
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
		Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
		Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();


		public GameRoom(int roomid, RoomInfo roominfo)
		{
			_roomId = roomid;
			roominfo.RoomNumber = _roomId;
			_roomInfo = roominfo;

			for (int i = 0; i < _slots.Length; i++)
			{
				_slots[i] = new Slot();
			}

			InitializeSlotCount(_roomInfo.GameMode);
		}

		private void InitializeSlotCount(GameModeType gameModeType)
		{
			if (gameModeType == GameModeType.NormalMode)
			{
                _roomInfo.MaxPeopleCnt = 8;
            }

			else if (gameModeType == GameModeType.MonsterMode)
			{
                _roomInfo.MaxPeopleCnt = 4;
                for (int i = _roomInfo.MaxPeopleCnt; i < _slots.Length; i++)
                {
                    _slots[i].IsAvailable = false;
                }
            }

			else if (gameModeType == GameModeType.AIMode)
			{
                _roomInfo.MaxPeopleCnt = 8;
                // 음.. 인게임에서 AI Mode는 혼자 놀도록 해놨는데.. 
                // 직접구현한다면 어떻게 해야할지 몰루?
            }
        }


        #region Lecture

        public Zone[,] Zones { get; private set; }
		public int ZoneCells { get; private set; }

        public Map Map { get; private set; } = new Map();

        // ㅁㅁㅁ
        // ㅁㅁㅁ
        // ㅁㅁㅁ
        public Zone GetZone(Vector2Int cellPos)
        {
            int x = (cellPos.x - Map.MinX) / ZoneCells;
            int y = (Map.MaxY - cellPos.y) / ZoneCells;
            return GetZone(y, x);
        }

        public Zone GetZone(int indexY, int indexX)
        {
            if (indexX < 0 || indexX >= Zones.GetLength(1))
                return null;
            if (indexY < 0 || indexY >= Zones.GetLength(0))
                return null;

            return Zones[indexY, indexX];
        }


        #endregion


        public void Init(int mapId, int zoneCells)
		{
			Map.LoadMap(mapId);

			// Zone
			ZoneCells = zoneCells; // 10
			// 1~10 칸 = 1존
			// 11~20칸 = 2존
			// 21~30칸 = 3존
			int countY = (Map.SizeY + zoneCells - 1) / zoneCells;
			int countX = (Map.SizeX + zoneCells - 1) / zoneCells;
			Zones = new Zone[countY, countX];
			for (int y = 0; y < countY; y++)
			{
				for (int x = 0; x < countX; x++)
				{
					Zones[y, x] = new Zone(y, x);
				}
			}

			// TEMP
			for (int i = 0; i < 500; i++)
			{
				Monster monster = ObjectManager.Instance.Add<Monster>();
				monster.Init(1);
				EnterGame(monster, randomPos: true);
			}
		}

		// 누군가 주기적으로 호출해줘야 한다
		public void Update()
		{
			Flush();

			if ( IsRoomEmpty() && !_isClosed)
			{
				CloseRoom();
			}
		}

		private bool IsRoomEmpty()
		{
			foreach (var slot in _slots)
			{
				if (slot.ClientSession != null)
					return false;
			}
			return true;
		}

        private int? GetNextAvailableSlot()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].IsAvailable)
                    return i;
            }

            return null;
        }

		public void KickPlayer(ClientSession session, int kickslotidx)
		{
			if (kickslotidx < 0 || kickslotidx >= _slots.Length) 
				return;

			if (_slots[kickslotidx].ClientSession != null)
			{
				S_ChangeScene changeScenePakcet = new S_ChangeScene();
				changeScenePakcet.Scene = GameSceneType.CAMainLobby;
				_slots[kickslotidx].ClientSession.Send(changeScenePakcet);
				_slots[kickslotidx].ClientSession.ServerState = PlayerServerState.ServerStateLobby;
			 
			}
		}

        public void AddClient(ClientSession session)
		{
			int? slotId = GetNextAvailableSlot();
			if (slotId == null)
				return;   // 방이 꽉차서 더이상 추가가 안될 경우

            // 현재 Room의 인원정보를 수정한다.
            _roomInfo.CurPeopleCnt += 1;
            RoomManager.Instance.BroadCastRoomAlter(_roomId);


			// Host 역할을 맡고 있는 사람이 아무도 없다면 Host를 시켜준다.
			if (_hostIndex == -1)
				FindNewHost();

            // S_JoinRoomBroadcast Packet을 보내준다. (기존에 있던 클라이언트들)
            S_JoinRoomBroadcast joinRoomBroadcastPacket = new S_JoinRoomBroadcast();
			SlotInfo JoinRoomPacketSlotInfo = new SlotInfo
			{
				SlotIndex = slotId.Value,
				IsAvailable = false,
				PlayerId = session.AccountDbId,
				Character = session.Character
			};
			joinRoomBroadcastPacket.SlotInfo = JoinRoomPacketSlotInfo;

			for (int i = 0; i < _slots.Length; ++i)
			{
				if (_slots[i].ClientSession != null)
					_slots[i].ClientSession.Send(joinRoomBroadcastPacket);
			}

            // 새로들어온 클라이언트를 GameRoom에서 관리하도록 한다.
            session.SlotId = slotId.Value;
            session.JoinRoom(this);
            _slots[slotId.Value].ClientSession = session;
            _slots[slotId.Value].IsAvailable = false;
		 

            // S_JoinRoom Packet을 보내준다. (접속한 클라이언트)
            S_JoinRoom joinRoomPacket = new S_JoinRoom();
			joinRoomPacket.Joinresult = JoinResultType.Success;
			joinRoomPacket.HostIdx = _hostIndex;
			joinRoomPacket.ClientslotIdx = slotId.Value;

			for (int i = 0; i < _slots.Length; ++i)
			{
				SlotInfo slotInfo = new SlotInfo
				{
					SlotIndex = i,
					IsAvailable = _slots[i].IsAvailable
				};

				slotInfo.PlayerId = _slots[i].ClientSession == null ? -1 : _slots[i].ClientSession.AccountDbId;

				if (_slots[i].ClientSession != null)
				{
                    slotInfo.Character = _slots[i].ClientSession.Character;
                }

				slotInfo.CharacterState = _slots[i].CharacterState;
				slotInfo.Character = _slots[i].CharType;

				joinRoomPacket.SlotInfos.Add(slotInfo);				
			}
			session.Send(joinRoomPacket);        
		}

		public void RemoveClient(ClientSession session)
		{
			int slotId = session.SlotId;
			if (slotId < 0 || slotId >= _slots.Length || _slots[slotId].ClientSession != session)
			{
				Console.WriteLine("GameRoom 에서 Client 삭제 실패 (치명적 오류)");
				return;
			}
          
            // 해당 Session의 삭제를 방에 있는 남아있는 유저들에게 알린다. 
            for (int i = 0; i < _slots.Length; ++i)
			{
				if (_slots[i].ClientSession == null || _slots[i].ClientSession == session) continue;

				S_ExitRoomBroadcast exitRoomPacket = new S_ExitRoomBroadcast();
				exitRoomPacket.SlotId = slotId;
				_slots[i].ClientSession.Send(exitRoomPacket);
			}

			session.BeloingRoom = null;
			session.SlotId = -1;

			_slots[slotId].ClientSession = null;
			_slots[slotId].IsAvailable = true;
			_slots[slotId].CharacterState = GameRoomCharacterStateType.NotReady;

            

            if (slotId == _hostIndex)
				FindNewHost();


            // 현재 Room의 인원정보를 수정하고 Lobby에 있는 플레이어들에게 알린다. 
            _roomInfo.CurPeopleCnt -= 1;
			RoomManager.Instance.BroadCastRoomAlter(_roomId);			 
		}

		private void FindNewHost()
		{
			S_AlterHost alterHostPacket = new S_AlterHost();
			alterHostPacket.Previousidx = _hostIndex;

			if (_hostIndex != -1)
				_slots[_hostIndex].CharacterState = GameRoomCharacterStateType.NotReady;
 

            _hostIndex = -1;
            // Host가 나갔을 때 가장 앞에 있는 클라이언트를 새로운 Host로 설정
            for (int i = 0; i < _slots.Length; i++)
			{
				if (!_slots[i].IsAvailable && _slots[i].ClientSession != null)
				{
					_hostIndex = i;
					_slots[_hostIndex].CharacterState = GameRoomCharacterStateType.Host;
					alterHostPacket.Nowidx = i;

					Console.WriteLine($"New Host Found! It's {_hostIndex} slot!");
					break;
				}
			}

            for (int i = 0; i < _slots.Length; i++)
			{
				if (!_slots[i].IsAvailable && _slots[i].ClientSession != null)
				{
					_slots[i].ClientSession.Send(alterHostPacket);
				}
			}
		}

		public ClientSession GetClientBySlot(int slotId)
		{
			if (slotId < 0 || slotId >= _slots.Length)
				return null;
			return _slots[slotId].ClientSession;
		}


		// Host가 Slot을 열고 닫을때 사용하는 함수. 만약 사용자가 있다면 해당 사용자를 로비로 킥해야한다.
		// 불안정한 코드이고 아직 개선이 필요한 코드

		public void CloseSlot(int slotId)
		{
            //if (slotId >= 0 && slotId < _slots.Length)
            //{
            //    _slots[slotId].IsAvailable = false;
            //    _slots[slotId].ClientSession = null;
            //}
        }

		public void OpenSlot(int slotId)
		{
            //if (slotId >= 0 && slotId < _slots.Length)
            //{
            //    _slots[slotId].IsAvailable = true;
            //    _slots[slotId].ClientSession = null;
            //}
        }


		private void CloseRoom()
		{
			_isClosed = true;

			//GameLogic.Instance.RemoveRoom(_roomId);
			RoomManager.Instance.RemoveRoom(_roomId);
		}

		Random _rand = new Random();
		public void EnterGame(GameObject gameObject, bool randomPos)
		{
			if (gameObject == null)
				return;

			if (randomPos)
			{
				Vector2Int respawnPos;
				while (true)
				{
					respawnPos.x = _rand.Next(Map.MinX, Map.MaxX + 1);
					respawnPos.y = _rand.Next(Map.MinY, Map.MaxY + 1);
					if (Map.Find(respawnPos) == null)
					{
						gameObject.CellPos = respawnPos;
						break;
					}
				}
			}

			GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

			if (type == GameObjectType.Player)
			{
				Player player = gameObject as Player;
				_players.Add(gameObject.Id, player);
				player.Room = this;

				player.RefreshAdditionalStat();

				Map.ApplyMove(player, new Vector2Int(player.CellPos.x, player.CellPos.y));
				GetZone(player.CellPos).Players.Add(player);

				// 본인한테 정보 전송
				{
					S_EnterGame enterPacket = new S_EnterGame();
					enterPacket.Player = player.Info;
					player.Session.Send(enterPacket);

					player.Vision.Update();
				}
			}
			else if (type == GameObjectType.Monster)
			{
				Monster monster = gameObject as Monster;
				_monsters.Add(gameObject.Id, monster);
				monster.Room = this;

				GetZone(monster.CellPos).Monsters.Add(monster);
				Map.ApplyMove(monster, new Vector2Int(monster.CellPos.x, monster.CellPos.y));

				monster.Update();
			}
			else if (type == GameObjectType.Projectile)
			{
				Projectile projectile = gameObject as Projectile;
				_projectiles.Add(gameObject.Id, projectile);
				projectile.Room = this;

				GetZone(projectile.CellPos).Projectiles.Add(projectile);
				projectile.Update();
			}

			// 타인한테 정보 전송
			{
				S_Spawn spawnPacket = new S_Spawn();
				spawnPacket.Objects.Add(gameObject.Info);
				Broadcast(gameObject.CellPos, spawnPacket);
			}
		}

		public void LeaveGame(int objectId)
		{
			GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

			Vector2Int cellPos;

			if (type == GameObjectType.Player)
			{
				Player player = null;
				if (_players.Remove(objectId, out player) == false)
					return;

				cellPos = player.CellPos;

				player.OnLeaveGame();
				Map.ApplyLeave(player);
				player.Room = null;

				// 본인한테 정보 전송
				{
					S_LeaveGame leavePacket = new S_LeaveGame();
					player.Session.Send(leavePacket);
				}
			}
			else if (type == GameObjectType.Monster)
			{
				Monster monster = null;
				if (_monsters.Remove(objectId, out monster) == false)
					return;

				cellPos = monster.CellPos;
				Map.ApplyLeave(monster);
				monster.Room = null;
			}
			else if (type == GameObjectType.Projectile)
			{
				Projectile projectile = null;
				if (_projectiles.Remove(objectId, out projectile) == false)
					return;

				cellPos = projectile.CellPos;
				Map.ApplyLeave(projectile);
				projectile.Room = null;
			}
			else
			{
				return;
			}

			// 타인한테 정보 전송
			{
				S_Despawn despawnPacket = new S_Despawn();
				despawnPacket.ObjectIds.Add(objectId);
				Broadcast(cellPos, despawnPacket);
			}
		}

		Player FindPlayer(Func<GameObject, bool> condition)
		{
			foreach (Player player in _players.Values)
			{
				if (condition.Invoke(player))
					return player;
			}

			return null;
		}

		// 살짝 부담스러운 함수
		public Player FindClosestPlayer(Vector2Int pos, int range)
		{
			List<Player> players = GetAdjacentPlayers(pos, range);

			players.Sort((left, right) =>
			{
				int leftDist = (left.CellPos - pos).cellDistFromZero;
				int rightDist = (right.CellPos - pos).cellDistFromZero;
				return leftDist - rightDist;
			});

			foreach (Player player in players)
			{
				List<Vector2Int> path = Map.FindPath(pos, player.CellPos, checkObjects: true);
				if (path.Count < 2 || path.Count > range)
					continue;

				return player;
			}

			return null;
		}

		public void Broadcast(Vector2Int pos, IMessage packet)
		{
			List<Zone> zones = GetAdjacentZones(pos);

			foreach (Player p in zones.SelectMany(z => z.Players))
			{
				int dx = p.CellPos.x - pos.x;
				int dy = p.CellPos.y - pos.y;
				if (Math.Abs(dx) > GameRoom.VisionCells)
					continue;
				if (Math.Abs(dy) > GameRoom.VisionCells)
					continue;

				p.Session.Send(packet);
			}
		}

		public List<Player> GetAdjacentPlayers(Vector2Int pos, int range)
		{
			List<Zone> zones = GetAdjacentZones(pos, range);
			return zones.SelectMany(z => z.Players).ToList();
		}

		// ㅁㅁㅁㅁㅁㅁ
		// ㅁㅁㅁㅁㅁㅁ
		// ㅁㅁㅁㅁㅁㅁ
		// ㅁㅁㅁㅁㅁㅁ
		public List<Zone> GetAdjacentZones(Vector2Int cellPos, int range = GameRoom.VisionCells)
		{
			HashSet<Zone> zones = new HashSet<Zone>();

			int maxY = cellPos.y + range;
			int minY = cellPos.y - range;
			int maxX = cellPos.x + range;
			int minX = cellPos.x - range;

			// 좌측 상단
			Vector2Int leftTop = new Vector2Int(minX, maxY);
			int minIndexY = (Map.MaxY - leftTop.y) / ZoneCells;
			int minIndexX = (leftTop.x - Map.MinX) / ZoneCells;
			
			// 우측 하단
			Vector2Int rightBot = new Vector2Int(maxX, minY);
			int maxIndexY = (Map.MaxY - rightBot.y) / ZoneCells;
			int maxIndexX = (rightBot.x - Map.MinX) / ZoneCells;

			for (int x = minIndexX; x <= maxIndexX; x++)
			{
				for (int y = minIndexY; y <= maxIndexY; y++)
				{
					Zone zone = GetZone(y, x);
					if (zone == null)
						continue;

					zones.Add(zone);
				}
			}

			return zones.ToList();
		}

		public void HandleStartGame()
		{

		}

		public void HandleSetReady(ClientSession clientSession)
		{
			int slotidx = -1;
			GameRoomCharacterStateType state = GameRoomCharacterStateType.NotReady;

			for (int i = 0; i < _slots.Length; i++)
			{
				if (_slots[i].ClientSession == clientSession)
				{
					slotidx = i;

					if (_slots[i].CharacterState == GameRoomCharacterStateType.Ready)
					{
						_slots[i].CharacterState = GameRoomCharacterStateType.NotReady;
						state = GameRoomCharacterStateType.NotReady;
					}
					else if (_slots[i].CharacterState == GameRoomCharacterStateType.NotReady)
					{
						_slots[i].CharacterState = GameRoomCharacterStateType.Ready;
						state = GameRoomCharacterStateType.Ready;
					}

					break;
				}
			}

			for (int i = 0; i < _slots.Length; i++)
			{
				if (_slots[i].ClientSession != null)
				{
					S_GameroomCharState charstate = new S_GameroomCharState();
					charstate.SlotId = slotidx;
					charstate.Charstate = state;

					_slots[i].ClientSession.Send(charstate);
				}
			}
		}

		public void HandleChangeSlotState(C_ChangeSlotState changeslotpkt)
		{
			// slotidx 와 // close 인지 open인지 여부를 알고 있다.
			// 우선 서버의 slot상태를 바꿔준다.
			if (changeslotpkt.Slotidx < 0 || changeslotpkt.Slotidx >= _slots.Length)
				Console.WriteLine("C_ChangeSlotState Packet Error");


			// 모드에 따라서 열수 없을 수 있음.
			if (changeslotpkt.Isopen)
			{
				if (_roomInfo.GameMode == GameModeType.MonsterMode)
				{
					if (changeslotpkt.Slotidx >= 4) return;
				}
			}

			_slots[changeslotpkt.Slotidx].IsAvailable = changeslotpkt.Isopen;


			// 클라이언트들에게 바뀐 Slot 상태를 Broadcast한다.
			S_ChangeSlotStateBroadcast broadcastpkt = new S_ChangeSlotStateBroadcast();
			broadcastpkt.Slotidx = changeslotpkt.Slotidx;
			broadcastpkt.Isopen = changeslotpkt.Isopen; ;

			for (int i = 0; i < _slots.Length; ++i)
			{
				if (_slots[i].ClientSession != null)
				{
					_slots[i].ClientSession.Send(broadcastpkt);
				}
			}

			// MaxPeople Count 변경이 있는데, 이를 Client들에게 Boradcast한다. 
			if (changeslotpkt.Isopen)
				_roomInfo.MaxPeopleCnt += 1;
			else
				_roomInfo.MaxPeopleCnt -= 1;
            RoomManager.Instance.BroadCastRoomAlter(_roomId);
        }

		public void HandleCharacterSelect(ClientSession clientSession, C_CharacterSelect pkt)
		{
			int slotidx = clientSession.SlotId;
			if (slotidx < 0 || slotidx > _slots.Length)
			{
				Console.WriteLine("Character Select Handle Error");
				return;
			}

			// 모드에 따라서 캐릭터를 변경할 할 수도 있고 없을 수도 있다.
			if (_roomInfo.GameMode == GameModeType.NormalMode)
			{
                // NormalMode에서는 아무 캐릭터나 골라도 상관없다.
                _slots[slotidx].CharType = pkt.Chartype;

				// 요청을 한 클라이언트에게 결과를 알려준다. ( Client Check 표시용도)
                S_CharacterSelectResponse characterSelectResponse = new S_CharacterSelectResponse();
				characterSelectResponse.IsSuccess = true;
				characterSelectResponse.Chartype = pkt.Chartype;
				clientSession.Send(characterSelectResponse);

				// 다른 클라이언트들에게 캐릭터 변경을 알려준다.
				S_CharacterSelectBroadcast characterSelectBroadcast = new S_CharacterSelectBroadcast();
				characterSelectBroadcast.Slotid = slotidx;
				characterSelectBroadcast.Chartype = pkt.Chartype;

				for (int i = 0; i < _slots.Length; ++i)
				{
					if (_slots[i].ClientSession != null)
						_slots[i].ClientSession.Send(characterSelectBroadcast);
				}

				// DB에 CharacterType바뀐 것을 저장해야한다. 
				// 아직 DB가 없으므로 진행 X.. 나중에 해줘야 한다. 
			}

			else
			{
                // AI와 MonsterMode에서는 첫번째 캐릭터(Bazzi)만 가능하다. 
                // 해당 모드들에서는 캐릭터의 선택이 Bazzi가 되도록 할 것이므로 False를 보내준다. 
                S_CharacterSelectResponse characterSelectResponse = new S_CharacterSelectResponse();
                characterSelectResponse.IsSuccess = false;
                clientSession.Send(characterSelectResponse);
            }
		}

		public void TestFunc()
		{
			Console.WriteLine($"Test Func Called from Room_{_roomId}");
			 
		}
	}
}
