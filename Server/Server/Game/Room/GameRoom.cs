using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
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
		}


		public RoomInfo _roomInfo { get; private set; }

		public int _roomId { get; set; }

		// 동적 배열 방식에서 Array로 교환 필요.. 흠.. 
		//private List<ClientSession> _sessions = new List<ClientSession>();
		private Slot[] _slots = new Slot[8];


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


		public bool AddClient(ClientSession session)
		{
			int? slotId = GetNextAvailableSlot();
			if (slotId == null)
				return false; 

			session.SlotId = slotId.Value;
			session.JoinRoom(this);
			_slots[slotId.Value].ClientSession = session;
			_slots[slotId.Value].IsAvailable = false;
			return true;
		}

		public bool RemoveClient(ClientSession session)
		{
			int slotId = session.SlotId;
			if (slotId < 0 || slotId > _slots.Length || _slots[slotId].ClientSession != session)
			{
				Console.WriteLine("GameRoom 에서 Client 삭제 실패 (치명적 오류)");
				return false;
			}

			_slots[slotId].ClientSession = null;
			_slots[slotId].IsAvailable = true;
			session.LeaveRoom();
			return true;
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


		public int? GetNextAvailableSlot()
		{
			for (int i = 0; i < _slots.Length; i++)
			{
				if (_slots[i].IsAvailable)
					return i;
			}

			return null;
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


		public void TestFunc()
		{
			Console.WriteLine($"Test Func Called from Room_{_roomId}");
			 
		}
	}
}
