using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
 
namespace Server.Game
{
	public class GameLogic : JobSerializer
	{
		public static GameLogic Instance { get; } = new GameLogic();
		private const double UpdateTime = 16.67;

		Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
		int _roomId = 1;

		public void AddRoom()
		{
			var room = new GameRoom(_roomId);
			_rooms[_roomId] = room;
			_roomId++;

			Task.Run(() => GameRoomTask(room));

			//Thread roomThread = new Thread(() => GameRoomTask(room));
			//roomThread.Name = $"GameRoom_{room.RoomId}";
			//roomThread.Start();
		}

		static async void GameRoomTask(GameRoom room)
		{
			//while (true)
			//{
			//	Console.WriteLine($"Running Thread : {room.RoomId}");
			//	room.Update();
			//	Thread.Sleep(0);
			//}

			var updateInterval = TimeSpan.FromMilliseconds(UpdateTime);
			while (true)
			{
				var startTime = DateTime.UtcNow;

				room.Update();

				var elapsedTime = DateTime.UtcNow - startTime;
				var delayTime = updateInterval - elapsedTime;
				if (delayTime > TimeSpan.Zero)
				{
					await Task.Delay(delayTime);
				}

            }
		}


		public void Update()
		{
			Flush();

			UserManager.Instance.Update();

			//foreach (GameRoom room in _rooms.Values)
			//{
			//	room.Update();
			//}
		}

		public GameRoom Add(int mapId)
		{
			GameRoom gameRoom = new GameRoom(_roomId);
			gameRoom.Push(gameRoom.Init, mapId, 10);

			gameRoom.RoomId = _roomId;
			_rooms.Add(_roomId, gameRoom);
			_roomId++;

			return gameRoom;
		}

		public bool Remove(int roomId)
		{
			return _rooms.Remove(roomId);
		}

		public GameRoom Find(int roomId)
		{
			GameRoom room = null;
			if (_rooms.TryGetValue(roomId, out room))
				return room;

			return null;
		}
	}
}
