using Google.Protobuf;
using Google.Protobuf.Protocol;
using Microsoft.Extensions.Logging.Abstractions;
using Server.Data;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static Server.Game.GameRoom;

namespace Server.Game
{
	public partial class GameRoom : JobSerializer
	{
		public class Slot
		{
			public bool IsAvailable { get; set; } = true;
			public ClientSession ClientSession { get; set; } = null;
			public GameRoomCharacterStateType CharacterState { get; set; } = GameRoomCharacterStateType.NotReady;
			public CharacterType CharType { get; set; } = CharacterType.CharacterNone;

			public bool isGameLoaded = false; 
		}

		public RoomInfo _roomInfo { get; private set; }
		public int _roomId { get; set; }

		private Slot[] _slots = new Slot[8];

		public double _deltaTime { get; set; }

		public Slot[] Slots
		{
			get { return _slots; }
		}

		private int _hostIndex { get; set; } = -1;
		MapType SelectedMap { get; set; }  // 작업 시작 할 부분. 이제 막 변수만 만들어 놓음. Packet Handler 부터 작성하면 된다.
		MapTeamType SelectedMapTeamType { get; set; }

		public bool _isClosed { get; private set; } = false; // 방이 더이상 존재하는 방인지 따질때 사용.

		public InGame _inGame { get; private set; }

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


		public void EndGame(CharacterType charType, bool isDraw)
		{
			S_EndGame endGamePkt = new S_EndGame();

			// 0. 기존에 Host였던 사람은 host를 시켜주고 아닌 사람들은 NotReady로 바꿔준다.
			// 사실 이것은 EndGame()에서 하는 것이 아니라 StartGame에서 해줘야 할 책임이 있다.
			// 우선은 동작하는지 확인
			for (int i = 0; i < _slots.Length; ++i)
			{
				if (_slots[i] != null & _slots[i].ClientSession != null)
				{
					if (_hostIndex == i)
						_slots[i].CharacterState = GameRoomCharacterStateType.Host;
					else
						_slots[i].CharacterState = GameRoomCharacterStateType.NotReady;

					if (isDraw == true)
						endGamePkt.GameResult = GameResultType.GameDraw;
					else
					{
						if (_slots[i].CharType == charType)
							endGamePkt.GameResult = GameResultType.GameWin;
						else
							endGamePkt.GameResult = GameResultType.GameLose;
					}

					_slots[i].ClientSession.Send(endGamePkt);


					// Animation 전환용 
					CAPlayer focusedPlayer = _slots[i].ClientSession.CA_MyPlayer;

					if (!(focusedPlayer._currentState is Player_DeadState))
					{
						S_Move movepkt = new S_Move();

						movepkt.ObjectId = focusedPlayer.Id;
						movepkt.PosInfo = new PositionInfo();
						movepkt.PosInfo.PosX = focusedPlayer._transform.Position.X;
						movepkt.PosInfo.PosY = focusedPlayer._transform.Position.Y;
						movepkt.PosInfo.MoveDir = MoveDir.Down;
						movepkt.PosInfo.State = CreatureState.Idle;
						BroadcastPacket(movepkt);

						S_ChangeAnimation changeAnimation = new S_ChangeAnimation();
						changeAnimation.ObjectId = focusedPlayer.Id;
						changeAnimation.PlayerAnim = PlayerAnimState.PlayerAnimIdle;
						BroadcastPacket(changeAnimation);
					}
				}
			}

            PushAfter(2000, PostEndGame);

			// 2. GameRoom에서 변경해야 하는 값들 변경
			_inGame = null;

			//FindNewHost(); --> 다시해야할듯..?

			// 마음에 안듬.. 일단 테스트 만약에 다 찼으면 Waiting이 아니겠지.
			// 
		}

		public void PostEndGame()
		{
			Console.WriteLine("PostEndGame Called!");

            // 1. Client Session 에서 참조하고 있는 값들중 변경해야할 것들 변경
            for (int i = 0; i < _slots.Length; ++i)
            {
                if (_slots[i] != null && _slots[i].ClientSession != null)
                {
                    ClientSession clientSession = _slots[i].ClientSession;
                    clientSession.ServerState = PlayerServerState.ServerStateRoom;
                    clientSession.CA_MyPlayer = null;

                    ReEnterRoom(clientSession, i);
                }
            }

			// 사람이 꽉차있지 않으면 Waiting, 꽉차있으면 Full
			if (_roomInfo.MaxPeopleCnt == _roomInfo.CurPeopleCnt)
				_roomInfo.RoomState = RoomStateType.Full;
			else
				_roomInfo.RoomState = RoomStateType.Waiting;

			S_PostEndGame postEndGamePkt = new S_PostEndGame();

			BroadcastPacket(postEndGamePkt);

            RoomManager.Instance.BroadCastRoomAlter(_roomId);
        }

        public void ReEnterRoom(ClientSession clientSession, int slotidx) // 09.29 작성중인 코드 
        {
            S_JoinRoom joinRoomPacket = new S_JoinRoom();
            joinRoomPacket.Joinresult = JoinResultType.Success;
            joinRoomPacket.HostIdx = _hostIndex;
            joinRoomPacket.ClientslotIdx = slotidx;
            joinRoomPacket.Maptype = SelectedMap;
            joinRoomPacket.Gamemode = _roomInfo.GameMode;

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
            clientSession.Send(joinRoomPacket);
        }

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

			if (_inGame != null && _inGame._isGameFinished == false)
			{
				_inGame.Update();
			}

			if ( IsRoomEmpty() && !_isClosed)
			{
				CloseRoom();
			}
		}

		public void StartGame(MapType maptype)
		{
			// GameRoom에 알려줘야 할 정보는 뭐가있을까?

			// 어떤 맵을 플레이할 것인지 
			// 누가 참여하는지 (clientSession)
			// 참여하는 ClientSession의 Team정보 (CharacterType이 곧 Team정보임)

			_roomInfo.RoomState = RoomStateType.Playing;

			S_AlterRoom alterRoomPacket = new S_AlterRoom();
			alterRoomPacket.Altertype = RoomAlterType.Alter;
			alterRoomPacket.Roominfo = _roomInfo;


			for (int i = 0; i < _slots.Length; ++i)
			{
				if (_slots[i].ClientSession != null)
				{
					_slots[i].ClientSession.Send(alterRoomPacket);

				}
			}

			// 로비에 있는 사람들에게 alterRoom Packet보내야 한다. 
            RoomManager.Instance.BroadCastRoomAlter(_roomId);


            // 여기 코드를 Ack가 모두 모였을때로 미뤄야 함. 
            //_inGame = new InGame(this, maptype);
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


			// 방이 Playing중일 경우 안됨
			if (_roomInfo.RoomState == RoomStateType.Playing)
				return;


            // 현재 Room의 인원정보를 수정한다.
            _roomInfo.CurPeopleCnt += 1;

			UpdateRoomState(); 

            RoomManager.Instance.BroadCastRoomAlter(_roomId);


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

            // 기존

            // 새로들어온 클라이언트를 GameRoom에서 관리하도록 한다.
            session.SlotId = slotId.Value;
            session.JoinRoom(this);
            _slots[slotId.Value].ClientSession = session;
            _slots[slotId.Value].IsAvailable = false;
			_slots[slotId.Value].CharType = session.Character; // 10.07추가 코드


            // S_JoinRoom Packet을 보내준다. (접속한 클라이언트)
            S_JoinRoom joinRoomPacket = new S_JoinRoom();
			joinRoomPacket.Joinresult = JoinResultType.Success;
			joinRoomPacket.HostIdx = _hostIndex;
			joinRoomPacket.ClientslotIdx = slotId.Value;
			joinRoomPacket.Maptype = _roomInfo.MapType;
			joinRoomPacket.Gamemode = _roomInfo.GameMode;

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
				//slotInfo.Character = _slots[i].CharType;

				joinRoomPacket.SlotInfos.Add(slotInfo);				
			}
			session.Send(joinRoomPacket);



            // Host 역할을 맡고 있는 사람이 아무도 없다면 Host를 시켜준다.
            if (_hostIndex == -1)
            {
                FindNewHost();

				// 처음 시작 맵 초기화
				if (_roomInfo.GameMode == GameModeType.NormalMode)
				{
					SelectedMap = _roomInfo.MapType;
					//SelectedMap = MapType.Desert1;
				}
				else if (_roomInfo.GameMode == GameModeType.MonsterMode)
					SelectedMap = MapType.Penguin1;
            }



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
			UpdateRoomState();

			RoomManager.Instance.BroadCastRoomAlter(_roomId);			 
		}

		public void UpdateRoomState()
		{
			if (_roomInfo.RoomState == RoomStateType.Playing)
				return;

			if (_roomInfo.CurPeopleCnt == _roomInfo.MaxPeopleCnt)
				_roomInfo.RoomState = RoomStateType.Full;
			else
				_roomInfo.RoomState = RoomStateType.Waiting;
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

		public void HandleStartGame(ClientSession clientSession)
		{
            bool isAllReady = IsAllPlayerReady(clientSession.SlotId);

            if (!isAllReady)
            {
                S_StartGameRes startGameResPkt = new S_StartGameRes();
                startGameResPkt.IsSuccess = false;
                clientSession.Send(startGameResPkt);
                return;
            }


            S_StartGameBroadcast StartGameBroadcastPkt = new S_StartGameBroadcast();
			StartGameBroadcastPkt.Maptype = SelectedMap;
			
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].ClientSession != null)
                    _slots[i].ClientSession.Send(StartGameBroadcastPkt);
            }

			// 이부분은 추가해야함
			// TileMap을 로드해줘야 한다. 
			StartGame(SelectedMap);

			return;


            ///////////////////////////////////


            // 모든 플레이어가 Ready상태가 아니면 return
            // S_StartGameRes에서 FairGame인지 AllPlayerNotReady등의 Type을 건내줘야 좋을듯
            //bool isAllReady =  IsAllPlayerReady(clientSession.SlotId);

			if (!isAllReady)
			{
                S_StartGameRes startGameResPkt = new S_StartGameRes();
                startGameResPkt.IsSuccess = false;
                clientSession.Send(startGameResPkt);
                return;
            }

			if (_roomInfo.GameMode == GameModeType.NormalMode)
			{
				if (_roomInfo.TeamMode == TeamModeType.MannerMode)
				{
					bool isfairgame = JudgeIsFairGame();
					 
					if (!isfairgame)
					{
						S_StartGameRes startGameResPkt = new S_StartGameRes();
						startGameResPkt.IsSuccess = false;
						clientSession.Send(startGameResPkt);
						return;
					}
				}
			}
 
			// Success인 경우에는 Client들에게 성공결과를 Broadcast 
			S_StartGameBroadcast StartGameBroadcastPacket = new S_StartGameBroadcast();
			for (int i = 0; i < _slots.Length; i++)
			{
				if (_slots[i].ClientSession != null)
					_slots[i].ClientSession.Send(StartGameBroadcastPacket);
			}
		}

		private bool IsAllPlayerReady(int hostidx)
		{
			for (int i = 0; i < _slots.Length; ++i)
			{
				if (_slots[i].ClientSession != null && _slots[i].ClientSession.SlotId != hostidx)
				{
					if (_slots[i].CharacterState != GameRoomCharacterStateType.Ready)
						return false;
				}
			}

			return true;
		}


		private bool JudgeIsFairGame()
		{
            Dictionary<CharacterType, int> teamMemberCounts = new Dictionary<CharacterType, int>();

            for (int i = 0; i < _slots.Length; ++i)
            {
                if (_slots[i].ClientSession != null)
                {
                    CharacterType type = _slots[i].CharType;

                    if (teamMemberCounts.ContainsKey(type))
                        teamMemberCounts[type]++;
                    else
                        teamMemberCounts[type] = 1;
                }
            }

			// Team이 하나만 경우한 경우도 게임을 시작할 수 없다.
			if (teamMemberCounts.Count <= 1)
			{
				return false;
			}

            int? standardCount = null;
            foreach (var count in teamMemberCounts.Values)
            {
                if (standardCount == null)
                    standardCount = count;
                else if (standardCount != count)
                {
					// 팀원수가 다르면 게임을 시작할 수 없음
					return false;

                }
            }

			return true;
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
			UpdateRoomState();

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
				clientSession.Character = pkt.Chartype;
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

		public void HandleMapSelect(C_MapSelect pkt)
		{
			// Server에서의 Map변경. 변경후 모든 Client에게 Map이 바뀌었다고 BroadCast.
			SelectedMap = pkt.Maptype;

			// Camp8 같은 특수 Team A : Team B 로 나눠서하는 맵은 선택을 캐릭터 선택을 2개로 제한해야한다. 
			// 일단은 야매로 처리.. 
			if (SelectedMap == MapType.Camp8)
				SelectedMapTeamType = MapTeamType.TwoTeam;
			else
				SelectedMapTeamType = MapTeamType.FourTeam;

			_roomInfo.MapType = pkt.Maptype;

			S_MapSelectBroadcast MapSelectBroadcastPkt = new S_MapSelectBroadcast();
			MapSelectBroadcastPkt.Maptype = SelectedMap;
			MapSelectBroadcastPkt.MapTeamType = SelectedMapTeamType;

			for (int i = 0; i <_slots.Length; ++i)
			{
				if (_slots[i].ClientSession != null)
					_slots[i].ClientSession.Send(MapSelectBroadcastPkt);
			}

            RoomManager.Instance.BroadCastRoomAlter(_roomId);  
		}

		public void HandleGameLoadFinished(C_GameSceneLoadFinished pkt, ClientSession clientsession)
		{
			//bool allPlayersLoaded = true;

			//for (int i = 0; i < _slots.Length; ++i)
			//{
			//    if (_slots[i].ClientSession != null)
			//    {
			//        if (!_slots[i].isGameLoaded)
			//        {
			//            _slots[i].isGameLoaded = true;
			//        }
			//    }

			//    // 모든 플레이어가 로드를 완료했는지 확인
			//    if (_slots[i].ClientSession != null && !_slots[i].isGameLoaded)
			//    {
			//        allPlayersLoaded = false;
			//    }
			//}

			//if (allPlayersLoaded)
			//{
			//    // 모든 플레이어가 로드되었을 때만 InGame 객체를 생성하고 맵을 로드
			//    _inGame = new InGame(this, SelectedMap);

			//    // 모든 플레이어의 로드 상태를 초기화
			//    for (int i = 0; i < _slots.Length; ++i)
			//    {
			//        if (_slots[i].ClientSession != null)
			//        {
			//            _slots[i].isGameLoaded = false;
			//        }
			//    }
			//}


			int isAllLoadedFailCnt = 0;

			for (int i = 0; i < _slots.Length; ++i)
			{
				if (_slots[i].ClientSession != null)
				{
					if (_slots[i].isGameLoaded == true)
						continue;
					else if (_slots[i].isGameLoaded == false)
					{
						isAllLoadedFailCnt += 1;

						if (_slots[i].ClientSession == clientsession)
							_slots[i].isGameLoaded = true;
					}
				}
			}

			if (isAllLoadedFailCnt == 1)
			{
				// CreatePacket을 보내라고 명령
				_inGame = new InGame(this, SelectedMap);

				for (int i = 0; i < _slots.Length; ++i)
				{
					if (_slots[i].ClientSession != null)
					{
						_slots[i].isGameLoaded = false;
						_slots[i].ClientSession.ServerState = PlayerServerState.ServerStateGame;
					}
				}
			}
		}

		public void BroadcastPacket<T>(T packet) where T : IMessage
		{
			for (int i = 0; i < _slots.Length; i++)
			{
				if (_slots[i].ClientSession != null)
					_slots[i].ClientSession.Send(packet);
			}
		}

		public void TestFunc()
		{
			Console.WriteLine($"Test Func Called from Room_{_roomId}");
			 
		}
	}
}
