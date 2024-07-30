using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public partial class GameLogic : JobSerializer
    {
        public void HandleCreateRoom(ClientSession clientSession, C_CreateRoom createrRoomPacket)
        {
            // 방을 생성해 줘야 한다. 
            Console.WriteLine("서버에서 방을 생성해 줘야한다!");

            GameRoom NewRoom = AddRoom();
            clientSession.JoinRoom(NewRoom);
            clientSession.HandleServerStateChange(clientSession, PlayerServerState.ServerStateRoom);
            NewRoom.Push(NewRoom.TestFunc);

            Console.WriteLine($"There are {_rooms.Count} Rooms in Server Now");
        }

    }
}


 