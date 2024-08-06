using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class RoomManager
    {
        public static RoomManager Instance { get; } = new RoomManager();

        Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        int _roomId = 1;

        private const double RoomUpdateTime = 16.67;

        public IReadOnlyDictionary<int, GameRoom> Rooms => _rooms;

        object _lock = new object(); // 지금은 JobSerializer에 전부 담아서 시행함으로 딱히 필요가 없다. 

        public void HandleCreateRoom(ClientSession clientSession, C_CreateRoom createRoomPacket)
        {
            GameRoom NewRoom = AddRoom(createRoomPacket.Roominfo);

            NewRoom.AddClient(clientSession);
         

            Console.WriteLine($"There are {_rooms.Count} Rooms in Server Now");

            BroadcastRoomCreation(NewRoom, clientSession);

     
            // 두번째 할일. 상점 같은 곳에서 플레이어가 Lobby로 돌아왔을때 기존에 Lobby에 있던 방들을 모두 보내줘야한다. 
            // 지금은 이 rooms를 GameLogic Class에서 관리하고 있는데 RoomsManager Class를 만들어서 이들을 관리하고 
            // 패킷을 보내는 Logic도 여기서 작성하는 방향으로 변경. 



            // 세번째 할일
            // 서버에서 JoinRoom 이라는 Packet을 만들어서 클라이언트에게 보내줘야함.
            // 방을 만든 Client는 바로 S_JoinRoom Packet을 Handle 해서 방에 입장이 가능
            // 다른 클라이언들은 C_JoinRoom Packet을 통해서 Server에게 허락맡고 Server가 S_JoinRoom Packet을
            // 다시 보내줘서 입장 가능 .
            // S_JoinRoom Packet에는 현재 입장해 있는 Player들에 대한 정보를 넘겨줘야지 Client에서
            // 그에 맞는 Player Image를 생성할 수 있다. (하지만 아직 Player Image에 대한 정보를 관리하고 있지는 않음)
            // 대충 지금 존재하는 인원과 Room에서의 위치 (1 ~ 8) 어느 Room이 열려 있고 닫혀있는지에 대한 정보를 
            // 보내줘서 그에 대한 동기화를 해보자. 
        }


        private void BroadcastRoomCreation(GameRoom newRoom, ClientSession excludeSession)
        {
            S_AlterRoom alterRoomPacket = new S_AlterRoom();
            alterRoomPacket.Altertype = RoomAlterType.Add;
            alterRoomPacket.Roominfo = newRoom._roomInfo;

            foreach (KeyValuePair<int, PlayerInfo> kvp in UserManager.Instance.Players)
            {
                // 나중에 확장성 있게 고쳐야 한다.
                // 지금은 로비에서만 채팅을 친다고 가정 
                // 패킷에다가 서버스테이트가 무엇인지, InGame이라면 
                // Room Number에 따라 해당 룸에서만 BroadCast되도록 해야할듯
                if (kvp.Value.Session._serverState == PlayerServerState.ServerStateLobby && kvp.Value.Session != excludeSession)
                {
                    kvp.Value.Session.Send(alterRoomPacket);
                }
            }
        }

        private void BroadCastRoomDeletion(int roomId)
        {
            S_AlterRoom alterRoomPacket = new S_AlterRoom();
            RoomInfo roomInfo = new RoomInfo();
            roomInfo.RoomNumber = roomId;
            alterRoomPacket.Altertype = RoomAlterType.Delete;
            alterRoomPacket.Roominfo = roomInfo;

            foreach (KeyValuePair<int, PlayerInfo> kvp in UserManager.Instance.Players)
            {
                if(kvp.Value.Session._serverState == PlayerServerState.ServerStateLobby)
                {
                    kvp.Value.Session.Send(alterRoomPacket);
                }
            }
        }

        public void BroadCastRoomAlter(int roomId)
        {
            if (_rooms.TryGetValue(roomId, out GameRoom gameRoom))
            {
                S_AlterRoom alterRoomPacket = new S_AlterRoom();
                alterRoomPacket.Altertype = RoomAlterType.Alter;
                alterRoomPacket.Roominfo = gameRoom._roomInfo;

                foreach (KeyValuePair<int, PlayerInfo> kvp in UserManager.Instance.Players)
                {
                    if (kvp.Value.Session._serverState == PlayerServerState.ServerStateLobby)
                        kvp.Value.Session.Send(alterRoomPacket);
                }
            }
            
        }

        public void ClientEnterRoom(ClientSession clientSession, int roomid)
        {
            if ( _rooms.TryGetValue(roomid, out GameRoom gameRoom))
            {
                gameRoom.AddClient(clientSession);
            }
        }


        public void EnterLobby(ClientSession clientsession)
        {
            foreach (KeyValuePair<int, GameRoom> kvp in _rooms)
            {
                S_AlterRoom alterRoom = new S_AlterRoom();
                alterRoom.Altertype = RoomAlterType.Add;
                alterRoom.Roominfo = kvp.Value._roomInfo;

                clientsession.Send(alterRoom);
            }
        }



        public GameRoom AddRoom(RoomInfo roominfo)
        {
            var room = new GameRoom(_roomId, roominfo);
            _rooms[_roomId] = room;
            _roomId++;

            Task.Run(() => GameRoomTask(room));

            return room;
        }

        static async void GameRoomTask(GameRoom room)
        {
            var updateInterval = TimeSpan.FromMilliseconds(RoomUpdateTime);
            while (!room._isClosed)
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

        public bool Remove(int roomId)
        {
            return _rooms.Remove(roomId);
        }


        // 새로 작성한 함수 
        public void RemoveRoom(int roomId)
        {
            if (_rooms.TryGetValue(roomId, out var room))
            {
                _rooms.Remove(roomId);

                // 클라이언트에게도 방이 사라졌다는 정보를 보내야 한다. 
                BroadCastRoomDeletion(roomId);


                Console.WriteLine($"Room {roomId} has been removed. There are {_rooms.Count} Rooms in Server Now");
            }
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


 
